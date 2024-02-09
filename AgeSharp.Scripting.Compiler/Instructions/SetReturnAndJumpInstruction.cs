using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Instructions
{
    internal class SetReturnAndJumpInstruction(int goal, string label) : Instruction
    {
        public int Goal { get; } = goal;
        public string Label { get; } = label;
    }
}
