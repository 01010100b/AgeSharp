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
        [AgeMethod(EntryPoint = true)]
        public static void EntryPoint()
        {
            Array<Int> array = new(10);
            Int a = 3;
            array[a] = 7;

            for (Int i = 0; i < 10; i += 1)
            {
                for (Int j = 0; j < 3; j += 1)
                {
                    Increase(ref array[a], 2);
                }
            }

            ChatDataToSelf("t0 %d", array[a]);
            Test1(ref array[a]);
        }

        [AgeMethod]
        private static void Increase(ref Int a, Int b)
        {
            a += b;
        }

        [AgeMethod]
        private static void Test1(ref Int a)
        {
            ChatDataToSelf("t1 %d", a);
            Test2(ref a);
        }

        [AgeMethod]
        private static void Test2(ref Int a)
        {
            ChatDataToSelf("t2 %d", a);
            Test3(a);
        }

        [AgeMethod]
        private static void Test3(Int a)
        {
            ChatDataToSelf("t3 %d", a);
        }
    }
}
