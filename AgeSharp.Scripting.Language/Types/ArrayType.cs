using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (Length < 1) throw new NotSupportedException($"Array type {Name} has length < 1.");
            if (ElementType == PrimitiveType.Void) throw new NotSupportedException($"Array {Name} has element type Void.");
            if (ElementType is ArrayType) throw new NotSupportedException($"Array {Name} has element type array.");
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
