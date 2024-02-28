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
    internal class ResetSearch : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public ResetSearch(Script script) : base(script)
        {
            AddParameter(new("local_index", Bool));
            AddParameter(new("local_list", Bool));
            AddParameter(new("remote_index", Bool));
            AddParameter(new("remote_list", Bool));
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            var local_index = GetConstArgument(call.Arguments[0]);
            var local_list = GetConstArgument(call.Arguments[1]);
            var remote_index = GetConstArgument(call.Arguments[2]);
            var remote_list = GetConstArgument(call.Arguments[3]);

            instructions.Add(new CommandInstruction($"up-reset-search {local_index} {local_list} {remote_index} {remote_list}"));

            return instructions;
        }
    }
}
