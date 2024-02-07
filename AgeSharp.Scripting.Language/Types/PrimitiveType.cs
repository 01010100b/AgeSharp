using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language.Types
{
    public class PrimitiveType : Type
    {
        public static Type Int { get; } = new PrimitiveType("Int", 1);
        public static Type Bool { get; } = new PrimitiveType("Bool", 1);

        public override int Size { get; }

        private PrimitiveType(string name, int size) : base(name)
        {
            Size = size;
        }
    }
}
