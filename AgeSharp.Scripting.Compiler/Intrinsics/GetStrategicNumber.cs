using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;

namespace AgeSharp.Scripting.Compiler.Intrinsics
{
    internal class GetStrategicNumber : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public GetStrategicNumber(Script script) : base(script)
        {
            ReturnType = Int;
            AddParameter(new("sn", Int));
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            if (result is null)
            {
                return instructions;
            }

            var sn = GetConstArgument(call.Arguments[0]);
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Intr0} s:= {sn}"));
            instructions.AddRange(Utils.Assign(memory, memory.Intr0, result));

            return instructions;
        }
    }
}
