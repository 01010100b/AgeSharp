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
    internal class FullResetSearch : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public FullResetSearch(Script script) : base(script)
        {
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>()
            {
                new CommandInstruction("up-full-reset-search")
            };

            return instructions;
        }
    }
}
