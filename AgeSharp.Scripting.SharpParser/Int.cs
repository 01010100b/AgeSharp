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
        public static Bool operator <(Int a, Int b) => throw new Exception();
        public static Bool operator >(Int a, Int b) => throw new Exception();
        public static Int operator +(Int a, Int b) => throw new Exception();
        public static Int operator -(Int a, Int b) => throw new Exception();
    }
}
