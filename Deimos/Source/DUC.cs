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
    internal class DUC
    {
        [AgeMethod]
        public static void RotateLocalSearchList(Int count)
        {
            for (Int i = 0; i < count; i++)
            {
                SetTargetObject(SearchSource.LOCAL, 0);
                var id = GetObjectData(ObjectData.ID);
                AddObjectById(SearchSource.LOCAL, id);
                RemoveObjects("==", SearchSource.LOCAL, ObjectData.INDEX, 0);
            }
        }

        [AgeMethod]
        public static Point GetAverageLocalPosition()
        {
            Point point;
            var search_state = GetSearchState();

            if (search_state.LocalTotal == 0)
            {
                return point;
            }

            for (Int i = 0; i < search_state.LocalTotal; i++)
            {
                SetTargetObject(SearchSource.LOCAL, i);
                point.X += GetObjectData(ObjectData.POINT_X);
                point.Y += GetObjectData(ObjectData.POINT_Y);
            }

            point.X /= search_state.LocalTotal;
            point.Y /= search_state.LocalTotal;

            return point;
        }

        [AgeMethod]
        public static Int MoveLocalListToControlGroups()
        {
            Int count = 0;
            var search_state = GetSearchState();

            while (search_state.LocalTotal > 0)
            {
                CreateGroup(0, Main.MAX_CONTROL_GROUP_SIZE, count);
                RemoveObjects("<", SearchSource.LOCAL, ObjectData.INDEX, Main.MAX_CONTROL_GROUP_SIZE);
                count++;
                search_state = GetSearchState();

                if (count >= Main.MAX_CONTROL_GROUPS)
                {
                    break;
                }
            }

            return count;
        }
    }
}
