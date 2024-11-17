using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;

namespace AgeSharp.Scripting.Compiler.Intrinsics.DUC
{
    internal class TargetObjects : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public TargetObjects(Script script) : base(script)
        {
            AddParameter(new("use_target_object", Bool));
            AddParameter(new("action", Int));
            AddParameter(new("formation", Int));
            AddParameter(new("stance", Int));
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            var option = GetConstArgument(call.Arguments[0]);
            var action = GetConstArgument(call.Arguments[1]);
            var formation = GetConstArgument(call.Arguments[2]);
            var stance = GetConstArgument(call.Arguments[3]);

            instructions.Add(new CommandInstruction($"up-target-objects {option} {action} {formation} {stance}"));

            return instructions;
        }
    }
}
