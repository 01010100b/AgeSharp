using AgeSharp.Scripting.SharpParser;
using Deimos.Source.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deimos.Source
{
    internal enum CustomObjectData
    {
        TARGET
    }

    internal class CustomObjectDatas
    {
        private const int CUSTOM_DATAS = 3;

        [AgeGlobal]
        private static readonly Array<Int> CustomDataPtrs = new(CUSTOM_DATAS);

        [AgeMethod]
        public static Int Get(Int object_id, CustomObjectData custom_data)
        {
            if (custom_data < 0 || (int)custom_data >= CustomDataPtrs.Length)
            {
                throw new AgeException("Invalid custom object data");
            }

            return Memory.GetValue(CustomDataPtrs[(int)custom_data], object_id);
        }

        [AgeMethod]
        public static void Set(Int object_id, CustomObjectData custom_data, Int value)
        {
            if (custom_data < 0 || (int)custom_data >= CustomDataPtrs.Length)
            {
                throw new AgeException("Invalid custom object data");
            }

            Memory.SetValue(CustomDataPtrs[(int)custom_data], object_id, value);
        }

        [AgeMethod]
        public static void Initialize()
        {
            for (Int i = 0; i < CustomDataPtrs.Length; i++)
            {
                CustomDataPtrs[i] = Memory.Allocate(100000);
            }
        }
    }
}
