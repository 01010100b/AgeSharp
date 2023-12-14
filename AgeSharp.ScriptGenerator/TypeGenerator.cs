using AgeSharp.ScriptGenerator.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgeSharp.ScriptGenerator
{
    internal class TypeGenerator
    {
        public static void Generate(GeneratorExecutionContext context, List<TypeDeclarationSyntax> types)
        {
            foreach (var type in types)
            {
                var symbol = (ITypeSymbol)context.Compilation.GetSemanticModel(type.SyntaxTree).GetDeclaredSymbol(type);

                if (!IsAgeType(symbol))
                {
                    continue;
                }

                Generator.Log($"type {symbol.Name} is age type.");
                /*
                foreach (var field in type.Members.OfType<FieldDeclarationSyntax>().Select(x => x.Declaration))
                {
                    var type_symbol = (ITypeSymbol)context.Compilation.GetSemanticModel(field.SyntaxTree).GetDeclaredSymbol(field.Type);
                    Generator.Log($"field type {type_symbol.Name}");
                }*/
            }
        }

        private static bool IsAgeType(ITypeSymbol symbol)
        {
            foreach (var name in symbol.GetAttributes().Select(x => x.AttributeClass.Name))
            {
                if (name == nameof(AgeTypeAttribute))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
