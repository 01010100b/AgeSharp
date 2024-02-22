using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.SharpParser
{
    public static class Intrinsics
    {
        public static void ChatDataToSelf(string message, Int data) => throw new Exception();
        public static Int GetStrategicNumber(Int sn) => throw new Exception();
        public static void SetStrategicNumber(Int sn, Int value) => throw new Exception();
        public static void XsScriptCall(string script) => throw new Exception();

        // Math
        public static Bool Equals(Int a, Int b) => throw new Exception();
        public static Bool LessThan(Int a, Int b) => throw new Exception();
        public static Bool GreaterThan(Int a, Int b) => throw new Exception();
        public static Int Add(Int a, Int b) => throw new Exception();
        public static Int Sub(Int a, Int b) => throw new Exception();
        public static Int Mul(Int a, Int b) => throw new Exception();
        public static Int Div(Int a, Int b) => throw new Exception();
        public static Int Mod(Int a, Int b) => throw new Exception();
        public static Int Max(Int a, Int b) => throw new Exception();
        public static Int Min(Int a, Int b) => throw new Exception();
    }
}
