using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Instructions
{
    internal class JumpInstruction : Instruction
    {
        public LabelInstruction Label { get; }

        public JumpInstruction(LabelInstruction label) : base()
        {
            Label = label;
        }
    }
}
