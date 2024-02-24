using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language.Expressions;
using AgeSharp.Scripting.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Intrinsics.DUC
{
    internal class FindLocal : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public FindLocal(Script script) : base(script)
        {
            AddParameter(new("id", Int));
            AddParameter(new("count", Int));
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            instructions.AddRange(GetArgument(memory, call.Arguments[0], memory.Intr0));
            instructions.AddRange(GetArgument(memory, call.Arguments[1], memory.Intr1));
            instructions.Add(new CommandInstruction($"up-find-local g: {memory.Intr0} g: {memory.Intr1}"));

            return instructions;
        }
    }
}
