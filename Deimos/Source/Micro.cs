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
    internal class Micro
    {
        [AgeGlobal]
        private static Int PtrData0;
        [AgeGlobal]
        private static Int PtrData1;

        [AgeMethod]
        public static Int GetUnitData0(Int object_id)
        {
            return Memory.GetValue(PtrData0, object_id);
        }

        [AgeMethod]
        public static void SetUnitData0(Int object_id, Int value)
        {
            Memory.SetValue(PtrData0, object_id, value);
        }

        [AgeMethod]
        public static Int GetUnitData1(Int object_id)
        {
            return Memory.GetValue(PtrData1, object_id);
        }

        [AgeMethod]
        public static void SetUnitData1(Int object_id, Int value)
        {
            Memory.SetValue(PtrData1, object_id, value);
        }

        [AgeMethod]
        public static void Initialize()
        {
            PtrData0 = Memory.Allocate(100000);
            PtrData1 = Memory.Allocate(100000);
        }

        [AgeMethod]
        public static void Perform(Int object_id, Int target_id)
        {
            PerformRangedRanged(object_id, target_id);
        }

        [AgeMethod]
        private static void PerformRangedRanged(Int object_id, Int target_id)
        {
            SetTargetById(object_id);
            var next_attack = GetObjectData(ObjectData.NEXT_ATTACK);
            var reload_time = GetObjectData(ObjectData.RELOAD_TIME);
            var type_id = GetObjectData(ObjectData.UPGRADE_TYPE);
            var safe_next_attack = reload_time - GetAttackDelay(type_id);

            if (next_attack <= 0)
            {
                ResetSearch(true, true, false, false);
                AddObjectById(SearchSource.LOCAL, object_id);
                SetTargetById(target_id);
                TargetObjects(true, UnitAction.DEFAULT, UnitFormation.IGNORE, UnitStance.IGNORE);
            }
            else if (next_attack < safe_next_attack)
            {
                
                var target_pos = GetEvasionTarget(object_id, target_id);
                ResetSearch(true, true, false, false);
                AddObjectById(SearchSource.LOCAL, object_id);
                SetTargetPoint(target_pos);
                TargetPoint(target_pos, TargetPointAdjustment.PRECISE, UnitAction.MOVE, UnitFormation.IGNORE, UnitStance.NO_ATTACK);
            }
        }

        [AgeMethod]
        private static Int GetAttackDelay(Int type_id)
        {
            return 500;
        }

        [AgeMethod]
        private static Point GetEvasionTarget(Int object_id, Int target_id)
        {
            Point my_pos;
            Point target_pos;
            SetTargetById(object_id);
            my_pos.X = GetObjectData(ObjectData.PRECISE_X);
            my_pos.Y = GetObjectData(ObjectData.PRECISE_Y);
            SetTargetById(target_id);
            target_pos.X = GetObjectData(ObjectData.PRECISE_X);
            target_pos.Y = GetObjectData(ObjectData.PRECISE_Y);

            return CrossTiles(my_pos, target_pos, 200);
        }
    }
}
