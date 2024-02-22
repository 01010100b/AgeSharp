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

            Throw.If<NotSupportedException>(call.Arguments[0] is not ConstExpression, $"Method {Name} call with first argument not const.");
            var ce = (ConstExpression)call.Arguments[0];
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Intr0} s:= {ce.Value}"));
            instructions.AddRange(Utils.Assign(memory, memory.Intr0, result));

            return instructions;
        }
    }
}
