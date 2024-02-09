using AgeSharp.Scripting.Language.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language.Expressions
{
    public class ConstExpression(Type type, int value) : Expression
    {
        public override Type? Type { get; } = type;
        public int Value { get; } = value;

        public override void Validate()
        {
            if (Type is null) throw new Exception($"Const expression has type void.");
            if (Type is not PrimitiveType) throw new Exception($"Const expression of non-primitive type {Type.Name}");
            if (Type == PrimitiveType.Bool && Value != 0 && Value != 1) throw new Exception($"Bool const not 0 or 1.");
        }
    }
}
