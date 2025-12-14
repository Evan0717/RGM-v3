using Exiled.API.Features;
using MultiBroadcast.API;
using static RGM.Variables.Variable;

namespace RGM.Patches
{
    public class WavePostfix
    {
        public static void Postfix(ReferenceHub player, ref bool __result)
        {
            if (player.IsHost || player.IsDummy)
            {
                __result = false;
            }
            else
            {
                Player ply = Player.Get(player);

                __result = ply.IsDead || NonePlayers.Contains(ply);
            }
        }
    }
}
