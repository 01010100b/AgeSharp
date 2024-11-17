using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;

namespace AgeSharp.Scripting.Compiler.Intrinsics.DUC
{
    internal class CreateGroup : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public CreateGroup(Script script) : base(script)
        {
            AddParameter(new("index", Int));
            AddParameter(new("count", Int));
            AddParameter(new("group", Int));
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            var index = GetConstArgument(call.Arguments[0]);
            var count = GetConstArgument(call.Arguments[1]);
            instructions.AddRange(GetArgument(memory, call.Arguments[2], memory.Intr2));
            instructions.Add(new CommandInstruction($"up-create-group {index} {count} g: {memory.Intr2}"));

            return instructions;
        }
    }
}
