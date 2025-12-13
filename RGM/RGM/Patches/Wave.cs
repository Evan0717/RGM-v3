using Exiled.API.Features;
using MultiBroadcast.API;
using static RGM.Variables.ServerManagers;

namespace RGM.Patches
{
    public class WavePostfix
    {
        public static void Postfix(ReferenceHub player, ref bool __result)
        {
            Player ply = Player.Get(player);

            __result = ply.IsDead || NonePlayers.Contains(ply);
        }
    }
}
