using AgeSharp.Scripting.SharpParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AgeSharp.Scripting.SharpParser.Intrinsics;

namespace Deimos.Source.Controllers
{
    internal class ExterminationController
    {
        [AgeMethod]
        public static void Update(Group group)
        {
            var search_state = Group.SearchLocalGroupObjects(group.Id);
        }
    }
}
