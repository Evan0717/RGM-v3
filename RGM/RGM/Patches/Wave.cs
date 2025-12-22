using Exiled.API.Features;
using MultiBroadcast.API;
using RGM.API.Features;
using RGM.Modes.SubClass;
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

                __result = UsersManager.UsersCache[ply.UserId][23] == "0" && (ply.IsDead || NonePlayer.Players.Contains(ply));
            }
        }
    }
}
