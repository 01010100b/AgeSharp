using AgeSharp.ScriptGenerator.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgeSharp.ScriptGenerator
{
    internal class MethodGenerator
    {
        public static void Generate(GeneratorExecutionContext context, List<MethodDeclarationSyntax> methods)
        {
            foreach (var method in methods)
            {
                var symbol = (IMethodSymbol)context.Compilation.GetSemanticModel(method.SyntaxTree).GetDeclaredSymbol(method);

                if (!IsAgeMethod(symbol))
                {
                    continue;
                }

                Generator.Log($"method {symbol.Name} is age method.");

                if (!symbol.IsStatic)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        new DiagnosticDescriptor
                        (
                            "AGE0001",
                            "Non-static method",
                            "Method {0} is not static. AgeSharp methods must be static.",
                            "",
                            DiagnosticSeverity.Error,
                            true
                        ),
                        symbol.Locations.FirstOrDefault(), symbol.Name));
                }

                if (!symbol.ReturnsVoid)
                {
                    var type = symbol.ReturnType;

                    if (type.SpecialType != SpecialType.System_Int32 && type.SpecialType != SpecialType.System_Boolean)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                        new DiagnosticDescriptor
                        (
                            "AGE0002",
                            "Invalid return type",
                            "Method {0} has an invalid return type. AgeSharp methods must return void or AgeSharp types.",
                            "",
                            DiagnosticSeverity.Error,
                            true
                        ),
                        symbol.Locations.FirstOrDefault(), symbol.Name));
                    }
                }
            }
        }

        private static bool IsAgeMethod(IMethodSymbol symbol)
        {
            foreach (var name in symbol.GetAttributes().Select(x => x.AttributeClass.Name))
            {
                if (name == nameof(AgeMainMethodAttribute) || name == nameof(AgeMethodAttribute))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
