using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;

namespace AgeSharp.Scripting.Compiler.Intrinsics.DUC
{
    internal class ModifyGroupFlag : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public ModifyGroupFlag(Script script) : base(script)
        {
            AddParameter(new("append", Bool));
            AddParameter(new("group", Int));
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            var append = GetConstArgument(call.Arguments[0]);
            instructions.AddRange(GetArgument(memory, call.Arguments[1], memory.Intr0));
            instructions.Add(new CommandInstruction($"up-modify-group-flag {append} g: {memory.Intr0}"));

            return instructions;
        }
    }
}
