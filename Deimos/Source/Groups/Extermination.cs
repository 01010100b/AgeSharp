using AgeSharp.Common;
using AgeSharp.Scripting.SharpParser;
using Deimos.Source.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AgeSharp.Scripting.SharpParser.Intrinsics;

namespace Deimos.Source.Groups
{
    internal class Extermination
    {
        [AgeMethod]
        public static void Perform(Group group)
        {
            if (group.Type != MilitaryManager.GROUP_EXTERMINATION)
            {
                throw new AgeException("Not extermination group");
            }

            if (group.GetCount() == 0)
            {
                return;
            }

            var position = DUC.GetAverageLocalPosition();
            var groups = DUC.MoveLocalListToControlGroups();
            var enemy = DiplomacyManager.GetEnemy(0);

            FullResetSearch();
            FilterDistance(-1, 20);
            SetTargetPoint(position);
            FilterInclude(CmdId.MILITARY, UnitAction.IGNORE, UnitOrder.IGNORE, -1);
            FindRemote(enemy, -1, Main.MAX_REMOTE_LIST);
            FilterInclude(CmdId.MONK, UnitAction.IGNORE, UnitOrder.IGNORE, -1);
            FindRemote(enemy, -1, Main.MAX_REMOTE_LIST);
            FilterInclude(CmdId.VILLAGER, UnitAction.IGNORE, UnitOrder.IGNORE, -1);
            FindRemote(enemy, -1, Main.MAX_REMOTE_LIST);
            var search_state = GetSearchState();

            if (search_state.RemoteTotal == 0)
            {
                return;
            }

            for (Int i = 0; i < groups; i++)
            {
                ResetSearch(true, true, false, false);
                SetGroup(SearchSource.LOCAL, i);
                var count = GetGroupSize(i);

                for (Int j = 0; j < count; j++)
                {
                    SetTargetObject(SearchSource.LOCAL, j);
                    var id = GetObjectData(ObjectData.ID);
                    Micro.Retarget(id);
                }
            }
        }
    }
}
