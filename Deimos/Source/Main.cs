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
        [AgeGlobal]
        public static Int Tick;

        [AgeMethod(EntryPoint = true)]
        public static void EntryPoint()
        {
            Int timestamp;
            timestamp = GetPreciseTime(0);

            if (Tick == 0)
            {
                Initialize();
            }

            Test();

            timestamp = GetPreciseTime(timestamp);

            if (true)
            {
                ChatDataToSelf("Tick %d", Tick);
                ChatDataToSelf("Duration %d", timestamp);
            }

            Tick++;
        }

        [AgeMethod]
        private static void Test()
        {
            Int a;
        }

        [AgeMethod]
        private static void Initialize()
        {
            Group.Initialize();
        }
    }
}
