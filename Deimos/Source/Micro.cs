﻿using AgeSharp.Common;
using AgeSharp.Scripting.SharpParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AgeSharp.Scripting.SharpParser.Intrinsics;

namespace Deimos.Source
{
    internal class Micro
    {
        private const int CURRENT_TARGET_SCORE = 300;

        [AgeMethod]
        public static void Retarget(Int id)
        {
            var current_target = CustomObjectData.Get(id, CustomObjectData.TARGET);
            var current_score = GetTargetScore(id, current_target) + CURRENT_TARGET_SCORE;

            var search_state = GetSearchState();

            for (Int t = 0; t < search_state.RemoteTotal; t++)
            {
                SetTargetObject(SearchSource.REMOTE, t);
                var target = GetObjectData(ObjectData.ID);
                var score = GetTargetScore(id, target);

                if (score > current_score)
                {
                    current_target = target;
                    current_score = score;
                }
            }

            CustomObjectData.Set(id, CustomObjectData.TARGET, current_target);
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