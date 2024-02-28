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
        [AgeMethod]
        public static void ManageUngroupedObject(Int id)
        {
            var group = Manager.GetOrCreateGroup(Group.TYPE_EXTERMINATION, 39);
            group.UpdateRate = 1;
            Group.SetGroup(group.Id, group);
            Group.SetGroupId(id, group.Id);
        }

        [AgeMethod]
        public static void Initialize()
        {

        }

        [AgeMethod]
        public static void Update()
        {

        }
    }
}
