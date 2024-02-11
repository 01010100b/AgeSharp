using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler
{
    internal class Address(int base_goal, bool ref_base, int offset_goal, bool ref_offset)
    {
        public int BaseGoal { get; } = base_goal;
        public bool RefBase { get; } = ref_base;
        public int OffsetGoal { get; } = offset_goal;
        public bool RefOffset { get; } = ref_offset;
    }
}
