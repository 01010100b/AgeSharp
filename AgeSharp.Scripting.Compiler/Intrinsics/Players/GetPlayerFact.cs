using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language.Expressions;
using AgeSharp.Scripting.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgeSharp.Common;

namespace AgeSharp.Scripting.Compiler.Intrinsics.Players
{
    internal class GetPlayerFact : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public GetPlayerFact(Script script) : base(script)
        {
            AddParameter(new("player", Int));
            AddParameter(new("fact_id", Int));
            AddParameter(new("fact_parameter", Int));
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
            var fact_id = GetConstArgument(call.Arguments[1]);
            var fact_parameter = GetConstArgument(call.Arguments[2]);
            instructions.Add(new CommandInstruction($"up-modify-sn {(int)StrategicNumber.FOCUS_PLAYER_NUMBER} g:= {memory.Intr0}"));

            if (fact_id == (int)FactId.PLAYER_IN_GAME)
            {
                // DE bug workaround
                instructions.Add(new CommandInstruction($"up-modify-goal {memory.Intr1} c:= 0"));
                instructions.Add(new RuleInstruction("player-in-game focus-player", $"up-modify-goal {memory.Intr1} c:= 1"));
            }
            else
            {
                instructions.Add(new CommandInstruction($"up-get-focus-fact {fact_id} {fact_parameter} {memory.Intr1}"));
            }

            instructions.AddRange(Utils.Assign(memory, memory.Intr1, result));

            return instructions;
        }
    }
}
