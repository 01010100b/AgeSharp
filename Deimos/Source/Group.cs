using AgeSharp.Common;
using AgeSharp.Scripting.SharpParser;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AgeSharp.Scripting.SharpParser.Intrinsics;

namespace Deimos.Source
{
    [AgeType]
    internal struct Group
    {
        public Int Id;
        public Int Type;
        public Point Position;

        [AgeMethod]
        public void AddObject(Int object_id)
        {
            var search_state = GetSearchState();

            if (search_state.LocalTotal >= 240)
            {
                throw new AgeException("Group local list full");
            }

            AddObjectById(SearchSource.LOCAL, object_id);
            CreateGroup(search_state.LocalTotal, 1, Id);
            ModifyGroupFlag(true, Id);
            RemoveObjects("==", SearchSource.LOCAL, ObjectData.INDEX, search_state.LocalTotal);
        }

        [AgeMethod]
        public void RemoveObject(Int object_id)
        {
            var search_state = GetSearchState();

            if (search_state.LocalTotal >= 240)
            {
                throw new AgeException("Group local list full");
            }

            AddObjectById(SearchSource.LOCAL, object_id);
            CreateGroup(search_state.LocalTotal, 1, Id);
            ModifyGroupFlag(false, Id);
            RemoveObjects("==", SearchSource.LOCAL, ObjectData.INDEX, search_state.LocalTotal);
        }
    }
}
