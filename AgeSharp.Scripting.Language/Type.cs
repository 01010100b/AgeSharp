using AgeSharp.Scripting.Language.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language
{
    public abstract class Type(string name) : Validated
    {
        public string Name { get; } = name;
        public abstract int Size { get; }

        public void ValidateAssignment(Type to, bool to_isref)
        {
            if (!to_isref)
            {
                if (this != to) throw new NotSupportedException($"Can not assign {Name} to {to.Name}.");
            }
            else if (this != to)
            {
                if (this is not ArrayType af || to is not ArrayType at) throw new NotSupportedException($"Ref assign from {Name} to {to.Name} with not both being arrays.");
                if (af.ElementType != at.ElementType) throw new NotSupportedException($"Ref array assign from {Name} to {to.Name} with different element types.");
            }
        }
    }
}
