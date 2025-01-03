﻿using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;

namespace AgeSharp.Scripting.Compiler.Intrinsics.Points
{
    internal class GetPointContains : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public GetPointContains(Script script) : base(script)
        {
            AddParameter(new("point", Point));
            AddParameter(new("object_id", Int));
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
            instructions.AddRange(GetArgument(memory, call.Arguments[1], memory.Intr2));
            instructions.Add(new CommandInstruction($"up-get-point-contains {memory.Intr0} {memory.Intr3} g: {memory.Intr2}"));
            instructions.AddRange(Utils.Assign(memory, memory.Intr3, result));

            return instructions;
        }
    }
}
