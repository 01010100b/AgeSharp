using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;

namespace AgeSharp.Scripting.Compiler.Intrinsics.DUC
{
    internal class FilterDistance : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public FilterDistance(Script script) : base(script)
        {
            AddParameter(new("min", Int));
            AddParameter(new("max", Int));
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            instructions.AddRange(GetArgument(memory, call.Arguments[0], memory.Intr0));
            instructions.AddRange(GetArgument(memory, call.Arguments[1], memory.Intr1));
            instructions.Add(new CommandInstruction($"up-filter-distance g: {memory.Intr0} g: {memory.Intr1}"));

            return instructions;
        }
    }
}
