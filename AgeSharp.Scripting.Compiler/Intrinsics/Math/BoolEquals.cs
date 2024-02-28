using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language.Expressions;
using AgeSharp.Scripting.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Intrinsics.Math
{
    internal class BoolEquals : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public BoolEquals(Script script) : base(script)
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

            // algorithm by Leif Ericson

            instructions.AddRange(GetArgument(memory, call.Arguments[0], memory.Intr0));
            instructions.AddRange(GetArgument(memory, call.Arguments[1], memory.Intr1));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Intr0} g:- {memory.Intr1}"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Intr0} c:min 1"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Intr0} c:max -1"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Intr0} c:+ 1"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Intr0} c:mod 2"));
            instructions.AddRange(Utils.Assign(memory, memory.Intr0, result));

            return instructions;
        }
    }
}
