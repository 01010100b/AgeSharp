using AgeSharp.Scripting.SharpParser;

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
            TestBitwise();
            TestPoints();
            Int t = TestInline(123, 456);
        }

        [AgeMethod]
        private static void TestBitwise()
        {
            Int a = 7;
            Int b = 13;

            if ((a | b) != 15)
            {
                throw new AgeException("Fail Bitwise 0");
            }

            if ((a & b) != 5)
            {
                throw new AgeException("Fail Bitwise 1");
            }

            if ((~a) != -8)
            {
                throw new AgeException("Fail Bitwise 2");
            }

            if ((a ^ b) != 10)
            {
                throw new AgeException("Fail Bitwise 3");
            }

            if ((a << 7) != 896)
            {
                throw new AgeException("Fail Bitwise 4");
            }

            if ((b >> 1) != 6)
            {
                throw new AgeException("Fail Bitwise 5");
            }
        }

        [AgeMethod]
        private static void TestPoints()
        {
            Point a;
        }

        [AgeMethod]
        private static Int TestInline(Int a, Int b)
        {
            if (a > b)
            {
                return 111;
            }
            else
            {
                return 222;
            }
        }
    }
}
