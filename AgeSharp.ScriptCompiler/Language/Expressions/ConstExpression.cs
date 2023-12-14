using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgeSharp.ScriptCompiler.Language.Expressions
{
    public class ConstExpression : Expression
    {
        public Type ConstType { get; }
        public object ConstValue { get; }
        public override Type Type => ConstType;

        public ConstExpression(Type type, object value)
        {
            ConstType = type;
            ConstValue = value;
        }
    }
}
