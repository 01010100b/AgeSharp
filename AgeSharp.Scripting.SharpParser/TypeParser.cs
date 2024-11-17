using AgeSharp.Common;
using AgeSharp.Scripting.Language.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AgeSharp.Scripting.SharpParser
{
    internal static class TypeParser
    {
        public static void Parse(Parse parse)
        {
            var types = new List<INamedTypeSymbol>();

            foreach (var symbol in parse.Compilation.GetSymbolsWithName(x => true, SymbolFilter.Type).OfType<INamedTypeSymbol>())
            {
                var attr = symbol.GetAttributes().FirstOrDefault(x => x.AttributeClass!.Name == nameof(AgeTypeAttribute));

                if (attr is null)
                {
                    continue;
                }

                Throw.If<NotSupportedException>(symbol.IsRefLikeType, $"Type {symbol.Name} is ref type.");
                types.Add(symbol);
                var type = new CompoundType(symbol.ToString()!);
                parse.Script.AddType(type);
                parse.AddType(symbol, type);

            }

            foreach (var symbol in types)
            {
                var type = (CompoundType)parse.GetType(symbol);

                foreach (var field in symbol.GetMembers().OfType<IFieldSymbol>())
                {
                    if (field.IsStatic)
                    {
                        continue;
                    }

                    var syntax = (VariableDeclaratorSyntax)field.DeclaringSyntaxReferences.Single().GetSyntax();
                    var field_type = parse.GetType(field.Type);
                    type.AddField(new(field.Name, field_type));
                }
            }
        }
    }
}
