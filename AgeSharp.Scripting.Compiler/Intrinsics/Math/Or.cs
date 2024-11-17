﻿using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;

namespace AgeSharp.Scripting.Compiler.Intrinsics.Math
{
    internal class Or : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public Or(Script script) : base(script)
        {
            AddParameter(new("a", Bool));
            AddParameter(new("b", Bool));
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
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Intr0} g:max {memory.Intr1}"));
            instructions.AddRange(Utils.Assign(memory, memory.Intr0, result));

            return instructions;
        }
    }
}
