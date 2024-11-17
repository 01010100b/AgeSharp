using AgeSharp.Common;
using AgeSharp.Scripting.SharpParser;
using Deimos.Source.Groups;
using static AgeSharp.Scripting.SharpParser.Intrinsics;

namespace Deimos.Source.Managers
{
    internal class MilitaryManager
    {
        // cmdid MILITARY MONK
        public const int GROUP_NONE = 0;
        public const int GROUP_EXTERMINATION = 1;

        [AgeGlobal]
        private static readonly Array<Group> Groups = new(9);

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
            ManageUngroupedObjects();

            for (Int i = 0; i < Groups.Length; i++)
            {
                var group = Groups[i];

                if (group.Type == GROUP_EXTERMINATION)
                {
                    SearchLocalObjects();
                    group.FilterLocalSearchList();
                    Extermination.Perform(group);
                }

                Groups[i] = group;
            }
        }

        [AgeMethod]
        private static void ManageUngroupedObjects()
        {
            FullResetSearch();
            SearchLocalObjects();
            RemoveObjects("!=", SearchSource.LOCAL, ObjectData.GROUP_FLAG, -2);
            var search_state = GetSearchState();

            if (search_state.LocalTotal == 0)
            {
                return;
            }

            for (Int i = 0; i < search_state.LocalTotal; i++)
            {
                SetTargetObject(SearchSource.LOCAL, i);

                if (GetObjectData(ObjectData.RANGE) < 3)
                {
                    // only ranged units for now
                    continue;
                }

                var id = GetObjectData(ObjectData.ID);
                var group_id = GetOrAssignGroup(GROUP_EXTERMINATION, 100);

                if (group_id == -1)
                {
                    continue;
                }

                var group = Groups[group_id];
                group.AddObject(id);
                Groups[group_id] = group;
            }
        }

        [AgeMethod]
        private static Int GetOrAssignGroup(Int type, Int max_count)
        {
            for (Int i = 0; i < Groups.Length; i++)
            {
                var group = Groups[i];

                if (group.Type == type && group.GetCount() < max_count)
                {
                    return i;
                }
                else if (group.Type == GROUP_NONE)
                {
                    group.Type = type;
                    Groups[i] = group;

                    return i;
                }
            }

            return -1;
        }

        [AgeMethod]
        private static void SearchLocalObjects()
        {
            ResetFilters();
            ResetSearch(true, true, false, false);
            FilterInclude(CmdId.MILITARY, UnitAction.IGNORE, UnitOrder.IGNORE, -1);
            FindLocal(-1, Main.MAX_LOCAL_LIST);
            FilterInclude(CmdId.MONK, UnitAction.IGNORE, UnitOrder.IGNORE, -1);
            FindLocal(-1, Main.MAX_LOCAL_LIST);
        }
    }
}
