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

        public RuleInstruction(string fact) : this(fact, Enumerable.Empty<string>()) { }

        public RuleInstruction(string fact, string command) : this(fact, [command]) { }

        public RuleInstruction(string fact, IEnumerable<string> commands) : base()
        {
            Fact = fact;
            Commands.AddRange(commands);
        }
    }
}
