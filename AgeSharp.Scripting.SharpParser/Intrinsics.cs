using AgeSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.SharpParser
{
    public static class Intrinsics
    {
        public static Int GetConstant(string constant) => throw new Exception();
        public static void ChatDataToSelf(string message, Int data) => throw new Exception();
        public static Int GetPreciseTime(Int previous) => throw new Exception();
        public static Int GetStrategicNumber(StrategicNumber sn) => throw new Exception();
        public static Int SetStrategicNumber(StrategicNumber sn, Int value) => throw new Exception();
        public static Int GetStrategicNumber(Int sn) => throw new Exception();
        public static void SetStrategicNumber(Int sn, Int value) => throw new Exception();
        public static void XsScriptCall(string script) => throw new Exception();

        // Math
        public static Bool Equals(Int a, Int b) => throw new Exception();
        public static Bool NotEquals(Int a, Int b) => throw new Exception();
        public static Bool LessThan(Int a, Int b) => throw new Exception();
        public static Bool GreaterThan(Int a, Int b) => throw new Exception();
        public static Bool LessThanOrEquals(Int a, Int b) => throw new Exception();
        public static Bool GreaterThanOrEquals(Int a, Int b) => throw new Exception();
        public static Int Add(Int a, Int b) => throw new Exception();
        public static Int Sub(Int a, Int b) => throw new Exception();
        public static Int Increment(Int a) => throw new Exception();
        public static Int Decrement(Int a) => throw new Exception();
        public static Int Mul(Int a, Int b) => throw new Exception();
        public static Int Div(Int a, Int b) => throw new Exception();
        public static Int Mod(Int a, Int b) => throw new Exception();
        public static Int Max(Int a, Int b) => throw new Exception();
        public static Int Min(Int a, Int b) => throw new Exception();
        public static Bool BoolEquals(Bool a, Bool b) => throw new Exception();
        public static Bool BoolNotEquals(Bool a, Bool b) => throw new Exception();
        public static Bool And(Bool a, Bool b) => throw new Exception();
        public static Bool Or(Bool a, Bool b) => throw new Exception();
        public static Bool Not(Bool a) => throw new Exception();
        public static Int BitwiseAnd(Int a, Int b) => throw new Exception();
        public static Int BitwiseOr(Int a, Int b) => throw new Exception();
        public static Int BitwiseNot(Int a) => throw new Exception();
        public static Int BitwiseXor(Int a, Int b) => throw new Exception();

        // DUC
        public static void FullResetSearch() => throw new Exception();
        public static void ResetFilters() => throw new Exception();
        public static void ResetSearch(Bool local_index, Bool local_list, Bool remote_index, Bool remote_list) => throw new Exception();
        public static void FilterInclude(CmdId cmdid, UnitAction action, UnitOrder order, Int mainland) => throw new Exception();
        public static void FilterExclude(CmdId cmdid, UnitAction action, UnitOrder order, UnitClass class_id) => throw new Exception();
        public static void FilterDistance(Int min, Int max) => throw new Exception();
        public static void FindLocal(Int id, Int count) => throw new Exception();
        public static void FindRemote(Int player, Int id, Int count) => throw new Exception();
        public static void AddObjectById(SearchSource search_source, Int id) => throw new Exception();
        public static void SetTargetById(Int id) => throw new Exception();
        public static SearchState GetSearchState() => throw new Exception();
        public static void SetTargetObject(SearchSource search_source, Int index) => throw new Exception();
        public static void SetTargetPoint(Point point) => throw new Exception();
        public static Int GetObjectData(ObjectData data) => throw new Exception();
        public static Bool ObjectExists(Int id) => throw new Exception();
        public static void CleanSearch(SearchSource search_source, ObjectData object_data, SearchOrder search_order) => throw new Exception();
        public static void RemoveObjects(string comparison, SearchSource search_source, ObjectData data, Int value) => throw new Exception();
        public static void TargetObjects(Bool use_target_object, UnitAction action, UnitFormation formation, UnitStance stance) => throw new Exception();
        public static void TargetPoint(Point point, TargetPointAdjustment adjustment, UnitAction action, UnitFormation formation, UnitStance stance) => throw new Exception();
        public static void CreateGroup(Int index, Int count, Int group) => throw new Exception();
        public static void SetGroup(SearchSource search_source, Int group) => throw new Exception();
        public static Int GetGroupSize(Int group) => throw new Exception();
        public static void ModifyGroupFlag(Bool append, Int group) => throw new Exception();

        // Players
        public static Int GetPlayerFact(Int player, FactId fact, Int parameter) => throw new Exception();
        public static Int GetPlayerStance(Int player) => throw new Exception();

        // Points
        public static Point CrossTiles(Point a, Point b, Int value) => throw new Exception();
    }
}
