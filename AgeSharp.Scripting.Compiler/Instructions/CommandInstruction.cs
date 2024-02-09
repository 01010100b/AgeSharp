using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Instructions
{
    internal class CommandInstruction(string command, bool disable = false) : Instruction
    {
        public string Command { get; } = command;
        public bool Disable { get; } = disable;
    }
}
