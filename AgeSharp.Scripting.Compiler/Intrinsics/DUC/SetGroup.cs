using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;

namespace AgeSharp.Scripting.Compiler.Intrinsics.DUC
{
    internal class SetGroup : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public SetGroup(Script script) : base(script)
        {
            AddParameter(new("search_source", Int));
            AddParameter(new("group", Int));
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            var search_source = GetConstArgument(call.Arguments[0]);
            instructions.AddRange(GetArgument(memory, call.Arguments[0], memory.Intr0));
            instructions.Add(new CommandInstruction($"up-set-group {search_source} g: {memory.Intr0}"));

            return instructions;
        }
    }
}
