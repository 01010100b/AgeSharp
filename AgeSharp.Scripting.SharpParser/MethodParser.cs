using AgeSharp.Common;
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
using System.Reflection;
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
            Throw.If<NotSupportedException>(symbol.ReturnsByRef || symbol.ReturnsByRefReadonly, $"Method {name} has ref return.");
            if (return_type is ArrayType) throw new NotSupportedException($"Method {name} returns array.");

            var method = new Method(parse.Script) { Name = name, ReturnType = return_type };

            foreach (var parameter in symbol.Parameters)
            {
                var pname = parameter.Name;
                var type = parse.GetType(parameter.Type);
                var isref = type is ArrayType;
                
                if (parameter.HasExplicitDefaultValue) throw new NotSupportedException($"Method {name} parameter {pname} has default value.");

                switch (parameter.RefKind)
                {
                    case RefKind.Ref:
                    case RefKind.In:
                    case RefKind.RefReadOnlyParameter: isref = true; break;
                    case RefKind.Out: throw new NotSupportedException($"Method {name} parameter {pname} is out parameter.");
                }

                if (isref)
                {
                    type = parse.Script.GetRefType(type);
                }
                
                var v = new Variable(pname, type);
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
                ParseBlock((IBlockOperation)model.GetOperation(syntax.Body)!, method.Block, parse);
            }

            if (method.ReturnsVoid)
            {
                if (method.Block.Statements.Count == 0 || method.Block.Statements[^1] is not ReturnStatement)
                {
                    method.Block.Statements.Add(new ReturnStatement(method.Block.Scope, null));
                }
            }
        }

        private static void ParseBlock(IBlockOperation operation, Block block, Parse parse)
        {
            foreach (var local in operation.Operations.OfType<IVariableDeclarationGroupOperation>())
            {
                ParseLocal(local, block, parse);
            }

            foreach (var op in operation.Operations.Where(x => x is not IVariableDeclarationGroupOperation))
            {
                ParseStatement(op, block, parse);
            }
        }

        private static void ParseStatement(IOperation op, Block block, Parse parse)
        {
            if (op is IBlockOperation blockop)
            {
                var bb = new Block(block.Scope);
                ParseBlock(blockop, bb, parse);
                block.Statements.Add(bb);
            }
            else if (op is IReturnOperation ret)
            {
                if (ret.ReturnedValue is not null)
                {
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
                    if (assign is ISimpleAssignmentOperation simple)
                    {
                        var target = (AccessorExpression)ParseExpression(simple.Target, parse);
                        var value = ParseExpression(simple.Value, parse);
                        block.Statements.Add(new AssignStatement(block.Scope, target, value, simple.IsRef));
                    }
                    else if (assign is ICompoundAssignmentOperation compound)
                    {
                        var target = (AccessorExpression)ParseExpression(compound.Target, parse);
                        var value = ParseExpression(compound.Value, parse);
                        var method = parse.GetMethod(compound.OperatorMethod!);
                        var callexpr = new CallExpression(method);
                        callexpr.AddArgument(target);
                        callexpr.AddArgument(value);
                        block.Statements.Add(new AssignStatement(block.Scope, target, callexpr, false));
                    }
                    else
                    {
                        throw new NotSupportedException($"Assign {assign} not supported.");
                    }
                }
                else if (exprst.Operation is IInvocationOperation invoke)
                {
                    var right = ParseExpression(invoke, parse);
                    block.Statements.Add(new AssignStatement(block.Scope, null, right, false));
                }
                else if (exprst.Operation is IIncrementOrDecrementOperation incr)
                {
                    var left = (AccessorExpression)ParseExpression(incr.Target, parse);
                    var right = ParseExpression(incr, parse);
                    block.Statements.Add(new AssignStatement(block.Scope, left, right, false));
                }
                else
                {
                    throw new NotSupportedException($"Expression statement {exprst.Operation} not recognized.");
                }
            }
            else if (op is IConditionalOperation conditional)
            {
                var condition = ParseExpression(conditional.Condition, parse);
                var ifs = new IfStatement(block.Scope, condition);

                if (conditional.WhenTrue is not null)
                {
                    if (conditional.WhenTrue is IBlockOperation wt)
                    {
                        ParseBlock(wt, ifs.WhenTrue, parse);
                    }
                    else
                    {
                        ParseStatement(conditional.WhenTrue, ifs.WhenTrue, parse);
                    }
                }

                if (conditional.WhenFalse is not null)
                {
                    if (conditional.WhenFalse is IBlockOperation wf)
                    {
                        ParseBlock(wf, ifs.WhenFalse, parse);
                    }
                    else
                    {
                        ParseStatement(conditional.WhenFalse, ifs.WhenFalse, parse);
                    }
                }

                block.Statements.Add(ifs);
            }
            else if (op is IForLoopOperation forloop)
            {
                Throw.IfNull<NotSupportedException>(forloop.Condition, "For loop without condition.");
                Throw.If<NotSupportedException>(forloop.ConditionLocals.Any(), "For loop with locals in condition.");
                Throw.If<NotSupportedException>(forloop.Body is not IBlockOperation, "For loop with body not a block.");

                var scoping_block = new Block(block.Scope);

                // parse variables
                
                foreach (var decl in forloop.Before.OfType<IVariableDeclarationGroupOperation>())
                {
                    ParseLocal(decl, scoping_block, parse);
                }

                var condition = ParseExpression(forloop.Condition, parse);
                var loop = new LoopStatement(scoping_block, condition);

                foreach (var statement in scoping_block.Statements)
                {
                    loop.Before.Statements.Add(statement);
                }

                scoping_block.Statements.Clear();
                var body = (IBlockOperation)forloop.Body;

                foreach (var decl in body.Operations.OfType<IVariableDeclarationGroupOperation>())
                {
                    ParseLocal(decl, loop.Body, parse);
                }
                
                foreach (var decl in forloop.AtLoopBottom.OfType<IVariableDeclarationGroupOperation>())
                {
                    ParseLocal(decl, loop.AtLoopBottom, parse);
                }

                foreach (var v in loop.Body.Scope.Variables.ToList())
                {
                    loop.Body.Scope.MoveVariable(v, loop.Scope);
                }

                foreach (var v in loop.AtLoopBottom.Scope.Variables.ToList())
                {
                    loop.AtLoopBottom.Scope.MoveVariable(v, loop.Scope);
                }

                // parse statements

                foreach (var st in forloop.Before.Where(x => x is not IVariableDeclarationGroupOperation))
                {
                    ParseStatement(st, loop.Before, parse);
                }

                foreach (var st in body.Operations.Where(x => x is not IVariableDeclarationGroupOperation))
                {
                    ParseStatement(st, loop.Body, parse);
                }

                foreach (var st in forloop.AtLoopBottom.Where(x => x is not IVariableDeclarationGroupOperation))
                {
                    ParseStatement(st, loop.AtLoopBottom, parse);
                }

                block.Statements.Add(loop);
            }
            
            else
            {
                throw new NotSupportedException($"Operation {op} {op.GetType().Name} not supported.");
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
                    throw new NotSupportedException($"Const of type {expression.Type!.Name}.");
                }
            }
            else if (expression is IConversionOperation conversion)
            {
                if (!conversion.Conversion.IsUserDefined) throw new NotSupportedException($"Conversion {conversion} not supported.");
                if (!parse.IsInternal(conversion.OperatorMethod!)) throw new NotSupportedException($"Conversion {conversion} not supported.");

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
            else if (expression is IFieldReferenceOperation field)
            {
                if (field.Field.IsStatic)
                {
                    var variable = parse.GetGlobal(field.Field);

                    return new AccessorExpression(variable);
                }
                else
                {
                    var type = (CompoundType)parse.GetType(field.Field.ContainingType);
                    var type_field = type.Fields.Single(x => x.Name == field.Field.Name);
                    var instance = ParseExpression(field.Instance!, parse);

                    if (instance is AccessorExpression ae)
                    {
                        if (ae.IsVariableAccess)
                        {
                            return new AccessorExpression(ae.Variable, [type_field]);
                        }
                        else if (ae.IsStructAccess)
                        {
                            return new AccessorExpression(ae.Variable, ae.Fields!.Append(type_field));
                        }
                        else
                        {
                            throw new NotSupportedException($"Accessor not variable access for field {field}.");
                        }
                    }
                    else
                    {
                        throw new NotSupportedException($"Can not parse field access {field}.");
                    }
                }
            }
            else if (expression is IInvocationOperation call)
            {
                var method = parse.GetMethod(call.TargetMethod);
                var callexpr = new CallExpression(method);

                for (int i = 0; i < call.Arguments.Length; i++)
                {
                    var argop = call.Arguments[i];

                    if (argop.Value.Type!.SpecialType == SpecialType.System_String)
                    {
                        if (argop.Value is ILiteralOperation literal)
                        {
                            if (i != 0) throw new NotSupportedException($"Call to method {method.Name} with string literal not as first argument.");
                            if (!literal.ConstantValue.HasValue) throw new NotSupportedException($"String literal must be constant.");
                            
                            var lit = (string)literal.ConstantValue.Value!;
                            callexpr = new CallExpression(method, lit);
                        }
                        else
                        {
                            throw new NotSupportedException($"String literal argument {argop.Value.GetType().Name} not supported.");
                        }
                    }
                    else
                    {
                        var arg = ParseExpression(argop, parse);
                        callexpr.AddArgument(arg);
                    }
                }

                return callexpr;
            }
            else if (expression is IArgumentOperation arg)
            {
                return ParseExpression(arg.Value, parse);
            }
            else if (expression is IBinaryOperation bin)
            {
                var method = parse.GetMethod(bin.OperatorMethod!);
                var callexpr = new CallExpression(method);
                callexpr.AddArgument(ParseExpression(bin.LeftOperand, parse));
                callexpr.AddArgument(ParseExpression(bin.RightOperand, parse));

                return callexpr;
            }
            else if (expression is IIncrementOrDecrementOperation incr)
            {
                var method = parse.GetMethod(incr.OperatorMethod!);
                var callexpr = new CallExpression(method);
                callexpr.AddArgument(ParseExpression(incr.Target, parse));

                return callexpr;
            }
            else if (expression is IPropertyReferenceOperation prop)
            {
                var v = ((AccessorExpression)ParseExpression(prop.Instance!, parse)).Variable;

                if (v.Type is ArrayType)
                {
                    if (prop.Property.ToString()!.Contains(">.this[AgeSharp.Scripting.SharpParser.Int]"))
                    {
                        var index = ParseExpression(prop.Arguments.Single(), parse);

                        return new AccessorExpression(v, index);
                    }
                }

                throw new NotSupportedException($"Property reference {prop} not supported.");
            }
            else if (expression is IUnaryOperation unary)
            {
                Throw.IfNull<NotSupportedException>(unary.OperatorMethod, $"Unary {unary} does not have operator method.");

                if (unary.OperatorMethod.ToString() == "AgeSharp.Scripting.SharpParser.Bool.operator true(AgeSharp.Scripting.SharpParser.Bool)")
                {
                    return ParseExpression(unary.Operand, parse);
                }
                else if (unary.OperatorMethod.ToString() == "AgeSharp.Scripting.SharpParser.Bool.operator false(AgeSharp.Scripting.SharpParser.Bool)")
                {
                    throw new NotImplementedException($"Unary false operator not implemented.");
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                throw new NotSupportedException($"Expression {expression} {expression.GetType().Name} not supported.");
            }
        }

        private static void ParseLocal(IVariableDeclarationGroupOperation operation, Block block, Parse parse)
        {
            foreach (var variable in operation.Declarations.SelectMany(x => x.Declarators))
            {
                var local = variable.Symbol;
                var init = variable.GetVariableInitializer();
                var name = local.Name;
                var type = parse.GetType(local.Type);

                if (type is ArrayType)
                {
                    if (init is null) throw new NotSupportedException($"Array {name} without init.");
                    if (init.Value is not IConversionOperation conversion) throw new NotSupportedException($"Array {name} init not conversion.");
                    if (conversion.Operand is not IObjectCreationOperation creation) throw new NotSupportedException($"Array {name} init not object creation.");

                    var arg = creation.Arguments.Single().Value;
                    if (arg is not IConversionOperation argconv) throw new NotSupportedException($"Array {name} arg is not conversion.");
                    if (!argconv.Operand.ConstantValue.HasValue) throw new NotSupportedException($"Array {name} init without const length.");
                    var length = (int)argconv.Operand.ConstantValue.Value!;
                    type = parse.GetType(local.Type, length);
                }

                var v = new Variable(name, type);
                block.Scope.AddVariable(v);
                parse.AddLocal(local, v);

                if (type is not ArrayType && init is not null)
                {
                    var right = ParseExpression(init.Value, parse);
                    Throw.If<NotSupportedException>(right is not ConstExpression, $"Local {type.Name} {name} has non-constant initializer.");
                    var left = new AccessorExpression(v);
                    block.Statements.Add(new AssignStatement(block.Scope, left, right, false));
                }
            }
        }
    }
}
