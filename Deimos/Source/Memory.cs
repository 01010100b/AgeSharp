using AgeSharp.Scripting.SharpParser;
using static AgeSharp.Scripting.SharpParser.Intrinsics;

namespace Deimos.Source
{
    internal class Memory
    {
        [AgeMethod]
        public static void Initialize()
        {
            XsScriptCall("Memory_Initialize");
        }

        [AgeMethod]
        public static Int Allocate(Int size)
        {
            if (size < 0)
            {
                throw new AgeException("Allocation size < 0");
            }

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
            if (index < 0)
            {
                throw new AgeException("Memory index < 0");
            }

            SetStrategicNumber(Main.SN_ARG0, pointer);
            SetStrategicNumber(Main.SN_ARG0 + 1, index);
            XsScriptCall("Memory_GetValue");

            return GetStrategicNumber(Main.SN_ARG0);
        }

        [AgeMethod]
        public static void SetValue(Int pointer, Int index, Int value)
        {
            if (index < 0)
            {
                throw new AgeException("Memory index < 0");
            }

            SetStrategicNumber(Main.SN_ARG0, pointer);
            SetStrategicNumber(Main.SN_ARG0 + 1, index);
            SetStrategicNumber(Main.SN_ARG0 + 2, value);
            XsScriptCall("Memory_SetValue");
        }
    }
}
