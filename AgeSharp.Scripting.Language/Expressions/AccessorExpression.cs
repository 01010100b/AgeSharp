using AgeSharp.Scripting.Language.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language.Expressions
{
    public class AccessorExpression : Expression
    {
        public Variable Variable { get; }
        public Expression? Index { get; }
        public IReadOnlyList<Field>? Fields { get; }
        public override Type Type => GetExpressionType();

        public AccessorExpression(Variable variable) : base()
        {
            Variable = variable;
        }

        public AccessorExpression(Variable variable, Expression index)
        {
            if (variable.Type is not ArrayType) throw new Exception($"Indexed access when {variable.Name} is not array.");
            if (index.Type != PrimitiveType.Int) throw new Exception($"Indexed access to {variable.Name} when index is not int.");

            Variable = variable;
            Index = index;
        }

        public AccessorExpression(Variable variable, IEnumerable<Field> fields)
        {
            if (variable.Type is not CompoundType) throw new NotSupportedException($"Field access when {variable.Name} is not compound type.");

            Variable = variable;
            Fields = fields.ToList();
        }

        public override IEnumerable<Variable> GetReferencedVariables()
        {
            yield return Variable;

            if (Index is not null)
            {
                foreach (var variable in Index.GetReferencedVariables())
                {
                    yield return variable;
                }
            }
        }

        public override void Validate()
        {
            if (Index is not null)
            {
                Index.Validate();
                if (Index.Type != PrimitiveType.Int) throw new NotSupportedException($"Accessor index does not have type Int.");
            }
            else if (Fields is not null)
            {
                if (Fields.Count == 0) throw new NotSupportedException($"Accessor has no fields.");
            }
        }

        private Type GetExpressionType()
        {
            if (Index is null && Fields is null)
            {
                return Variable.Type;
            }
            else if (Fields is null)
            {
                return ((ArrayType)Variable.Type).ElementType;
            }
            else
            {
                return Fields[^-1].Type;
            }
        }
    }
}
