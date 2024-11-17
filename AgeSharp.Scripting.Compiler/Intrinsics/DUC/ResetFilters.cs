using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;

namespace AgeSharp.Scripting.Compiler.Intrinsics.DUC
{
    internal class ResetFilters : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public ResetFilters(Script script) : base(script)
        {
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>()
            {
                new CommandInstruction("up-reset-filters")
            };

            return instructions;
        }
    }
}
