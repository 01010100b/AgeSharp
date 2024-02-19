using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Intrinsics.Math
{
    internal class Equals : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public Equals(Script script) : base(script)
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

            instructions.AddRange(GetArgument(memory, call.Arguments[0], Int, memory.Intr0));
            instructions.AddRange(GetArgument(memory, call.Arguments[1], Int, memory.Intr1));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Intr2} c:= 0"));
            instructions.Add(new CommandFactInstruction($"up-compare-goal {memory.Intr0} g:== {memory.Intr1}", $"up-modify-goal {memory.Intr2} c:= 1"));
            instructions.AddRange(Utils.MemCpy(memory, memory.Intr2, result, Bool.Size));

            return instructions;
        }
    }
}
