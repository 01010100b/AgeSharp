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
            var group = Manager.GetOrCreateGroup(Group.TYPE_EXTERMINATION, 100);
            Group.SetGroupId(id, group.Id);
        }

        [AgeMethod]
        public static void Update()
        {
            var group_count = Group.GetGroupCount();

            for (Int i = 0; i < group_count; i++)
            {
                var group = Group.GetGroup(i);
                var type = group.Type;
                group.UpdateRate = 1;

                if (type == Group.TYPE_EXTERMINATION)
                {

                }

                Group.SetGroup(i, group);
            }
        }
    }
}
