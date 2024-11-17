using AgeSharp.Common;
using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;

namespace AgeSharp.Scripting.Compiler.Intrinsics.Points
{
    internal class GetPointExplored : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public GetPointExplored(Script script) : base(script)
        {
            AddParameter(new("point", Point));
            ReturnType = Int;
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            if (result is null)
            {
                return instructions;
            }

            instructions.AddRange(GetArgument(memory, call.Arguments[0], memory.Intr0));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Intr2} c:= {(int)Enum.GetValues<ExploredState>().First()}"));

            foreach (var state in Enum.GetValues<ExploredState>().Skip(1))
            {
                instructions.Add(new RuleInstruction($"up-point-explored {memory.Intr0} g: {(int)state}",
                    $"up-modify-goal {memory.Intr2} c:= {(int)state}"));
            }

            instructions.AddRange(Utils.Assign(memory, memory.Intr2, result));

            return instructions;
        }
    }
}
