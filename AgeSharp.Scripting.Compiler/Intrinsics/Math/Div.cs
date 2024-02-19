﻿using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language.Expressions;
using AgeSharp.Scripting.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Intrinsics.Math
{
    internal class Div : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public Div(Script script) : base(script)
        {
            AddParameter(new("a", Int));
            AddParameter(new("b", Int));
            ReturnType = Int;
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            if (result is null)
            {
                return instructions;
            }

            instructions.AddRange(GetArgument(memory, call.Arguments[0], Int, memory.Intr0, false));
            instructions.AddRange(GetArgument(memory, call.Arguments[1], Int, memory.Intr1, false));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Intr0} g:z/ {memory.Intr1}"));
            instructions.AddRange(Utils.MemCpy(memory, memory.Intr0, result, Int.Size));

            return instructions;
        }
    }
}
