using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;

namespace AgeSharp.Scripting.Compiler.Intrinsics.Points
{
    internal class SendFlare : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public SendFlare(Script script) : base(script)
        {
            AddParameter(new("point", Point));
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            instructions.AddRange(GetArgument(memory, call.Arguments[0], memory.Intr0));
            instructions.Add(new CommandInstruction($"up-send-flare {memory.Intr0}"));

            return instructions;
        }
    }
}
