using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;

namespace AgeSharp.Scripting.Compiler.Intrinsics.DUC
{
    internal class GetObjectData : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public GetObjectData(Script script) : base(script)
        {
            ReturnType = Int;
            AddParameter(new("data", Int));
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            if (result is null)
            {
                return instructions;
            }

            var data = GetConstArgument(call.Arguments[0]);
            instructions.Add(new CommandInstruction($"up-get-object-data {data} {memory.Intr0}"));
            instructions.AddRange(Utils.Assign(memory, memory.Intr0, result));

            return instructions;
        }
    }
}
