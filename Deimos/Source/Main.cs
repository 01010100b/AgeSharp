using AgeSharp.Scripting.SharpParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AgeSharp.Scripting.SharpParser.Intrinsics;

namespace Deimos.Source
{
    public class Main
    {
        const int S = 2;

        [AgeMethod(EntryPoint = true)]
        public static void EntryPoint()
        {
            Int[] a = new Int[S + 1];
            Int[] x = new Int[S + 1];
            Int b;
            Int c;
            c = 2;
            Fibonacci(a[c]);
            ChatDataToSelf("my message %d", 2);
            return;
        }

        [AgeMethod]
        public static Int Fibonacci(Int i)
        {
            Int a, b, fa, fb;
            b = i;
            return S * 3 + 1;
        }
    }
}
