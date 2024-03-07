using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.SharpParser
{
    public struct Point
    {
        public Int X;
        public Int Y;

        public static Bool operator ==(Point a, Point b) => throw new Exception();
        public static Bool operator !=(Point a, Point b) => throw new Exception();
        public static Point operator +(Point a, Point b) => throw new Exception();
        public static Point operator -(Point a, Point b) => throw new Exception();
        public static Point operator *(Point a, Int b) => throw new Exception();
        public static Point operator /(Point a, Int b) => throw new Exception();

        public override bool Equals(object? obj) => throw new Exception();
        public override int GetHashCode() => throw new Exception();
    }
}
