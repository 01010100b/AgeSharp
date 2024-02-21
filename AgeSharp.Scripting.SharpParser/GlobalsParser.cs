using AgeSharp.Common;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            var fields = new List<IFieldSymbol>();

            foreach (var type in parse.Compilation.GetSymbolsWithName(x => true, SymbolFilter.Type).OfType<INamedTypeSymbol>())
            {
                fields.Clear();

                foreach (var field in type.GetMembers().OfType<IFieldSymbol>())
                {
                    var attr = field.GetAttributes().FirstOrDefault(x => x.AttributeClass!.Name == nameof(AgeGlobalAttribute));

                    if (attr is null)
                    {
                        continue;
                    }

                    Throw.If<NotSupportedException>(!field.IsStatic, $"Global {field.Name} is not static.");

                    fields.Add(field);
                }

                if (fields.Count == 0)
                {
                    continue;
                }

                var tree = type.DeclaringSyntaxReferences.Single().SyntaxTree;
                var model = parse.Compilation.GetSemanticModel(tree);

                foreach (var field in fields)
                {
                    var syntax = (VariableDeclaratorSyntax)field.DeclaringSyntaxReferences.Single().GetSyntax();
                    var field_type = parse.GetType(field.Type);

                    if (field_type is ArrayType)
                    {
                        Throw.IfNull<NotSupportedException>(syntax.Initializer, $"Global {field} is array without initializer.");
                        var init = (IFieldInitializerOperation)model.GetOperation(syntax.Initializer)!;
                        Debug.WriteLine($"init {init}");

                        var value = init.Value;

                        if (value is IConversionOperation conv)
                        {
                            value = conv.Operand;
                        }

                        if (value is IObjectCreationOperation create)
                        {
                            Throw.If<NotSupportedException>(create.Arguments.Length != 1, $"Global {field} is array without exactly 1 initializer argument.");
                            value = create.Arguments.Single().Value;
                        }

                        if (value is IConversionOperation conv2)
                        {
                            value = conv2.Operand;
                        }

                        Throw.If<NotSupportedException>(!value.ConstantValue.HasValue, $"Global {field} initialized with argument not a compile-time constant.");
                        var length = (int)value.ConstantValue.Value!;
                        field_type = parse.GetType(field.Type, length);
                    }
                    else
                    {
                        Throw.If<NotSupportedException>(syntax.Initializer is not null, $"Global {field} is not array but has initializer.");
                    }

                    var v = new Variable(field.ToString()!, field_type);
                    parse.Script.GlobalScope.AddVariable(v);
                    parse.AddGlobal(field, v);
                }
            }
        }
    }
}
