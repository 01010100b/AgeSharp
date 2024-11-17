using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;
using AgeSharp.Scripting.Language.Types;
using Type = AgeSharp.Scripting.Language.Type;

namespace AgeSharp.Scripting.Compiler
{
    internal abstract class Intrinsic : Method
    {
        protected static Type Void => PrimitiveType.Void;
        protected static Type Int => PrimitiveType.Int;
        protected static Type Bool => PrimitiveType.Bool;
        protected static Type Point => BuiltinTypes.Point;
        protected static Type SearchState => BuiltinTypes.SearchState;

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

        protected List<Instruction> GetArgument(Memory memory, Expression argument, int goal)
        {
            var instructions = new List<Instruction>();

            if (argument is ConstExpression ce)
            {
                instructions.Add(new CommandInstruction($"up-modify-goal {goal} c:= {ce.Value}"));
            }
            else if (argument is AccessorExpression ae)
            {
                var from = Utils.GetAddress(memory, ae);
                var type = from.Type is RefType rt ? rt.ReferencedType : from.Type;
                var to = new Address(type, goal, false);

                instructions.AddRange(Utils.Assign(memory, from, to));
            }
            else
            {
                throw new NotSupportedException($"Instrinc {Name} argument type {argument.GetType().Name} not supported.");
            }

            return instructions;
        }

        protected int GetConstArgument(Expression argument)
        {
            if (argument is ConstExpression ce)
            {
                return ce.Value;
            }
            else
            {
                throw new NotSupportedException($"Call to method {Name} with argument not const.");
            }
        }
    }
}
