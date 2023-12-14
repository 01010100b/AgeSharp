using AgeSharp.ScriptCompiler.Language.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgeSharp.ScriptCompiler.Language.Expressions
{
    public class AccessorExpression : Expression
    {
        public Variable Variable { get; }
        public Expression OffsetExpression { get; }
        public Field Field { get; }
        public override Type Type => GetExpressionType();

        public AccessorExpression(Variable variable)
        {
            Variable = variable;
            OffsetExpression = null;
            Field = null;
        }

        public AccessorExpression(Variable variable, Expression offset_expression)
        {
            Variable = variable;
            OffsetExpression = offset_expression;
            Field = null;
        }

        public AccessorExpression(Variable variable, Field field)
        {
            Variable = variable;
            OffsetExpression = null;
            Field = field;
        }

        private Type GetExpressionType()
        {
            if (OffsetExpression != null)
            {
                return ((ArrayType)Variable.Type).ElementType;
            }
            else if (Field != null)
            {
                return Field.Type;
            }
            else
            {
                return Variable.Type;
            }
        }
    }
}
