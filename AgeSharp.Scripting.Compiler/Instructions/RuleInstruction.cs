using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Instructions
{
    internal class RuleInstruction : Instruction
    {
        public IReadOnlyList<string> Facts { get; }
        public IReadOnlyList<string> Commands { get; }

        public RuleInstruction(string fact, string command) : this([fact], [command]) { }

        public RuleInstruction(string fact, IEnumerable<string> commands) : this([fact], commands) { }

        public RuleInstruction(IEnumerable<string> facts, IEnumerable<string> commands) : base()
        {
            Facts = facts.ToList();
            Commands = commands.ToList();
        }
    }
}
