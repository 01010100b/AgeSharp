using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;

namespace AgeSharp.Scripting.Compiler.Intrinsics.Points
{
    internal class SendScout : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public SendScout(Script script) : base(script)
        {
            AddParameter(new("group_type", Int));
            AddParameter(new("scout_method", Int));
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            var group_type = GetConstArgument(call.Arguments[0]);
            var scout_method = GetConstArgument(call.Arguments[1]);
            instructions.Add(new CommandInstruction($"up-send-scout {group_type} {scout_method}"));

            return instructions;
        }
    }
}
