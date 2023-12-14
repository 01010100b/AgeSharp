using AgeSharp.ScriptCompiler.Compiler;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgeSharp.ScriptCompiler
{
    public class CompilationResult
    {
        public IReadOnlyList<Instruction> Instructions { get; }
        public IReadOnlyList<Rule> Rules { get; }
        public string Per { get; }
        public Memory Memory { get; }

        internal CompilationResult(IReadOnlyList<Instruction> instructions, IReadOnlyList<Rule> rules,
            string per, Memory memory)
        {
            Instructions = instructions;
            Rules = rules;
            Per = per;
            Memory = memory;
        }
    }
}
