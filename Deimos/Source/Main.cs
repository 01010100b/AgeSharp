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
            Int a;
            a = 17;
            ChatDataToSelf("t0 %d", a);
            Test1(ref a);
        }
        
        [AgeMethod]
        public static void Test1(ref Int a)
        {
            ChatDataToSelf("t1 %d", a);
            Test2(ref a);
        }

        [AgeMethod]
        public static void Test2(ref Int a)
        {
            ChatDataToSelf("t2 %d", a);
            Test3(a);
        }

        [AgeMethod]
        public static void Test3(Int a)
        {
            ChatDataToSelf("t3 %d", a);
        }
    }
}
