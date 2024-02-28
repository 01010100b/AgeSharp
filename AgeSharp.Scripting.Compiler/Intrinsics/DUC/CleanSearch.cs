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
    internal class CleanSearch : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public CleanSearch(Script script) : base(script)
        {
            AddParameter(new("search_source", Int));
            AddParameter(new("object_data", Int));
            AddParameter(new("search_order", Int));
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            var search_source = GetConstArgument(call.Arguments[0]);
            var object_data = GetConstArgument(call.Arguments[1]);
            var search_order = GetConstArgument(call.Arguments[2]);
            instructions.Add(new CommandInstruction($"up-clean-search {search_source} {object_data} {search_order}"));

            return instructions;
        }
    }
}
