using AgeSharp.Common;
using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;

namespace AgeSharp.Scripting.Compiler.Intrinsics.Players
{
    internal class GetPlayerStance : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public GetPlayerStance(Script script) : base(script)
        {
            AddParameter(new("player", Int));
            ReturnType = Int;
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            if (result is null)
            {
                return instructions;
            }

            instructions.AddRange(GetArgument(memory, call.Arguments[0], memory.Intr0));
            instructions.Add(new CommandInstruction($"up-modify-sn {(int)StrategicNumber.FOCUS_PLAYER_NUMBER} g:= {memory.Intr0}"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Intr0} c:= {(int)Enum.GetValues<PlayerStance>().First()}"));

            foreach (var stance in Enum.GetValues<PlayerStance>().Skip(1))
            {
                instructions.Add(new RuleInstruction($"players-stance focus-player {(int)stance}", $"up-modify-goal {memory.Intr0} c:= {(int)stance}"));
            }

            instructions.AddRange(Utils.Assign(memory, memory.Intr0, result));

            return instructions;
        }
    }
}
