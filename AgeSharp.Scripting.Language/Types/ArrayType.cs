using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language.Types
{
    public class ArrayType : Type
    {
        public static string GetArrayTypeName(Type element_type, int length) => $"{element_type.Name}[{length}]";

        // first goal is length
        public override int Size => ElementType.Size * Length + 1;
        public Type ElementType { get; }
        public int Length { get; }

        internal ArrayType(Type element_type, int length) : base(GetArrayTypeName(element_type, length))
        {
            ElementType = element_type;
            Length = length;
        }

        public override void Validate()
        {
            base.Validate();

            ElementType.Validate();
            if (Length < 1) throw new Exception($"Array type {Name} has length < 1.");
        }
    }
}
