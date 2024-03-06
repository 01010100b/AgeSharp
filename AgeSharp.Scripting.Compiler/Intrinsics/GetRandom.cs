using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language.Expressions;
using AgeSharp.Scripting.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgeSharp.Common;

namespace AgeSharp.Scripting.Compiler.Intrinsics
{
    internal class GetRandom : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public GetRandom(Script script) : base(script)
        {
            ReturnType = Int;
            AddParameter(new("max", Int));
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            if (result is null)
            {
                return instructions;
            }

            instructions.AddRange(GetArgument(memory, call.Arguments[0], memory.Intr0));
            instructions.Add(new CommandInstruction($"up-get-precise-time 0 {memory.Intr1}"));

            for (int i = 0; i < 3; i++)
            {
                instructions.Add(new CommandInstruction($"generate-random-number 32767"));
                instructions.Add(new CommandInstruction($"up-get-fact {(int)FactId.RANDOM_NUMBER} 0 {memory.Intr2}"));
                instructions.Add(new CommandInstruction($"up-modify-goal {memory.Intr1} g:+ {memory.Intr2}"));
            }

            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Intr1} g:mod {memory.Intr0}"));
            instructions.AddRange(Utils.Assign(memory, memory.Intr1, result));

            return instructions;
        }
    }
}
