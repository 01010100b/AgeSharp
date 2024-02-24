using AgeSharp.Scripting.SharpParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AgeSharp.Scripting.SharpParser.Intrinsics;

namespace Deimos.Source
{
    internal class Tests
    {
        [AgeMethod]
        public static void Test()
        {
            Array<Int> array = new(10);
            
            ChatDataToSelf("a %d", array[3]);
            Test2(ref array[3]);
            ChatDataToSelf("a2 %d", array[3]);
        }

        [AgeMethod]
        private static void Test1(Array<Int> a)
        {
            Int b = a.Length;
            ChatDataToSelf("length %d", b);
            //ChatDataToSelf("t1 %d", a[3]);

            Test2(ref a[3]);
        }

        [AgeMethod]
        private static void Test2(ref Int a)
        {
            //ChatDataToSelf("t2 %d", a);
            Test3(ref a);
        }

        [AgeMethod]
        private static void Test3(ref Int a)
        {
            //ChatDataToSelf("t3 %d", a);
            Test4(a);
            a++;
        }

        [AgeMethod]
        private static void Test4(Int a)
        {
            //ChatDataToSelf("t4 %d", a);
        }
    }
}
