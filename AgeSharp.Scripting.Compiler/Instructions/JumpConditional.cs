using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Instructions
{
    internal class JumpConditional(int goal, string comparison, int value, LabelInstruction label) : Instruction
    {
        private static readonly string[] COMPARISONS = ["<", "<=", ">", ">=", "==", "!="];

        public int Goal { get; } = goal;
        public string Comparison { get; } = comparison;
        public int Value { get; } = value;  
        public LabelInstruction Label { get; } = label;

        public override void Validate()
        {
            Debug.Assert(Goal >= 1 && Goal <= 512);
            Debug.Assert(COMPARISONS.Contains(Comparison));
        }
    }
}
