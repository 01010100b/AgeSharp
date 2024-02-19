using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Instructions
{
    internal class CommandFactInstruction : Instruction
    {
        public string Fact { get; }
        public string Command { get; }

        public CommandFactInstruction(string fact, string command) : base()
        {
            Fact = fact;
            Command = command;
        }
    }
}
