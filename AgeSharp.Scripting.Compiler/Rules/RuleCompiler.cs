using AgeSharp.Scripting.Compiler.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Rules
{
    internal class RuleCompiler(IEnumerable<Instruction> instructions, Settings settings)
    {
        private IEnumerable<Instruction> Instructions { get; } = instructions.ToList();
        private Settings Settings { get; } = settings;

        public List<Rule> Compile()
        {
            throw new NotImplementedException();
        }
    }
}
