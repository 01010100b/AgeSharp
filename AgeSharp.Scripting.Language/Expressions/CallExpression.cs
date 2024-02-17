using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language.Expressions
{
    public class CallExpression(Method method, string? literal = null) : Expression
    {
        public Method Method { get; } = method;
        public override Type Type => Method.ReturnType;
        public string? Literal { get; } = literal;
        public IReadOnlyList<Expression> Arguments { get; } = new List<Expression>();

        public void AddArgument(Expression argument)
        {
            ((List<Expression>)Arguments).Add(argument);
        }

        public override IEnumerable<Variable> GetReferencedVariables()
        {
            foreach (var arg in Arguments)
            {
                foreach (var variable in arg.GetReferencedVariables())
                {
                    yield return variable;
                }
            }
        }

        public override void Validate()
        {
            if (Arguments.Count != Method.Parameters.Count) throw new NotSupportedException($"Call argument count differs from method parameter count.");

            for (int i = 0; i < Arguments.Count; i++)
            {
                var arg = Arguments[i];
                var par = Method.Parameters[i];

                arg.Type.ValidateAssignment(par.Type, par.IsRef);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{Method.ShortName}(");

            for (int i = 0; i < Arguments.Count; i++)
            {
                var argument = Arguments[i];
                sb.Append(argument.ToString());

                if (i < Arguments.Count - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append(")");

            return sb.ToString();
        }
    }
}
