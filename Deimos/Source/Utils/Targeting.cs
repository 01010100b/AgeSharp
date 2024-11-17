using AgeSharp.Common;
using AgeSharp.Scripting.SharpParser;
using static AgeSharp.Scripting.SharpParser.Intrinsics;

namespace Deimos.Source.Utils
{
    internal class Targeting
    {
        private const int CURRENT_TARGET_SCORE = 300;

        [AgeMethod]
        public static void Retarget(Int id)
        {
            if (GetRandom(100) < 10)
            {
                // forget current target every once in a while
                CustomObjectDatas.Set(id, CustomObjectData.TARGET, -1);
            }

            var current_target = CustomObjectDatas.Get(id, CustomObjectData.TARGET);

            if (ObjectExists(current_target) && GetRandom(100) < 75)
            {
                // too expensive to retarget everyone every tick
                return;
            }

            var current_score = GetTargetScore(id, current_target) + CURRENT_TARGET_SCORE;

            var search_state = GetSearchState();

            for (Int i = 0; i < search_state.RemoteTotal; i++)
            {
                SetTargetObject(SearchSource.REMOTE, i);
                var target = GetObjectData(ObjectData.ID);
                var score = GetTargetScore(id, target);

                if (score > current_score)
                {
                    current_target = target;
                    current_score = score;
                }
            }

            CustomObjectDatas.Set(id, CustomObjectData.TARGET, current_target);
        }

        [AgeMethod]
        private static Int GetTargetScore(Int object_id, Int target_id)
        {
            if (!ObjectExists(target_id))
            {
                return -1000000;
            }

            Int score = 0;
            Point position;
            SetTargetById(object_id);
            position.X = GetObjectData(ObjectData.POINT_X);
            position.Y = GetObjectData(ObjectData.POINT_Y);
            SetTargetPoint(position);

            SetTargetById(target_id);
            score -= 100 * GetObjectData(ObjectData.DISTANCE);

            return score;
        }
    }
}
