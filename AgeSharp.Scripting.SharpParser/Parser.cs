﻿using AgeSharp.Common;
using AgeSharp.Scripting.Compiler;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;

namespace AgeSharp.Scripting.SharpParser
{
    public class Parser
    {
        public Script Parse(IEnumerable<string> sources)
        {
            var trees = new List<SyntaxTree>();

            foreach (var source in sources)
            {
                trees.Add(CSharpSyntaxTree.ParseText(source));
            }

            var references = GetNetCoreReferences();
            references.Add(MetadataReference.CreateFromFile(typeof(Parser).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(SearchSource).Assembly.Location));

            var compilation = CSharpCompilation.Create("MyCompilation", trees, references);
            var script = ScriptCompiler.CreateScript();
            var parse = new Parse(script, compilation);

            var int_type = compilation.GetTypeByMetadataName("AgeSharp.Scripting.SharpParser.Int")!;
            var bool_type = compilation.GetTypeByMetadataName("AgeSharp.Scripting.SharpParser.Bool")!;
            var point_type = compilation.GetTypeByMetadataName("AgeSharp.Scripting.SharpParser.Point")!;
            var search_state_type = compilation.GetTypeByMetadataName("AgeSharp.Scripting.SharpParser.SearchState")!;
            parse.AddType(int_type, PrimitiveType.Int);
            parse.AddType(bool_type, PrimitiveType.Bool);
            parse.AddType(point_type, BuiltinTypes.Point);
            parse.AddType(search_state_type, BuiltinTypes.SearchState);

            var intrinsics = compilation.GetTypeByMetadataName("AgeSharp.Scripting.SharpParser.Intrinsics");
            Debug.Assert(intrinsics is not null);

            foreach (var intrinsic in intrinsics.GetMembers().OfType<IMethodSymbol>())
            {
                var method = script.Methods.Single(x => x.Name == intrinsic.Name);
                parse.AddMethod(intrinsic, method);
            }

            foreach (var op in int_type.GetMembers().OfType<IMethodSymbol>())
            {
                var name = op.ToString()!;

                if (name.Contains("operator ==("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "Equals"));
                }
                else if (name.Contains("operator !=("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "NotEquals"));
                }
                else if (name.Contains("operator <("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "LessThan"));
                }
                else if (name.Contains("operator >("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "GreaterThan"));
                }
                else if (name.Contains("operator <=("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "LessThanOrEquals"));
                }
                else if (name.Contains("operator >=("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "GreaterThanOrEquals"));
                }
                else if (name.Contains("operator +("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "Add"));
                }
                else if (name.Contains("operator -("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "Sub"));
                }
                else if (name.Contains("operator ++("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "Increment"));
                }
                else if (name.Contains("operator --("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "Decrement"));
                }
                else if (name.Contains("operator *("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "Mul"));
                }
                else if (name.Contains("operator /("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "Div"));
                }
                else if (name.Contains("operator %("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "Mod"));
                }
                else if (name.Contains("operator &("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "BitwiseAnd"));
                }
                else if (name.Contains("operator |("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "BitwiseOr"));
                }
                else if (name.Contains("operator ~("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "BitwiseNot"));
                }
                else if (name.Contains("operator ^("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "BitwiseXor"));
                }
                else if (name.Contains("operator <<("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "ShiftLeft"));
                }
                else if (name.Contains("operator >>("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "ShiftRight"));
                }
            }

            foreach (var op in bool_type.GetMembers().OfType<IMethodSymbol>())
            {
                var name = op.ToString()!;

                if (name.Contains("operator ==("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "EqualsBool"));
                }
                else if (name.Contains("operator !=("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "NotEqualsBool"));
                }
                else if (name.Contains("operator &("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "And"));
                }
                else if (name.Contains("operator |("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "Or"));
                }
                else if (name.Contains("operator !("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "Not"));
                }
            }

            foreach (var op in point_type.GetMembers().OfType<IMethodSymbol>())
            {
                var name = op.ToString()!;

                if (name.Contains("operator ==("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "EqualsPoint"));
                }
                else if (name.Contains("operator !=("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "NotEqualsPoint"));
                }
                else if (name.Contains("operator +("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "AddPoint"));
                }
                else if (name.Contains("operator -("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "SubPoint"));
                }
                else if (name.Contains("operator *("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "MulPoint"));
                }
                else if (name.Contains("operator /("))
                {
                    parse.AddMethod(op, script.Methods.Single(x => x.Name == "DivPoint"));
                }
            }

            TypeParser.Parse(parse);
            GlobalsParser.Parse(parse);
            MethodParser.Parse(parse);

            return parse.Script;
        }

        private List<MetadataReference> GetNetCoreReferences()
        {
            var references = new List<MetadataReference>();

            var folder = Path.GetDirectoryName(typeof(object).Assembly.Location) +
                 Path.DirectorySeparatorChar;

            var assemblies = new List<string>()
            {
                folder + "System.Private.CoreLib.dll",
                folder + "System.Runtime.dll",
                folder + "System.Console.dll",
                folder + "netstandard.dll",

                folder + "System.Text.RegularExpressions.dll", // IMPORTANT!
                folder + "System.Linq.dll",
                folder + "System.Linq.Expressions.dll", // IMPORTANT!

                folder + "System.IO.dll",
                folder + "System.Net.Primitives.dll",
                folder + "System.Net.Http.dll",
                folder + "System.Private.Uri.dll",
                folder + "System.Reflection.dll",
                folder + "System.ComponentModel.Primitives.dll",
                folder + "System.Globalization.dll",
                folder + "System.Collections.Concurrent.dll",
                folder + "System.Collections.NonGeneric.dll",
                folder + "Microsoft.CSharp.dll"
            };

            foreach (var assembly in assemblies)
            {
                references.Add(MetadataReference.CreateFromFile(assembly));
            }

            return references;
        }
    }
}
