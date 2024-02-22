using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Instructions
{
    internal class RuleInstruction : Instruction
    {
        public string Fact { get; }
        public List<string> Commands { get; } = [];

        public RuleInstruction(string fact) : base()
        {
            Fact = fact;
        }

        public RuleInstruction(string fact, IEnumerable<string> commands) : base()
        {
            Fact = fact;
            Commands.AddRange(commands);
        }
    }
}
