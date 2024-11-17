using AgeSharp.Scripting.SharpParser;
using Deimos.Source.Managers;

namespace Deimos.Source
{
    internal class Manager
    {
        [AgeMethod]
        public static void Initialize()
        {
            DiplomacyManager.Initialize();
            MilitaryManager.Initialize();
        }

        [AgeMethod]
        public static void Update()
        {
            DiplomacyManager.Update();
            MilitaryManager.Update();
        }
    }
}
