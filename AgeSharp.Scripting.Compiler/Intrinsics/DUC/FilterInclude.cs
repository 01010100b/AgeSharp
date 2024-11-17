using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;

namespace AgeSharp.Scripting.Compiler.Intrinsics.DUC
{
    internal class FilterInclude : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public FilterInclude(Script script) : base(script)
        {
            AddParameter(new("cmdid", Int));
            AddParameter(new("action", Int));
            AddParameter(new("order", Int));
            AddParameter(new("mainland", Int));
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            var cmdid = GetConstArgument(call.Arguments[0]);
            var action = GetConstArgument(call.Arguments[1]);
            var order = GetConstArgument(call.Arguments[2]);
            var mainland = GetConstArgument(call.Arguments[3]);
            instructions.Add(new CommandInstruction($"up-filter-include {cmdid} {action} {order} {mainland}"));

            return instructions;
        }
    }
}
