using AgeSharp.Common;
using AgeSharp.Scripting.SharpParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AgeSharp.Scripting.SharpParser.Intrinsics;

namespace Deimos.Source.Managers
{
    internal class DiplomacyManager
    {
        [AgeGlobal]
        private static Array<Int> PlayerStances = new(8);

        [AgeMethod]
        public static Int GetMyPlayerNumber()
        {
            return GetConstant("my-player-number");
        }

        [AgeMethod]
        public static Int GetPlayerCount(Int stance)
        {
            Int count = 0;

            for (Int i = 0; i < PlayerStances.Length; i++)
            {
                if (PlayerStances[i] == stance)
                {
                    count++;
                }
            }

            return count;
        }

        [AgeMethod]
        public static Int GetPlayer(Int stance, Int index)
        {
            Int count = 0;

            for (Int i = 0; i < PlayerStances.Length; i++)
            {
                if (PlayerStances[i] == stance)
                {
                    if (count == index)
                    {
                        return i + 1;
                    }

                    count++;
                }
            }

            throw new AgeException("Invalid index.");
        }

        [AgeMethod]
        public static void Initialize()
        {

        }

        [AgeMethod]
        public static void Update()
        {
            var me = GetMyPlayerNumber();

            for (Int player = 1; player <= 8; player++)
            {
                var index = player - 1;
                PlayerStances[index] = -1;
                var ingame = GetPlayerFact(player, FactId.PLAYER_IN_GAME, 0);

                if (ingame != 1 || player == me)
                {
                    continue;
                }

                var stance = GetPlayerStance(player);
                PlayerStances[index] = stance;
            }
        }
    }
}
