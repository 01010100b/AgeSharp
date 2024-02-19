using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Intrinsics
{
    internal class ChatDataToSelf : Intrinsic
    {
        public override bool HasStringLiteral => true;

        public ChatDataToSelf(Script script) : base(script)
        {
            AddParameter(new("data", Int));
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();
            instructions.AddRange(GetArgument(memory, call.Arguments[0], Int, memory.Intr0, false));
            instructions.Add(new CommandInstruction($"up-chat-data-to-self \"{call.Literal}\" g: {memory.Intr0}"));

            return instructions;
        }
    }
}
