using System.Collections.Generic;
using Exiled.API.Enums;
using MEC;
using RGM.API.Features;
using UnityEngine;
using LabApi.Features.Wrappers;

namespace RGM.Modes.Abilities.Synergy;

[RequiresAbility(AbilityType.LEGEND_FLASHLIGHT, AbilityType.NORMAL_TORCH)]
[Ability("광휘", "<플래시라이트, 횃불> 당신을 쳐다보는 눈은 멀어버릴 것입니다.", AbilityCategory.Synergy, AbilityType.SYNERGY_GLORY)]
public class Glory : Ability
{
    private CoroutineHandle _radiation;

    public override void OnEnabled()
    {
        _radiation = Timing.RunCoroutine(Radiation());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_radiation);
    }

    private IEnumerator<float> Radiation()
    {
        LightSourceToy lightSource = LightSourceToy.Create();
        lightSource.Color = Color.yellow;
        lightSource.Intensity = 50;
        lightSource.Range = 10;

        while (Owner.IsAlive)
        {
            foreach (var player in PlayerManager.List)
            {
                if (!player.TryGetLookPlayer(45f, out Exiled.API.Features.Player target, out _)) continue;
                if (Owner != target || !HitboxIdentity.IsEnemy(player.ReferenceHub, target.ReferenceHub)) continue;
                lightSource.Position = Owner.Position;

                Hitmarker.SendHitmarkerDirectly(Owner.ReferenceHub, 0.8f);
                player.EnableEffect(EffectType.Flashed, 1, 1.5f);
            }

            yield return Timing.WaitForOneFrame;
        }
    }
}
