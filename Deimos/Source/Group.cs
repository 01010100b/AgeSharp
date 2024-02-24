using AgeSharp.Common;
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
        public Int Type;
        public Int UpdateRate;
        public Point Position;

        [AgeMethod]
        public static void Initialize()
        {
            XsScriptCall("Group_Initialize");
        }

        [AgeMethod]
        public static Int GetGroupCount()
        {
            XsScriptCall("Group_GetGroupCount");

            return GetStrategicNumber(Main.SN_ARG0);
        }

        [AgeMethod]
        public static Int CreateGroup()
        {
            XsScriptCall("Group_CreateGroup");

            return GetStrategicNumber(Main.SN_ARG0);
        }

        [AgeMethod]
        public static Group GetGroup(Int id)
        {
            SetStrategicNumber(Main.SN_ARG0, id);
            XsScriptCall("Group_GetGroup");

            Group group;
            group.Type = GetStrategicNumber(Main.SN_ARG0);
            group.UpdateRate = GetStrategicNumber(Main.SN_ARG0 + 1);
            group.Position.X = GetStrategicNumber(Main.SN_ARG0 + 2);
            group.Position.Y = GetStrategicNumber(Main.SN_ARG0 + 3);

            return group;
        }

        [AgeMethod]
        public static void SetGroup(Int id, Group group)
        {
            SetStrategicNumber(Main.SN_ARG0, id);
            SetStrategicNumber(Main.SN_ARG0 + 1, group.Type);
            SetStrategicNumber(Main.SN_ARG0 + 2, group.UpdateRate);
            SetStrategicNumber(Main.SN_ARG0 + 3, group.Position.X);
            SetStrategicNumber(Main.SN_ARG0 + 4, group.Position.Y);
            XsScriptCall("Group_SetGroup");
        }

        [AgeMethod]
        public static Int GetGroupId(Int object_id)
        {
            SetStrategicNumber(Main.SN_ARG0, object_id);
            XsScriptCall("Group_GetGroupId");

            return GetStrategicNumber(Main.SN_ARG0);
        }

        [AgeMethod]
        public static void SetGroupId(Int object_id, Int id)
        {
            SetStrategicNumber(Main.SN_ARG0, object_id);
            SetStrategicNumber(Main.SN_ARG0 + 1, id);
            XsScriptCall("Group_SetGroupId");
        }

        [AgeMethod]
        public static SearchState SearchLocalGroupObjects(Int id)
        {
            FullResetSearch();
            FindLocal(-1, 240);
            SearchState search_state;
            search_state = GetSearchState();

            for (Int i = 0; i < search_state.LocalTotal; i++)
            {
                SetTargetObject(SearchSource.LOCAL, i);
                Int data = GetObjectData(ObjectData.ID);
                Int group;
                group = GetGroupId(data);

                if (group != id)
                {
                    RemoveObjects("==", SearchSource.LOCAL, ObjectData.INDEX, i);
                    i--;
                    search_state = GetSearchState();
                }
            }

            return search_state;
        }
    }
}
