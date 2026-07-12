using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using MEC;

namespace RGM.Modes.Abilities.Epic;

[Ability("신성방어", "모든 디버프에 면역을 가집니다.", AbilityCategory.Epic, AbilityType.EPIC_HOLYPROTECTION)]
public class HolyProtection : Ability
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

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.ReceivingEffect += OnReceivingEffect;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.ReceivingEffect -= OnReceivingEffect;
    }
    public void OnReceivingEffect(ReceivingEffectEventArgs ev)
    {
        if (ev.Player != Owner) return;

        var effectType = ev.Effect.GetEffectType();

        if (!KeptBuffs.Contains(effectType))
        {
            Timing.CallDelayed(Timing.WaitForOneFrame, () =>
            {
                ev.Player.DisableEffect(effectType);
            });
        }
    }
}