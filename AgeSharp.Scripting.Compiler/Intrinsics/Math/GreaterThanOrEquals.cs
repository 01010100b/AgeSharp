﻿using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;

namespace AgeSharp.Scripting.Compiler.Intrinsics.Math
{
    internal class GreaterThanOrEquals : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public GreaterThanOrEquals(Script script) : base(script)
        {
            AddParameter(new("a", Int));
            AddParameter(new("b", Int));
            ReturnType = Bool;
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            if (result is null)
            {
                return instructions;
            }

            instructions.AddRange(GetArgument(memory, call.Arguments[0], memory.Intr0));
            instructions.AddRange(GetArgument(memory, call.Arguments[1], memory.Intr1));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Intr0} g:- {memory.Intr1}"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Intr0} c:+ 1"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Intr0} c:max 0"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Intr0} c:min 1"));
            instructions.AddRange(Utils.Assign(memory, memory.Intr0, result));

            return instructions;
        }
    }
}
