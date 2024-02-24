using AgeSharp.Common;
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
        public const int SN_ARG0 = 450;

        [AgeGlobal]
        public static Int Tick;

        [AgeMethod(EntryPoint = true)]
        public static void EntryPoint()
        {
            Int timestamp;
            timestamp = GetPreciseTime(0);

            if (Tick == 0)
            {
                ChatDataToSelf("Initializing...", 0);
                Initialize();
            }

            //Tests.Test();
            //Run();

            timestamp = GetPreciseTime(timestamp);

            if (true)
            {
                ChatDataToSelf("Tick %d", Tick);
                ChatDataToSelf("Duration %d", timestamp);
            }

            Tick++;
        }

        [AgeMethod]
        private static void Run()
        {

        }

        [AgeMethod]
        private static void Initialize()
        {
            Group.Initialize();
        }
    }
}
