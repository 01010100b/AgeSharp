using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;

namespace AgeSharp.Scripting.Compiler.Intrinsics.DUC
{
    internal class AddObjectById : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public AddObjectById(Script script) : base(script)
        {
            AddParameter(new("search_source", Int));
            AddParameter(new("id", Int));
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            var search_source = GetConstArgument(call.Arguments[0]);
            instructions.AddRange(GetArgument(memory, call.Arguments[1], memory.Intr0));
            instructions.Add(new CommandInstruction($"up-add-object-by-id {search_source} g: {memory.Intr0}"));

            return instructions;
        }
    }
}
