using AgeSharp.Scripting.SharpParser;
using Deimos.Source.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deimos.Source
{
    internal class Controller
    {
        [AgeMethod]
        public static void Initialize()
        {

        }

        [AgeMethod]
        public static void Update()
        {
            var group_count = Group.GetGroupCount();

            for (Int i = 0; i < group_count; i++)
            {
                var group = Group.GetGroup(i);

                if (!ShouldUpdate(group.Id, group.UpdateRate))
                {
                    continue;
                }

                if (group.Type == Group.TYPE_EXTERMINATION)
                {
                    ExterminationController.Update(group);
                }
            }
        }

        [AgeMethod]
        private static Bool ShouldUpdate(Int id, Int update_rate)
        {
            if (update_rate <= 0)
            {
                return false;
            }

            return Main.Tick % update_rate == id % update_rate;
        }
    }
}
