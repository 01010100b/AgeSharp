using AgeSharp.Common;
using AgeSharp.Scripting.SharpParser;
using static AgeSharp.Scripting.SharpParser.Intrinsics;

namespace Deimos.Source.Managers
{
    internal class DiplomacyManager
    {
        [AgeGlobal]
        private static readonly Array<Int> PlayerStances = new(8);

        [AgeMethod]
        public static Int GetMyPlayerNumber()
        {
            return GetConstant("my-player-number");
        }

        [AgeMethod]
        public static Int GetAllyCount()
        {
            return GetPlayerCount(PlayerStance.ALLY);
        }

        [AgeMethod]
        public static Int GetEnemyCount()
        {
            return GetPlayerCount(PlayerStance.ENEMY);
        }

        [AgeMethod]
        public static Int GetPlayerCount(PlayerStance stance)
        {
            Int count = 0;

            for (Int i = 0; i < PlayerStances.Length; i++)
            {
                if (PlayerStances[i] == (int)stance)
                {
                    count++;
                }
            }

            return count;
        }

        [AgeMethod]
        public static Int GetAlly(Int index)
        {
            return GetPlayer(PlayerStance.ALLY, index);
        }

        [AgeMethod]
        public static Int GetEnemy(Int index)
        {
            return GetPlayer(PlayerStance.ENEMY, index);
        }

        [AgeMethod]
        public static Int GetPlayer(PlayerStance stance, Int index)
        {
            Int count = 0;

            for (Int i = 0; i < PlayerStances.Length; i++)
            {
                if (PlayerStances[i] == (int)stance)
                {
                    if (count == index)
                    {
                        return i + 1;
                    }

                    count++;
                }
            }

            throw new AgeException("Invalid player index");
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
