using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;

namespace AgeSharp.Scripting.Compiler.Intrinsics.Math
{
    internal class ShiftRight : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public ShiftRight(Script script) : base(script)
        {
            ReturnType = Int;
            AddParameter(new("a", Int));
            AddParameter(new("count", Int));
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            if (result is null)
            {
                return instructions;
            }

            instructions.AddRange(GetArgument(memory, call.Arguments[0], memory.Intr0));
            instructions.AddRange(GetArgument(memory, call.Arguments[1], memory.Intr1));
            instructions.Add(new RuleInstruction($"up-compare-goal {memory.Intr1} c:> 0",
                [
                    $"up-modify-goal {memory.Intr0} c:z/ 2",
                    $"up-modify-goal {memory.Intr1} c:- 1",
                    $"up-jump-rule -1"
                ]));
            instructions.AddRange(Utils.Assign(memory, memory.Intr0, result));

            return instructions;
        }
    }
}
