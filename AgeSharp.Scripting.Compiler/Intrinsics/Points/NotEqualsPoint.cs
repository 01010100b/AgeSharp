using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language.Expressions;
using AgeSharp.Scripting.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Intrinsics.Points
{
    internal class NotEqualsPoint : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public NotEqualsPoint(Script script) : base(script)
        {
            AddParameter(new("a", Point));
            AddParameter(new("b", Point));
            ReturnType = Bool;
        }

        protected override List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call)
        {
            var instructions = new List<Instruction>();

            if (result is null)
            {
                return instructions;
            }

            throw new NotImplementedException();

            return instructions;
        }
    }
}
