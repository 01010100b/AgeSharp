using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Instructions
{
    internal class CommandInstruction : Instruction
    {
        public string Command { get; }

        public CommandInstruction(string command) : base()
        {
            Command = command;
        }
    }
}
