using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Instructions
{
    internal class JumpIndirectInstruction(int goal) : Instruction
    {
        public int Goal { get; } = goal;

        public override void Validate()
        {
            Debug.Assert(Goal >= 1 && Goal <= 512);
        }
    }
}
