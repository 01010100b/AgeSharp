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
    internal class CreateGroup : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public CreateGroup(Script script) : base(script)
        {
            AddParameter(new("index", Int));
            AddParameter(new("count", Int));
            AddParameter(new("group", Int));
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            instructions.AddRange(GetArgument(memory, call.Arguments[0], memory.Intr0));
            instructions.AddRange(GetArgument(memory, call.Arguments[1], memory.Intr1));
            instructions.AddRange(GetArgument(memory, call.Arguments[2], memory.Intr2));
            instructions.Add(new CommandInstruction($"up-create-group {memory.Intr0} {memory.Intr1} g: {memory.Intr2}"));

            return instructions;
        }
    }
}
