namespace AgeSharp.Scripting.Compiler.Instructions
{
    internal class LabelInstruction : Instruction
    {
        public string Label { get; } = "label-" + Guid.NewGuid().ToString();
    }
}
