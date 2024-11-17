using AgeSharp.Common;

namespace AgeSharp.Scripting.Language.Types
{
    public class Field(string name, Type type) : Validated
    {
        public string Name { get; } = name;
        public Type Type { get; } = type;

        public override void Validate()
        {
            ValidateName(Name);
            Throw.If<NotSupportedException>(Type == PrimitiveType.Void, $"Field {Type.Name}.{Name} has type Void.");
            Throw.If<NotSupportedException>(Type is ArrayType, $"Field {Name} has array type.");
            Throw.If<NotSupportedException>(Type is RefType, $"Field {Name} has ref type.");
        }

        public override string ToString()
        {
            return $"{Type.Name} {Name}";
        }
    }
}
