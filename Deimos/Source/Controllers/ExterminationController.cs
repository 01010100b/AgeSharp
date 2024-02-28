using AgeSharp.Common;
using AgeSharp.Scripting.SharpParser;
using Deimos.Source.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AgeSharp.Scripting.SharpParser.Intrinsics;

namespace Deimos.Source.Controllers
{
    internal class ExterminationController
    {
        [AgeMethod]
        public static void Update(Group group)
        {
            var count = Group.SearchLocalGroupObjects(group.Id);
            ChatDataToSelf("count %d", count);
            CreateGroup(0, 40, 1);

            for (Int i = 0; i < count; i++)
            {
                ResetSearch(true, true, false, false);
                SetGroup(SearchSource.LOCAL, 1);
                RemoveObjects("!=", SearchSource.LOCAL, ObjectData.INDEX, i);

                SetTargetObject(SearchSource.LOCAL, 0);
                var id = GetObjectData(ObjectData.ID);
                ChatDataToSelf("performing %d", id);
                Point my_pos;
                my_pos.X = GetObjectData(ObjectData.POINT_X);
                my_pos.Y = GetObjectData(ObjectData.POINT_Y);
                SetTargetPoint(my_pos);
                CleanSearch(SearchSource.REMOTE, ObjectData.DISTANCE, SearchOrder.ASCENDING);
                SetTargetObject(SearchSource.REMOTE, 0);
                var target_id = GetObjectData(ObjectData.ID);
                Micro.Perform(id, target_id);
            }
            
        }

        [AgeMethod]
        private static Int SearchTargets()
        {
            ResetSearch(false, false, true, true);
            var enemy = DiplomacyManager.GetPlayer((int)PlayerStance.ENEMY, 0);
            FindRemote(enemy, -1, 40);
            var search_state = GetSearchState();

            return search_state.RemoteTotal;
        }
    }
}
