using AgeSharp.Scripting.Language.Types;

namespace AgeSharp.Scripting.Compiler
{
    public static class BuiltinTypes
    {
        public static CompoundType Point { get; }
        public static CompoundType SearchState { get; }

        static BuiltinTypes()
        {
            Point = new("Point");
            Point.AddField(new("X", PrimitiveType.Int));
            Point.AddField(new("Y", PrimitiveType.Int));

            SearchState = new("SearchState");
            SearchState.AddField(new("LocalTotal", PrimitiveType.Int));
            SearchState.AddField(new("LocalLast", PrimitiveType.Int));
            SearchState.AddField(new("RemoteTotal", PrimitiveType.Int));
            SearchState.AddField(new("RemoteLast", PrimitiveType.Int));
        }
    }
}
