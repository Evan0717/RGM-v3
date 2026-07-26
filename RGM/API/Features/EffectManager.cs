using System.Collections.Generic;
using Exiled.API.Enums;

namespace RGM.API.Features
{
    public static class EffectManager
    {
        private static readonly HashSet<EffectType> KeptBuffs =
        [
            EffectType.Scp1853,
            EffectType.Invigorated,
            EffectType.Invisible,
            EffectType.RainbowTaste,
            EffectType.BodyshotReduction,
            EffectType.DamageReduction,
            EffectType.MovementBoost,
            EffectType.Vitality,
            EffectType.SpawnProtected,
            EffectType.Ghostly,
            EffectType.SilentWalk,
            EffectType.Fade,
            EffectType.FocusedVision,
            EffectType.AnomalousRegeneration,
            EffectType.Scp1344,
            EffectType.Scp207,
            EffectType.AntiScp207,
            EffectType.Lightweight,
            EffectType.NightVision,
            EffectType.FogControl,
            EffectType.PitDeath,
            EffectType.Decontaminating
        ];

        public static bool IsKeptBuff(EffectType effectType) => KeptBuffs.Contains(effectType);
    }
}
