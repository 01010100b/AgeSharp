using AgeSharp.Common;
using AgeSharp.Scripting.SharpParser;
using Deimos.Source.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AgeSharp.Scripting.SharpParser.Intrinsics;

namespace Deimos.Source
{
    internal class Manager
    {
        [AgeMethod]
        public static void Initialize()
        {
            DiplomacyManager.Initialize();
            MilitaryManager.Initialize();
        }

        [AgeMethod]
        public static void Update()
        {
            DiplomacyManager.Update();
            MilitaryManager.Update();
        }
    }
}
