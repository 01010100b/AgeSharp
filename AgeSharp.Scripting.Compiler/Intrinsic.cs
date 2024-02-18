using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;
using AgeSharp.Scripting.Language.Types;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Type = AgeSharp.Scripting.Language.Type;

namespace AgeSharp.Scripting.Compiler
{
    internal abstract class Intrinsic : Method
    {
        public static Type Void => PrimitiveType.Void;
        public static Type Int => PrimitiveType.Int;
        public static Type Bool => PrimitiveType.Bool;

        public abstract bool HasStringLiteral { get; }

        public Intrinsic(Script script) : base(script)
        {
            Name = GetType().Name;
        }

        public List<Instruction> Compile(Memory memory, Address? result, CallExpression call)
        {
            if (call.Literal is null == HasStringLiteral) throw new NotSupportedException($"Intrinsic {Name} incorrect string literal call.");

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

        protected List<Instruction> GetArgument(Memory memory, Expression argument, int goal, bool req_const)
        {
            var instructions = new List<Instruction>();

            if (argument is ConstExpression ce)
            {
                instructions.Add(new CommandInstruction($"up-modify-goal {goal} c:= {ce.Value}"));
            }
            else if (argument is AccessorExpression ae)
            {
                if (req_const) throw new NotSupportedException($"Argument {argument} for intrinsic {Name} is not const.");
                if (!ae.IsVariableAccess) throw new NotSupportedException($"Argument {argument} for intrinsic {Name} is not variable access.");

                var from = memory.GetAddress(ae.Variable);
                var to = new Address(goal, false);

                instructions.AddRange(Utils.MemCpy(memory, from, to, ae.Type.Size));
            }
            else
            {
                throw new NotSupportedException($"Instrinc {Name} argument type {argument.GetType().Name} not supported.");
            }

            return instructions;
        }
    }
}
