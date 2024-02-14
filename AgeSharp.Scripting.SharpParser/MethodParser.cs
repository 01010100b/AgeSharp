using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Statements;
using AgeSharp.Scripting.Language.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.SharpParser
{
    internal static class MethodParser
    {
        public static void Parse(Parse parse)
        {
            var methods = new List<(MethodDeclarationSyntax, IMethodSymbol, SemanticModel, bool)>();

            foreach (var tree in parse.Compilation.SyntaxTrees)
            {
                var model = parse.Compilation.GetSemanticModel(tree);

                foreach (var method in tree.GetCompilationUnitRoot().DescendantNodes().OfType<MethodDeclarationSyntax>())
                {
                    var symbol = model.GetDeclaredSymbol(method) ?? throw new Exception($"Can not find symbol for method {method.Identifier.ValueText}.");

                    foreach (var attribute in symbol.GetAttributes().Where(x => x.AttributeClass!.Name == nameof(AgeMethodAttribute)))
                    {
                        Debug.WriteLine($"method {symbol}");
                        var entry_point = false;

                        foreach (var arg in attribute.NamedArguments)
                        {
                            if (arg.Key == "EntryPoint")
                            {
                                var value = arg.Value.Value ?? throw new Exception("Attribute EntryPoint is null.");

                                if ((bool)value)
                                {
                                    entry_point = true;
                                    Debug.WriteLine($"entry point {symbol}");
                                }
                            }
                        }

                        methods.Add((method, symbol, model, entry_point));

                        break;
                    }
                }
            }

            foreach (var method in methods)
            {
                ParseDefinition(method.Item1, method.Item2, method.Item3, method.Item4, parse);
            }

            foreach (var method in methods)
            {
                ParseBody(method.Item1, method.Item2, method.Item3, parse);
            }
        }

        private static void ParseDefinition(MethodDeclarationSyntax method, IMethodSymbol symbol, SemanticModel model, bool entry_point, Parse parse)
        {
            var name = symbol.ToString()!;
            var return_type = symbol.ReturnType.SpecialType == SpecialType.System_Void ? PrimitiveType.Void : parse.Types[(INamedTypeSymbol)symbol.ReturnType];
            var m = new Method(parse.Script) { Name = name, ReturnType = return_type };


            
            if (m.Block.Statements.Count == 0)
            {
                m.Block.Statements.Add(new ReturnStatement(m.Block.Scope, null));
            }

            parse.Script.AddMethod(m, entry_point);
        }

        private static void ParseBody(MethodDeclarationSyntax method, IMethodSymbol symbol, SemanticModel model, Parse parse)
        {

        }
    }
}
