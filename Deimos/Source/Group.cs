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

        [AgeMethod]
        public void DoStuff()
        {
            ChatDataToSelf("id %d", Id);
        }
    }
}
