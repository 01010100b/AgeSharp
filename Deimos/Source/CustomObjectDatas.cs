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
            Int data = (int)custom_data;

            if (data < 0 || data >= CustomDataPtrs.Length)
            {
                throw new AgeException("Invalid custom object data");
            }

            return Memory.GetValue(CustomDataPtrs[data], object_id);
        }

        [AgeMethod]
        public static void Set(Int object_id, CustomObjectData custom_data, Int value)
        {
            Int data = (int)custom_data;

            if (data < 0 || data >= CustomDataPtrs.Length)
            {
                throw new AgeException("Invalid custom object data");
            }

            Memory.SetValue(CustomDataPtrs[data], object_id, value);
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
