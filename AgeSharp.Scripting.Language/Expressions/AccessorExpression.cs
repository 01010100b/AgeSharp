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
        public override Type? Type => GetExpressionType();

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
            Variable = variable;
            Fields = fields.ToList();
        }

        public override void Validate()
        {
            throw new NotImplementedException();
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
