using System.Diagnostics;

namespace AgeSharp.Scripting.Compiler.Instructions
{
    internal class JumpIndirectInstruction : Instruction
    {
        public int Goal { get; }

        public JumpIndirectInstruction(int goal) : base()
        {
            Debug.Assert(goal >= 1 && goal <= 512);
            Goal = goal;
        }
    }
}
