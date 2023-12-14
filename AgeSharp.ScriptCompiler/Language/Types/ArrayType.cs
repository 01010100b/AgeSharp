using System;
using System.Collections.Generic;
using System.Text;

namespace AgeSharp.ScriptCompiler.Language.Types
{
    public class ArrayType : Type
    {
        // first goal is length
        public Type ElementType { get; }
        public int Length { get; }

        public ArrayType(Type element_type, int length) : base($"{element_type.Name}[{length}]", element_type.Size * length + 1)
        {
            ElementType = element_type;
            Length = length;
        }
    }
}
