using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language.Expressions;
using AgeSharp.Scripting.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgeSharp.Common;

namespace AgeSharp.Scripting.Compiler.Intrinsics.DUC
{
    internal class ObjectExists : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public ObjectExists(Script script) : base(script)
        {
            ReturnType = Bool;
            AddParameter(new("id", Int));
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            if (result is null)
            {
                return instructions;
            }

            instructions.AddRange(GetArgument(memory, call.Arguments[0], memory.Intr0));
            instructions.Add(new CommandInstruction($"up-set-target-by-id g: {memory.Intr0}"));
            instructions.Add(new CommandInstruction($"up-get-object-data {(int)ObjectData.ID} {memory.Intr1}"));
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
