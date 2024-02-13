using AgeSharp.Scripting.Compiler;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Statements;
using AgeSharp.Scripting.Language.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deimos
{
    internal class Tests
    {
        public static CompilationResult Run()
        {
            var compiler = new Compiler();
            var script = compiler.CreateScript();
            var settings = new Settings();

            var main = new Method(script) { Name = "Main" };
            script.AddMethod(main, true);
            var va = new Variable("a", PrimitiveType.Int, false);
            main.Block.Scope.AddVariable(va);
            var vb = new Variable("b", script.GetOrAddArrayType(PrimitiveType.Int, 4), false);
            main.Block.Scope.AddVariable(vb);

            main.Block.Statements.Add(new ReturnStatement(main.Block.Scope, null));
            var result = compiler.Compile(script, settings);

            var lines = new List<string>();
            lines.AddRange(result.InstructionStream);
            lines.Add("");
            lines.Add("");
            lines.Add("--------------------------------------------------------------------------------------");
            lines.Add("");
            lines.Add("");
            lines.Add(result.GetPer());

            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "instructions.txt");
            File.WriteAllLines(file, lines);
            var psinfo = new ProcessStartInfo() { FileName = file, UseShellExecute = true };
            Process.Start(psinfo);

            return result;
        }
    }
}
