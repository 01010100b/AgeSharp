using AgeSharp.Common;
using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            Throw.If<NotSupportedException>(call.Arguments[0] is not ConstExpression, $"Method {Name} call with first argument not const.");
            var ce = (ConstExpression)call.Arguments[0];

            instructions.AddRange(GetArgument(memory, call.Arguments[1], memory.Intr0));
            instructions.Add(new CommandInstruction($"up-modify-sn {ce.Value} g:= {memory.Intr0}"));

            return instructions;
        }
    }
}
