using AgeSharp.Common;
using AgeSharp.Scripting.Language;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.SharpParser
{
    internal static class GlobalsParser
    {
        public static void Parse(Parse parse)
        {
            foreach (var field in parse.Compilation.GetSymbolsWithName(x => true, SymbolFilter.Member).OfType<IFieldSymbol>())
            {
                var attr = field.GetAttributes().FirstOrDefault(x => x.AttributeClass!.Name == nameof(AgeGlobalAttribute));

                if (attr is null)
                {
                    continue;
                }

                Debug.WriteLine($"Found field {field}");
                if (!field.IsStatic) throw new NotSupportedException($"Global {field} is not static.");
                Throw.If<NotSupportedException>(field.ContainingType.StaticConstructors.Any(), $"The type where global {field} is declared has a static constructor.");

                var type = parse.GetType(field.Type);
                var v = new Variable(field.ToString()!, type);
                parse.Script.GlobalScope.AddVariable(v);
                parse.AddGlobal(field, v);
            }
        }
    }
}
