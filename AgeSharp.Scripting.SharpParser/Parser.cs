using AgeSharp.Scripting.Compiler;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            
            var compilation = CSharpCompilation.Create("MyCompilation", trees, references);
            var script = ScriptCompiler.CreateScript();
            var parse = new Parse(script, compilation);

            var int_type = compilation.GetTypeByMetadataName("AgeSharp.Scripting.SharpParser.Int")!;
            var bool_type = compilation.GetTypeByMetadataName("AgeSharp.Scripting.SharpParser.Bool")!;
            parse.AddType(int_type, PrimitiveType.Int);
            parse.AddType(bool_type, PrimitiveType.Bool);

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
