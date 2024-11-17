using System.Diagnostics;

namespace AgeSharp.Scripting.Compiler.Instructions
{
    internal class JumpFactInstruction : Instruction
    {
        private static readonly string[] COMPARISONS = ["<", "<=", ">", ">=", "==", "!="];

        public string Fact { get; }
        public LabelInstruction Label { get; }

        public JumpFactInstruction(int goal, string comparison, int value, LabelInstruction label) : base()
        {
            Debug.Assert(goal >= 1 && goal <= 512);
            Debug.Assert(COMPARISONS.Contains(comparison));

            Fact = $"up-compare-goal {goal} c:{comparison} {value}";
            Label = label;
        }

        public JumpFactInstruction(string fact, LabelInstruction label) : base()
        {
            Fact = fact;
            Label = label;
        }
    }
}
