using AgeSharp.Scripting.SharpParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AgeSharp.Scripting.SharpParser.Intrinsics;

namespace Deimos.Source
{
    [AgeType]
    internal struct Group
    {
        private const int ARG0 = 450;

        public Int Type;
        public Int UpdateRate;

        [AgeMethod]
        public static void Initialize()
        {
            XsScriptCall("Group_Initialize");
        }

        [AgeMethod]
        public static Int GetGroupCount()
        {
            XsScriptCall("Group_GetGroupCount");

            return GetStrategicNumber(ARG0);
        }

        [AgeMethod]
        public static Int CreateGroup()
        {
            XsScriptCall("Group_CreateGroup");

            return GetStrategicNumber(ARG0);
        }

        [AgeMethod]
        public static Group GetGroup(Int id)
        {
            SetStrategicNumber(ARG0, id);
            XsScriptCall("Group_GetGroup");

            Group group;
            group.Type = GetStrategicNumber(ARG0);
            group.UpdateRate = GetStrategicNumber(ARG0 + 1);

            return group;
        }

        [AgeMethod]
        public static void SetGroup(Int id, Group group)
        {
            SetStrategicNumber(ARG0, id);
            SetStrategicNumber(ARG0 + 1, group.Type);
            SetStrategicNumber(ARG0 + 2, group.UpdateRate);
            XsScriptCall("Group_SetGroup");
        }

        [AgeMethod]
        public static Int GetGroupId(Int object_id)
        {
            SetStrategicNumber(ARG0, object_id);
            XsScriptCall("Group_GetGroupId");

            return GetStrategicNumber(ARG0);
        }

        [AgeMethod]
        public static void SetGroupId(Int object_id, Int id)
        {
            SetStrategicNumber(ARG0, object_id);
            SetStrategicNumber(ARG0 + 1, id);
            XsScriptCall("Group_SetGroupId");
        }

        public static SearchState SearchLocalGroupObjects(Int id)
        {
            throw new NotImplementedException();
        }
    }
}
