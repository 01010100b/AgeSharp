using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Instructions
{
    internal class CommandInstruction(string command) : Instruction
    {
        public string Command { get; } = command;

        public override void Validate()
        {
        }
    }
}
