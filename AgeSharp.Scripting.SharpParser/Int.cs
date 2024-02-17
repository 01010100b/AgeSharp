using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.SharpParser
{
    public readonly struct Int
    {
        public static implicit operator Int(int value) => throw new Exception();

        public static implicit operator int(Int value) => throw new Exception();
    }
}
