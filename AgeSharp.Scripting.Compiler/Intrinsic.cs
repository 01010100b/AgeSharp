using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler
{
    internal abstract class Intrinsic(Script script) : Method(script)
    {
        public bool HasStringLiteral { get; } = false;

        public List<Instruction> Compile(Memory memory, Address? result, CallExpression call)
        {
            return CompileCall(memory, result, call);
        }

        public override void Validate()
        {
            ValidateName(Name);
            ReturnType.Validate();

            foreach (var parameter in Parameters)
            {
                parameter.Validate();
            }
        }

        protected abstract List<Instruction> CompileCall(Memory memory, Address? result, CallExpression call);
    }
}
