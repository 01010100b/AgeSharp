namespace AgeSharp.Scripting.Compiler.Instructions
{
    internal class RuleInstruction : Instruction
    {
        public IReadOnlyList<string> Facts { get; }
        public IReadOnlyList<string> Actions { get; }

        public RuleInstruction(string fact, string action) : this([fact], [action]) { }

        public RuleInstruction(string fact, IEnumerable<string> actions) : this([fact], actions) { }

        public RuleInstruction(IEnumerable<string> facts, IEnumerable<string> actions) : base()
        {
            Facts = facts.ToList();
            Actions = actions.ToList();
        }
    }
}
