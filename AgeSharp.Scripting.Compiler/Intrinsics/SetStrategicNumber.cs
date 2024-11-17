using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;

namespace AgeSharp.Scripting.Compiler.Intrinsics
{
    internal class SetStrategicNumber : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public SetStrategicNumber(Script script) : base(script)
        {
            AddParameter(new("sn", Int));
            AddParameter(new("value", Int));
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            var sn = GetConstArgument(call.Arguments[0]);
            instructions.AddRange(GetArgument(memory, call.Arguments[1], memory.Intr0));
            instructions.Add(new CommandInstruction($"up-modify-sn {sn} g:= {memory.Intr0}"));

            return instructions;
        }
    }
}
