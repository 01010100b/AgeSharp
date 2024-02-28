using AgeSharp.Scripting.SharpParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AgeSharp.Scripting.SharpParser.Intrinsics;

namespace Deimos.Source
{
    internal class Memory
    {
        [AgeMethod]
        public static void Initialize()
        {
            XsScriptCall("Memory_Initialize");
            ChatDataToSelf("mem init", 0);
        }

        [AgeMethod]
        public static Int Allocate(Int size)
        {
            SetStrategicNumber(Main.SN_ARG0, size);
            XsScriptCall("Memory_Allocate");

            return GetStrategicNumber(Main.SN_ARG0);
        }

        [AgeMethod]
        public static void Free(Int pointer)
        {
            SetStrategicNumber(Main.SN_ARG0, pointer);
            XsScriptCall("Memory_Free");
        }

        [AgeMethod]
        public static Int GetValue(Int pointer, Int index)
        {
            SetStrategicNumber(Main.SN_ARG0, pointer);
            SetStrategicNumber(Main.SN_ARG0 + 1, index);
            XsScriptCall("Memory_GetValue");

            return GetStrategicNumber(Main.SN_ARG0);
        }

        [AgeMethod]
        public static void SetValue(Int pointer, Int index, Int value)
        {
            SetStrategicNumber(Main.SN_ARG0, pointer);
            SetStrategicNumber(Main.SN_ARG0 + 1, index);
            SetStrategicNumber(Main.SN_ARG0 + 2, value);
            XsScriptCall("Memory_SetValue");
        }
    }
}
