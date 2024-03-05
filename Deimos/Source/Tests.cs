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
            RunSuite();
        }

        [AgeMethod]
        private static void RunSuite()
        {
            Test0();
        }

        [AgeMethod]
        private static void Test0()
        {
            // bitwise operators

            if ((7 | 13) != 15)
            {
                throw new AgeException("Fail Test0.0");
            }

            if ((7 & 13) != 5)
            {
                throw new AgeException("Fail Test0.1");
            }

            if ((~7) != -8)
            {
                throw new AgeException("Fail Test0.2");
            }

            if ((7 ^ 13) != 10)
            {
                throw new AgeException("Fail Test0.3");
            }
        }
    }
}
