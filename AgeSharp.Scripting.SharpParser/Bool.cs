using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.SharpParser
{
    public readonly struct Bool
    {
        public static implicit operator Bool(bool b) => throw new Exception();
        public static explicit operator Int(Bool a) => throw new Exception();
        public static bool operator true(Bool b) => throw new Exception();
        public static bool operator false(Bool b) => throw new Exception();
        public static Bool operator &(Bool a, Bool b) => throw new Exception();
        public static Bool operator |(Bool a, Bool b) => throw new Exception();
        public static Bool operator !(Bool a) => throw new Exception();
    }
}
