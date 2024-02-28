using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language.Expressions;
using AgeSharp.Scripting.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgeSharp.Common;

namespace AgeSharp.Scripting.Compiler.Intrinsics.DUC
{
    internal class TargetPoint : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public TargetPoint(Script script) : base(script)
        {
            AddParameter(new("point", Point));
            AddParameter(new("point_adjustment", Int));
            AddParameter(new("action", Int));
            AddParameter(new("formation", Int));
            AddParameter(new("stance", Int));
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            instructions.AddRange(GetArgument(memory, call.Arguments[0], memory.Intr0));
            instructions.AddRange(GetArgument(memory, call.Arguments[1], memory.Intr2));
            var action = GetConstArgument(call.Arguments[1]);
            var formation = GetConstArgument(call.Arguments[2]);
            var stance = GetConstArgument(call.Arguments[3]);

            instructions.Add(new CommandInstruction($"up-modify-sn {(int)StrategicNumber.TARGET_POINT_ADJUSTMENT} g:= {memory.Intr2}"));
            instructions.Add(new CommandInstruction($"up-target-point {memory.Intr0} {action} {formation} {stance}"));

            return instructions;
        }
    }
}
