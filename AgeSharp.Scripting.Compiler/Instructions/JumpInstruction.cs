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
