using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;

namespace AgeSharp.Scripting.Compiler.Intrinsics
{
    internal class XsScriptCall : Intrinsic
    {
        public override bool HasStringLiteral => true;

        public XsScriptCall(Script script) : base(script)
        {
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>()
            {
                new CommandInstruction($"xs-script-call \"{call.Literal}\"")
            };

            return instructions;
        }
    }
}
