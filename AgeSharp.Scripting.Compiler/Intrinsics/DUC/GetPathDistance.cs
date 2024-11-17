using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;

namespace AgeSharp.Scripting.Compiler.Intrinsics.DUC
{
    internal class GetPathDistance : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public GetPathDistance(Script script) : base(script)
        {
            AddParameter(new("point", Point));
            AddParameter(new("strict", Bool));
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
            var strict = GetConstArgument(call.Arguments[1]);
            instructions.Add(new CommandInstruction($"up-get-path-distance {memory.Intr0} {strict} {memory.Intr2}"));
            instructions.AddRange(Utils.Assign(memory, memory.Intr2, result));

            return instructions;
        }
    }
}
