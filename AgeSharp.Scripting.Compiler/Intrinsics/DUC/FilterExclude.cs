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
    internal class FilterExclude : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public FilterExclude(Script script) : base(script)
        {
            AddParameter(new("cmdid", Int));
            AddParameter(new("action", Int));
            AddParameter(new("order", Int));
            AddParameter(new("class", Int));
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            var cmdid = GetConstArgument(call.Arguments[0]);
            var action = GetConstArgument(call.Arguments[1]);
            var order = GetConstArgument(call.Arguments[2]);
            var class_id = GetConstArgument(call.Arguments[3]);
            instructions.Add(new CommandInstruction($"up-filter-include {cmdid} {action} {order} {class_id}"));

            return instructions;
        }
    }
}
