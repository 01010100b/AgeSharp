using AgeSharp.Scripting.Language.Types;

namespace AgeSharp.Scripting.Language
{
    public class Variable : Validated
    {
        public string Name { get; }
        public Type Type { get; }
        public int Size => Type.Size;

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
