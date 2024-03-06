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
        public static Bool operator ==(Int a, Int b) => throw new Exception();
        public static Bool operator !=(Int a, Int b) => throw new Exception();
        public static Bool operator <(Int a, Int b) => throw new Exception();
        public static Bool operator >(Int a, Int b) => throw new Exception();
        public static Bool operator <=(Int a, Int b) => throw new Exception();
        public static Bool operator >=(Int a, Int b) => throw new Exception();
        public static Int operator +(Int a, Int b) => throw new Exception();
        public static Int operator -(Int a, Int b) => throw new Exception();
        public static Int operator ++(Int a) => throw new Exception();
        public static Int operator --(Int a) => throw new Exception();
        public static Int operator *(Int a, Int b) => throw new Exception();
        public static Int operator /(Int a, Int b) => throw new Exception();
        public static Int operator %(Int a, Int b) => throw new Exception();
        public static Int operator &(Int a, Int b) => throw new Exception();
        public static Int operator |(Int a, Int b) => throw new Exception();
        public static Int operator ~(Int a) => throw new Exception();
        public static Int operator ^(Int a, Int b) => throw new Exception();
        public static Int operator <<(Int a, Int count) => throw new Exception();
        public static Int operator >>(Int a, Int count) => throw new Exception();

        public override bool Equals(object? obj) => throw new Exception();
        public override int GetHashCode() => throw new Exception();
    }
}
