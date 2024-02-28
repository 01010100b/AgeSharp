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
            Group group;
            group.Id = 7;
            group.DoStuff();
        }
    }
}
