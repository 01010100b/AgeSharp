using AgeSharp.Scripting.SharpParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static AgeSharp.Scripting.SharpParser.Intrinsics;

namespace Deimos.Source.Managers
{
    internal class MilitaryManager
    {
        // cmdid MILITARY MONK

        [AgeGlobal]
        private readonly static Array<Group> Groups = new(9);

        [AgeMethod]
        public static void Initialize()
        {
            for (Int i = 0; i < Groups.Length; i++)
            {
                var group = Groups[i];
                group.Id = i + 1;
                Groups[i] = group;
            }
        }

        [AgeMethod]
        public static void Update()
        {

        }
    }
}
