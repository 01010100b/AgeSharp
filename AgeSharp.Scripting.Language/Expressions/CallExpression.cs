using AgeSharp.Scripting.Language.Types;
using System.Text;

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
            if (Arguments.Count != Method.Parameters.Count) throw new NotSupportedException($"Call to method {Method.Name} argument count differs from method parameter count.");

            for (int i = 0; i < Arguments.Count; i++)
            {
                var arg = Arguments[i];
                var par = Method.Parameters[i];

                par.Type.ValidateAssignmentFrom(arg.Type);

                if (par.Type is RefType)
                {
                    if (arg is not AccessorExpression)
                    {
                        throw new NotSupportedException($"Call to method {Method.Name} when argument {arg} is not a variable accessor and parameter {par} is ref.");
                    }
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{Method.ShortName}(");

            if (Literal is not null)
            {
                sb.Append($"\"{Literal}\"");

                if (Arguments.Count > 0)
                {
                    sb.Append(", ");
                }
            }

            for (int i = 0; i < Arguments.Count; i++)
            {
                if (Method.Parameters[i].Type is RefType)
                {
                    sb.Append("ref ");
                }

                sb.Append(Arguments[i].ToString());

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
