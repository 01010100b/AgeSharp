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
        public List<string> Commands { get; } = new();

        public RuleInstruction(string fact) : base()
        {
            Fact = fact;
        }
    }
}
