using AgeSharp.Common;
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

        public bool IsVariableAccess => !IsArrayAccess && !IsStructAccess;
        public bool IsArrayAccess => Index is not null;
        public bool IsStructAccess => Fields is not null;

        public AccessorExpression(Variable variable) : base()
        {
            Variable = variable;
        }

        public AccessorExpression(Variable variable, Expression index)
        {
            if (variable.Type.ProperType is not ArrayType) throw new Exception($"Indexed access when {variable.Name} is not array.");
            if (index.Type != PrimitiveType.Int) throw new Exception($"Indexed access to {variable.Name} when index is not int.");

            Variable = variable;
            Index = index;
        }

        public AccessorExpression(Variable variable, IEnumerable<Field> fields)
        {
            Throw.If<NotSupportedException>(variable.Type.ProperType is not CompoundType, $"Field access when {variable.Name} is not compound type.");

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

                var type = Variable.Type.ProperType;
                var field = Fields[0];
                var field_type = field.Type;
            }
        }

        public override string ToString()
        {
            if (IsVariableAccess)
            {
                return Variable.Name;
            }
            else if (IsArrayAccess)
            {
                return $"{Variable.Name}[{Index}]";
            }
            else
            {
                var sb = new StringBuilder();
                sb.Append(Variable.Name);

                foreach (var field in Fields!)
                {
                    sb.Append($".{field.Name}");
                }

                return sb.ToString();
            }
        }

        private Type GetExpressionType()
        {
            if (IsVariableAccess)
            {
                return Variable.Type;
            }
            else if (IsArrayAccess)
            {
                if (Index is ConstExpression ci && ci.Value == -1)
                {
                    return PrimitiveType.Int;
                }
                else
                {
                    return ((ArrayType)Variable.Type.ProperType).ElementType;
                }
            }
            else
            {
                return Fields![^1].Type;
            }
        }
    }
}
