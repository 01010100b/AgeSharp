using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language.Expressions;
using AgeSharp.Scripting.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgeSharp.Common;

namespace AgeSharp.Scripting.Compiler.Intrinsics.Points
{
    internal class FindPlayerFlare : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public FindPlayerFlare(Script script) : base(script)
        {
            AddParameter(new("player", Int));
            ReturnType = Point;
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
            instructions.Add(new CommandInstruction($"up-find-player-flare focus-player {memory.Intr1}"));
            instructions.AddRange(Utils.Assign(memory, memory.Intr1, result));

            return instructions;
        }
    }
}
