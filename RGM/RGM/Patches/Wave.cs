using Exiled.API.Features;

using RGM.API.Features;
using RGM.Modes.SubClass;
using System;
using static RGM.Variables.Variable;

namespace RGM.Patches
{
    public class WavePatch
    {
        public static void Postfix(ReferenceHub player, ref bool __result)
        {
            try
            {
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
