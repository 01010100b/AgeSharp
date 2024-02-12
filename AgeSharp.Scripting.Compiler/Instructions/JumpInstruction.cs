using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Instructions
{
    internal class JumpInstruction(LabelInstruction label) : Instruction
    {
        public LabelInstruction Label { get; } = label;

        public override void Validate()
        {
        }
    }
}
