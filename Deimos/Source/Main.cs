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
        [AgeGlobal]
        public static Int Tick;
        [AgeGlobal]
        public static Array<Int> Stuff = new(10);

        [AgeMethod(EntryPoint = true)]
        public static void EntryPoint()
        {
        }
    }
}
