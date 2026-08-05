using Exiled.API.Features;

using RGM.API.Features;
using System;

namespace RGM.Patches
{
    public class WavePatch
    {
        public static void Postfix(ReferenceHub player, ref bool __result)
        {
            try
            {
                if (player.IsHost || player.IsDummy)
                    __result = false;
                else
                {
                    Player ply = Player.Get(player);

                    if (ply == null) { __result = false; return; }

                    __result = !ply.IsDND() && ply.IsDead;
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
