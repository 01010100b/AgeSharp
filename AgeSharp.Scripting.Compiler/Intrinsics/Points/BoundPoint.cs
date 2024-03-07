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
    internal class BoundPoint : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public BoundPoint(Script script) : base(script)
        {
            AddParameter(new("point", Point));
            AddParameter(new("precise", Bool));
            AddParameter(new("border", Int));
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
            instructions.AddRange(GetArgument(memory, call.Arguments[1], memory.Intr2));
            instructions.AddRange(GetArgument(memory, call.Arguments[2], memory.Intr3));
            instructions.Add(new CommandInstruction($"up-bound-precise-point {memory.Intr0} {memory.Intr2} g: {memory.Intr3}"));
            instructions.AddRange(Utils.Assign(memory, memory.Intr0, result));

            return instructions;
        }
    }
}
