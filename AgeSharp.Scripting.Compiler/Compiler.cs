using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Compiler.Rules;
using AgeSharp.Scripting.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler
{
    public class Compiler
    {
        public Script CreateScript()
        {
            var script = new Script();

            var assembly = GetType().Assembly;

            return script;
        }

        public CompilationResult Compile(Script script, Settings settings)
        {
            Validate(script, settings);

            var memory = new Memory();
            memory.Compile(script, settings);

            var instruction_compiler = new InstructionCompiler();
            var instructions = instruction_compiler.Compile(script, memory, settings);

            var rule_compiler = new RuleCompiler();
            var rules = rule_compiler.Compile(instructions, settings);

            var result = new CompilationResult(script, settings, memory, instructions, rules);

            return result;
        }

        private void Validate(Script script, Settings settings)
        {
            throw new NotImplementedException();
        }
    }
}
