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
            Int a = 7;
            Int b = 13;

            if ((a | b) != 15)
            {
                throw new AgeException("Fail Test0.0");
            }

            if ((a & b) != 5)
            {
                throw new AgeException("Fail Test0.1");
            }

            if ((~a) != -8)
            {
                throw new AgeException("Fail Test0.2");
            }

            if ((a ^ b) != 10)
            {
                throw new AgeException("Fail Test0.3");
            }

            if ((a << 7) != 896)
            {
                throw new AgeException("Fail Test0.4");
            }

            if ((b >> 1) != 6)
            {
                throw new AgeException("Fail Test0.5");
            }
        }
    }
}
