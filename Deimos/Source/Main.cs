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
        [AgeGlobal]
        private static Int Timestamp;

        [AgeMethod]
        public static Int GetTimePassed()
        {
            return GetPreciseTime(Timestamp);
        }

        [AgeMethod(EntryPoint = true)]
        public static void EntryPoint()
        {
            Timestamp = GetPreciseTime(0);

            if (Tick == 0)
            {
                ChatDataToSelf("Initializing...", 0);
                Initialize();
            }

            Tests.Test();
            Run();

            Timestamp = GetPreciseTime(Timestamp);

            if (true)
            {
                ChatDataToSelf("Tick %d", Tick);
                ChatDataToSelf("Duration %d", Timestamp);
            }

            Tick++;
        }

        [AgeMethod]
        private static void Run()
        {
            Manager.Update();
            Controller.Update();
        }

        [AgeMethod]
        private static void Initialize()
        {
            Memory.Initialize();
            Group.Initialize();
            Manager.Initialize();
            Controller.Initialize();
            Micro.Initialize();
        }
    }
}
