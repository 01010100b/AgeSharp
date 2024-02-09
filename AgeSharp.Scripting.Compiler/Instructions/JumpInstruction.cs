using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Instructions
{
    internal class JumpInstruction(string label) : Instruction
    {
        public string Label { get; } = label;
    }
}
