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

        }

        [AgeMethod]
        public static void Update()
        {
            AssignUngroupedObjects();
            MilitaryManager.Update();
        }

        [AgeMethod]
        public static Group GetOrCreateGroup(Int type, Int max_count)
        {
            var group_count = Group.GetGroupCount();
            Int id = -1;

            for (Int i = 0; i < group_count; i++)
            {
                var g = Group.GetGroup(i);

                if (g.Type == type && g.Count <= max_count)
                {
                    id = i;

                    break;
                }
            }

            if (id == -1)
            {
                id = Group.CreateGroup();
            }

            var group = Group.GetGroup(id);

            if (group.Type == Group.TYPE_NONE)
            {
                group.Id = id;
                group.Type = type;
                Group.SetGroup(id, group);
            }

            return group;
        }

        [AgeMethod]
        private static void AssignUngroupedObjects()
        {
            var search_state = Group.SearchLocalGroupObjects(-1);

            for (Int i = 0; i < search_state.LocalTotal; i++)
            {
                SetTargetObject(SearchSource.LOCAL, i);
                var id = GetObjectData(ObjectData.ID);
                var data = GetObjectData(ObjectData.CMDID);

                if (data == (int)CmdId.MILITARY)
                {
                    MilitaryManager.ManageUngroupedObject(id);
                }
                else
                {
                    // ungroupable
                    Group.SetGroupId(id, -2);
                }
            }
        }
    }
}
