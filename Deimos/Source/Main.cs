using AgeSharp.Scripting.SharpParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AgeSharp.Scripting.SharpParser.Intrinsics;

namespace Deimos.Source
{
    public class Main
    {
        [AgeGlobal]
        public static Int Tick;
        [AgeGlobal]
        public static Array<Int> Stuff = new(10);

        [AgeMethod(EntryPoint = true)]
        public static void EntryPoint()
        {
            if (Tick < 1)
            {
                Initialize();
                
                Group.CreateGroup();
                Group.CreateGroup();
                Group g;
                g = Group.GetGroup(0);
                g.Type = 17;
                Group.SetGroup(0, g);
                g = Group.GetGroup(1);
                g.Type = 23;
                Group.SetGroup(1, g);
            }

            Group group;
            Group group1;
            group = Group.GetGroup(0);
            group1 = Group.GetGroup(1);

            ChatDataToSelf("groups %d", Group.GetGroupCount());
            ChatGroupType(ref group);
            ChatGroupType(ref group1);

            Tick += 1;
        }

        [AgeMethod]
        private static void Initialize()
        {
            Group.Initialize();
        }

        [AgeMethod]
        private static void ChatGroupType(ref Group group)
        {
            ChatDataToSelf("group type %d", group.Type);
        }
    }
}
