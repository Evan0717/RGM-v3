using Exiled.API.Features;
using MultiBroadcast.API;
using RGM.API.Features;
using RGM.Modes.SubClass;
using System;
using static RGM.Variables.Variable;

namespace RGM.Patches
{
    public class WavePostfix
    {
        public static void Postfix(ReferenceHub player, ref bool __result)
        {
            try
            {
                if (!__result)
                    return;

                if (player.IsHost || player.IsDummy)
                {
                    __result = false;
                }
                else
                {
                    Player ply = Player.Get(player);

                    __result = !ply.IsDND() && (ply.IsDead || ply.IsNonePlayer());
                }
            }
            catch (Exception e)
            {
                Log.Error($"[WavePostfix] Exception: {e}");
                __result = false;
            }
        }
    }
}
