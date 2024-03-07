using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language.Expressions;
using AgeSharp.Scripting.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Intrinsics.Points
{
    internal class GetPoint : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public GetPoint(Script script) : base(script)
        {
            AddParameter(new("position_type", Int));
            ReturnType = Point;
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            if (result is null)
            {
                return instructions;
            }

            var position_type = GetConstArgument(call.Arguments[0]);
            instructions.Add(new CommandInstruction($"up-get-point {position_type} {memory.Intr0}"));
            instructions.AddRange(Utils.Assign(memory, memory.Intr0, result));

            return instructions;
        }
    }
}
