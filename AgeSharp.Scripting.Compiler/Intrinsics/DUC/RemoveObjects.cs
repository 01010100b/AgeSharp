using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;

namespace AgeSharp.Scripting.Compiler.Intrinsics.DUC
{
    internal class RemoveObjects : Intrinsic
    {
        public override bool HasStringLiteral => true;

        public RemoveObjects(Script script) : base(script)
        {
            AddParameter(new("search_source", Int));
            AddParameter(new("data", Int));
            AddParameter(new("value", Int));
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            var search_source = GetConstArgument(call.Arguments[0]);
            var data = GetConstArgument(call.Arguments[1]);
            instructions.AddRange(GetArgument(memory, call.Arguments[2], memory.Intr0));
            instructions.Add(new CommandInstruction($"up-remove-objects {search_source} {data} g:{call.Literal} {memory.Intr0}"));

            return instructions;
        }
    }
}
