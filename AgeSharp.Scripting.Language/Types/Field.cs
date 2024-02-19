using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language.Types
{
    public class Field(string name, Type type) : Validated
    {
        public string Name { get; } = name;
        public Type Type { get; } = type;

        public override void Validate()
        {
            ValidateName(Name);
            if (Type == PrimitiveType.Void) throw new NotSupportedException($"Field {Type.Name}.{Name} has type Void.");
            if (Type is ArrayType) throw new NotSupportedException($"Field {Name} has array type.");
            if (Type is RefType) throw new NotSupportedException($"Field {Name} has ref type.");
        }

        public override string ToString()
        {
            return $"{Type.Name} {Name}";
        }
    }
}
