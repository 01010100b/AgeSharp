using AgeSharp.Scripting.Language.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language
{
    public class Variable : Validated
    {
        public string Name { get; }
        public Type Type { get; }
        public int Size => Type.Size;
        public Type ProperType => Type is RefType rt ? rt.ReferencedType : Type;

        public Variable(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public override void Validate()
        {
            ValidateName(Name);
            if (Type == PrimitiveType.Void) throw new NotSupportedException($"Variable {Name} has type Void.");
        }

        public override string ToString()
        {
            return $"{Type.Name} {Name};";
        }
    }
}
