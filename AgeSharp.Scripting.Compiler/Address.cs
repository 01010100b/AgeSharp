using AgeSharp.Scripting.Language;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Type = AgeSharp.Scripting.Language.Type;

namespace AgeSharp.Scripting.Compiler
{
    internal class Address
    {
        public Type Type { get; }
        public int Goal { get; }
        public bool IsRef { get; }
        public int Offset { get; }
        public int IndexStride { get; }
        public bool IsArrayAccess => IndexStride > 0; // in this case Offset is a goal holding the index

        public Address(Type type, int goal, bool is_ref, int offset = 0, int index_stride = 0)
        {
            Type = type;
            Goal = goal;
            IsRef = is_ref;
            Offset = offset;
            IndexStride = index_stride;

            Debug.Assert(Goal >= 1 && Goal <= 512);
            Debug.Assert(Offset >= 0);
            Debug.Assert(IndexStride >= 0);
        }
    }
}
