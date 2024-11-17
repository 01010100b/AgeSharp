using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;

namespace AgeSharp.Scripting.Compiler.Intrinsics.Math
{
    internal class BitwiseXor : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public BitwiseXor(Script script) : base(script)
        {
            AddParameter(new("a", Int));
            AddParameter(new("b", Int));
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
            instructions.AddRange(GetArgument(memory, call.Arguments[1], memory.Intr1));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Intr2} g:= {memory.Intr0}"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Intr3} c:= -1"));
            instructions.Add(new CommandInstruction($"up-modify-flag {memory.Intr3} g:- {memory.Intr1}"));
            instructions.Add(new CommandInstruction($"up-modify-flag {memory.Intr2} g:- {memory.Intr3}")); // i2 = a & b
            instructions.Add(new CommandInstruction($"up-modify-flag {memory.Intr0} g:+ {memory.Intr1}")); // i0 = a | b
            instructions.Add(new CommandInstruction($"up-modify-flag {memory.Intr0} g:- {memory.Intr2}")); // i0 = i0 & ~i2
            instructions.AddRange(Utils.Assign(memory, memory.Intr0, result));

            return instructions;
        }
    }
}
