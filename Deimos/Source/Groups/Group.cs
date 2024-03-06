using AgeSharp.Common;
using AgeSharp.Scripting.SharpParser;
using Deimos.Source.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AgeSharp.Scripting.SharpParser.Intrinsics;

namespace Deimos.Source.Groups
{
    [AgeType]
    internal struct Group
    {
        public Int Id;
        public Int Type;
        private Int Count;

        [AgeMethod]
        public Int GetCount()
        {
            return Count;
        }

        [AgeMethod]
        public void AddObject(Int object_id)
        {
            var search_state = GetSearchState();

            if (search_state.LocalTotal >= Main.MAX_LOCAL_LIST)
            {
                throw new AgeException("Group local list full");
            }

            AddObjectById(SearchSource.LOCAL, object_id);
            DUC.RotateLocalSearchList(search_state.LocalTotal);
            CreateGroup(0, 1, Id);
            ModifyGroupFlag(true, Id);
            RemoveObjects("==", SearchSource.LOCAL, ObjectData.INDEX, 0);
            Count++;
        }

        [AgeMethod]
        public void RemoveObject(Int object_id)
        {
            var search_state = GetSearchState();

            if (search_state.LocalTotal >= Main.MAX_LOCAL_LIST)
            {
                throw new AgeException("Group local list full");
            }

            AddObjectById(SearchSource.LOCAL, object_id);
            DUC.RotateLocalSearchList(search_state.LocalTotal);
            CreateGroup(0, 1, Id);
            ModifyGroupFlag(false, Id);
            RemoveObjects("==", SearchSource.LOCAL, ObjectData.INDEX, 0);
            Count--;
        }

        [AgeMethod]
        public void FilterLocalSearchList()
        {
            RemoveObjects("!=", SearchSource.LOCAL, ObjectData.GROUP_FLAG, Id);
            var search_state = GetSearchState();
            Count = search_state.LocalTotal;
        }
    }
}
