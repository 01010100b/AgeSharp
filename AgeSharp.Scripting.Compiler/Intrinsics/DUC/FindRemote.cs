using AgeSharp.Common;
using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;

namespace AgeSharp.Scripting.Compiler.Intrinsics.DUC
{
    internal class FindRemote : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public FindRemote(Script script) : base(script)
        {
            AddParameter(new("player", Int));
            AddParameter(new("id", Int));
            AddParameter(new("count", Int));
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            instructions.AddRange(GetArgument(memory, call.Arguments[0], memory.Intr0));
            instructions.AddRange(GetArgument(memory, call.Arguments[1], memory.Intr1));
            instructions.AddRange(GetArgument(memory, call.Arguments[2], memory.Intr2));
            instructions.Add(new CommandInstruction($"up-modify-sn {(int)StrategicNumber.FOCUS_PLAYER_NUMBER} g:= {memory.Intr0}"));
            instructions.Add(new CommandInstruction($"up-find-remote g: {memory.Intr1} g: {memory.Intr2}"));

            return instructions;
        }
    }
}
