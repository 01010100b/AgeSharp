using AgeSharp.Common;

namespace AgeSharp.Scripting.Language.Types
{
    public class ArrayType(Type element_type, int length) : Type(GetArrayTypeName(element_type, length))
    {
        public static string GetArrayTypeName(Type element_type, int length) => $"{element_type.Name}[{length}]";

        // first goal is length
        public override int Size => 1 + ElementType.Size * Length;
        public Type ElementType { get; } = element_type;
        public int Length { get; } = length;

        public override void Validate()
        {
            Throw.If<NotSupportedException>(Length < 1, $"Array type {Name} has length < 1.");
            Throw.If<NotSupportedException>(ElementType == PrimitiveType.Void, $"Array {Name} has void element type.");
            Throw.If<NotSupportedException>(ElementType is ArrayType, $"Array {Name} has array element type.");
            Throw.If<NotSupportedException>(ElementType is RefType, $"Array {Name} has ref element type.");
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
