using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;
using AgeSharp.Scripting.Language.Statements;
using AgeSharp.Scripting.Language.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AgeSharp.Scripting.SharpParser
{
    internal static class MethodParser
    {
        public static void Parse(Parse parse)
        {
            var methods = new List<(IMethodSymbol, SemanticModel, bool)>();

            foreach (var tree in parse.Compilation.SyntaxTrees)
            {
                var model = parse.Compilation.GetSemanticModel(tree);

                foreach (var method in tree.GetCompilationUnitRoot().DescendantNodes().OfType<MethodDeclarationSyntax>())
                {
                    var symbol = model.GetDeclaredSymbol(method) ?? throw new Exception($"Can not find symbol for method {method.Identifier.ValueText}.");
                    var attr = symbol.GetAttributes().FirstOrDefault(x => x.AttributeClass!.Name == nameof(AgeMethodAttribute));

                    if (attr is null)
                    {
                        continue;
                    }

                    if (!symbol.IsStatic) throw new NotSupportedException($"Method {symbol} is not static.");

                    var entry_point = false;
                    if (attr.NamedArguments.Any())
                    {
                        entry_point = (bool)attr.NamedArguments.Single(x => x.Key == nameof(AgeMethodAttribute.EntryPoint)).Value.Value!;
                    }

                    methods.Add((symbol, model, entry_point));
                }
            }

            foreach (var method in methods)
            {
                ParseDefinition(method.Item1, method.Item2, method.Item3, parse);
            }

            foreach (var method in methods)
            {
                ParseBody(method.Item1, method.Item2, parse);
            }
        }

        private static void ParseDefinition(IMethodSymbol symbol, SemanticModel model, bool entry_point, Parse parse)
        {
            var name = symbol.ToString()!;
            var return_type = symbol.ReturnsVoid ? PrimitiveType.Void : parse.GetType(symbol.ReturnType);
            if (symbol.ReturnsByRef || symbol.ReturnsByRefReadonly) throw new NotSupportedException($"Method {name} has ref return.");
            if (return_type is ArrayType) throw new NotSupportedException($"Method {name} returns array.");

            var method = new Method(parse.Script) { Name = name, ReturnType = return_type };

            foreach (var parameter in symbol.Parameters)
            {
                var pname = parameter.Name;
                var isref = parameter.Type.SpecialType == SpecialType.System_Array;
                
                if (parameter.HasExplicitDefaultValue) throw new NotSupportedException($"Method {name} parameter {pname} has default value.");

                switch (parameter.RefKind)
                {
                    case RefKind.Ref:
                    case RefKind.In:
                    case RefKind.RefReadOnlyParameter: isref = true; break;
                    case RefKind.Out: throw new NotSupportedException($"Method {name} parameter {pname} is out parameter.");
                }

                var type = parse.GetType(parameter.Type);
                var v = new Variable(pname, type, isref);
                method.AddParameter(v);
                parse.AddParameter(parameter, v);
            }

            parse.Script.AddMethod(method, entry_point);
            parse.AddMethod(symbol, method);
        }

        private static void ParseBody(IMethodSymbol symbol, SemanticModel model, Parse parse)
        {
            var method = parse.GetMethod(symbol);

            if (symbol.DeclaringSyntaxReferences.Length != 1) throw new NotSupportedException($"Method {symbol} has multiple bodies.");
            var syntax = (MethodDeclarationSyntax)symbol.DeclaringSyntaxReferences.Single().GetSyntax();

            if (syntax.Body is not null)
            {
                ParseBlock((IBlockOperation)model.GetOperation(syntax.Body)!, method.Block, model, parse);
            }

            if (method.Block.Statements.Count == 0 && method.ReturnsVoid)
            {
                method.Block.Statements.Add(new ReturnStatement(method.Block.Scope, null));
            }
        }

        private static void ParseBlock(IBlockOperation operation, Block block, SemanticModel model, Parse parse)
        {
            foreach (var local in operation.Operations.OfType<IVariableDeclarationGroupOperation>())
            {
                ParseLocal(local, block, parse);
            }

            foreach (var op in operation.Operations.Where(x => x is not IVariableDeclarationGroupOperation))
            {
                if (op is IBlockOperation blockop)
                {
                    var bb = new Block(block.Scope);
                    ParseBlock(blockop, bb, model, parse);
                    block.Statements.Add(bb);
                }
                else if (op is IReturnOperation ret)
                {
                    if (ret.ReturnedValue is not null)
                    {
                        Debug.WriteLine($"ret {ret.ReturnedValue} {ret.ReturnedValue.GetType().Name}");
                        var expression = ParseExpression(ret.ReturnedValue, parse);
                        block.Statements.Add(new ReturnStatement(block.Scope, expression));
                    }
                    else
                    {
                        block.Statements.Add(new ReturnStatement(block.Scope, null));
                    }
                }
                else if (op is IExpressionStatementOperation exprst)
                {
                    if (exprst.Operation is IAssignmentOperation assign)
                    {
                        if (assign is not ISimpleAssignmentOperation) throw new NotSupportedException($"Assign {assign} is not simple assignment.");

                        var target = (AccessorExpression)ParseExpression(assign.Target, parse);
                        var value = ParseExpression(assign.Value, parse);
                        block.Statements.Add(new AssignStatement(block.Scope, target, value));
                    }
                    else if (exprst.Operation is IInvocationOperation invoke)
                    {
                        var right = ParseExpression(invoke, parse);
                        block.Statements.Add(new AssignStatement(block.Scope, null, right));
                    }
                    else
                    {
                        throw new NotSupportedException($"Expression statement {exprst.Operation} not recognized.");
                    }
                }
                else if (op is IInvocationOperation call)
                {
                    
                }
                else
                {
                    throw new NotSupportedException($"Operation {op.GetType().Name} not supported.");
                }
            }
        }

        private static Expression ParseExpression(IOperation expression, Parse parse)
        {
            if (expression.ConstantValue.HasValue)
            {
                if (expression.Type!.SpecialType == SpecialType.System_Int32)
                {
                    return new ConstExpression(PrimitiveType.Int, (int)expression.ConstantValue.Value!);
                }
                else if (expression.Type!.SpecialType == SpecialType.System_Boolean)
                {
                    return new ConstExpression(PrimitiveType.Bool, (bool)expression.ConstantValue.Value! ? 1 : 0);
                }
                else
                {
                    throw new NotSupportedException($"Const of type {expression.Type!.Name}");
                }
            }
            else if (expression is IConversionOperation conversion)
            {
                if (!conversion.Conversion.IsUserDefined) throw new NotSupportedException($"Conversion {conversion} not supported.");
                EnsureInternal(conversion.OperatorMethod!);

                return ParseExpression(conversion.Operand, parse);
            }
            else if (expression is IParameterReferenceOperation par)
            {
                var variable = parse.GetParameter(par.Parameter);

                return new AccessorExpression(variable);
            }
            else if (expression is ILocalReferenceOperation local)
            {
                var variable = parse.GetLocal(local.Local);

                return new AccessorExpression(variable);
            }
            else if (expression is IArrayElementReferenceOperation arr)
            {
                if (arr.Indices.Length != 1) throw new NotSupportedException($"Array indexing with not exactly 1 index.");
                
                var v = ((AccessorExpression)ParseExpression(arr.ArrayReference, parse)).Variable;
                var index = ParseExpression(arr.Indices.Single(), parse);

                return new AccessorExpression(v, index);
            }
            else if (expression is IInvocationOperation call)
            {
                var method = parse.GetMethod(call.TargetMethod);
                var callexpr = new CallExpression(method);

                foreach (var argop in call.Arguments)
                {
                    var arg = ParseExpression(argop, parse);
                    callexpr.AddArgument(arg);
                }

                return callexpr;
            }
            else if (expression is IArgumentOperation arg)
            {
                return ParseExpression(arg.Value, parse);
            }
            else
            {
                throw new NotImplementedException($"Not implemented expression {expression} {expression.GetType().Name}");
            }
        }

        private static void ParseLocal(IVariableDeclarationGroupOperation operation, Block block, Parse parse)
        {
            foreach (var variable in operation.Declarations.SelectMany(x => x.Declarators))
            {
                var local = variable.Symbol;
                var init = variable.GetVariableInitializer();
                var name = local.Name;
                var type = local.Type;
                
                if (type is IArrayTypeSymbol arr)
                {
                    if (init is null) throw new NotSupportedException($"Array {name} without init.");

                    if (init.Value is IArrayCreationOperation creation)
                    {
                        if (creation.DimensionSizes.Length != 1) throw new NotSupportedException($"Array {name} does not have exactly 1 dimension.");

                        var size = creation.DimensionSizes.Single();
                        if (!size.ConstantValue.HasValue) throw new NotSupportedException($"Array {name} init does not have constant length.");

                        var length = (int)size.ConstantValue.Value!;
                        var t = parse.GetType(type, length);
                        var v = new Variable(name, t, false);
                        block.Scope.AddVariable(v);
                        parse.AddLocal(local, v);
                    }
                    else
                    {
                        throw new NotSupportedException($"Array {name} init {init.Value} not recognized.");
                    }
                }
                else
                {
                    if (init is not null) throw new NotSupportedException($"Non-array local {name} has init.");
                    var t = parse.GetType(type);
                    var v = new Variable(name, t, false);
                    block.Scope.AddVariable(v);
                    parse.AddLocal(local, v);
                }
            }
        }

        private static void EnsureInternal(ISymbol symbol)
        {
            var namesp = symbol.ContainingNamespace;
            if (namesp is null) throw new Exception($"Symbol {symbol} has no namespace.");
            if (namesp.Name != "SharpParser") throw new Exception($"Symbol {symbol} is not internal.");
        }
    }
}
