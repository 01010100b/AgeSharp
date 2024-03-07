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
    internal class GetPointZone : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public GetPointZone(Script script) : base(script)
        {
            AddParameter(new("point", Point));
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
            instructions.Add(new CommandInstruction($"up-get-point-zone {memory.Intr0} {memory.Intr2}"));
            instructions.AddRange(Utils.Assign(memory, memory.Intr2, result));

            return instructions;
        }
    }
}
