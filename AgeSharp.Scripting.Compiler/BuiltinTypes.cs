using AgeSharp.Scripting.Language.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler
{
    public static class BuiltinTypes
    {
        public static CompoundType SearchState { get; }

        static BuiltinTypes()
        {
            SearchState = new("SearchState");
            SearchState.AddField(new("LocalTotal", PrimitiveType.Int));
            SearchState.AddField(new("LocalLast", PrimitiveType.Int));
            SearchState.AddField(new("RemoteTotal", PrimitiveType.Int));
            SearchState.AddField(new("RemoteLast", PrimitiveType.Int));
        }
    }
}
