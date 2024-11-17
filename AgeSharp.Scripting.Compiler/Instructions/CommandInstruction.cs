namespace AgeSharp.Scripting.Compiler.Instructions
{
    internal class CommandInstruction : Instruction
    {
        public string Command { get; }

        public CommandInstruction(string command) : base()
        {
            Command = command;
        }
    }
}
