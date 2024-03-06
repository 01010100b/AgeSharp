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
        public const int MAX_LOCAL_LIST = 240;
        public const int MAX_REMOTE_LIST = 40;
        public const int MAX_CONTROL_GROUPS = 10;
        public const int MAX_CONTROL_GROUP_SIZE = 40;
        public const int SN_ARG0 = 450;

        [AgeGlobal]
        private static Int Tick;
        [AgeGlobal]
        private static Int Timestamp;

        [AgeMethod]
        public static Int GetTick()
        {
            return Tick;
        }

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
        }

        [AgeMethod]
        private static void Initialize()
        {
            Memory.Initialize();
            CustomObjectData.Initialize();
            Manager.Initialize();
        }
    }
}
