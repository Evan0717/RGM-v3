using Exiled.API.Enums;
using LabApi.Features.Wrappers;
using MEC;
using RGM.API.Features;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
            if (Owner.HasAbility(AbilityType.SYNERGY_REFLECTEDLIGHT))
            {
                foreach (var player in PlayerManager.List)
                {
                    if (player == Owner || !player.IsAlive) continue;
                    if (!HitboxIdentity.IsEnemy(player.ReferenceHub, Owner.ReferenceHub)) continue;

                    lightSource.Position = Owner.Position;

                    if (player.IsLookingAt(Owner, fov: 5))
                    {
                        Hitmarker.SendHitmarkerDirectly(Owner.ReferenceHub, 2f);
                        player.Hit(Owner, player.AbilityCount(AbilityType.NORMAL_SHELL) + 1);
                        player.EnableEffect(EffectType.Burned, 1, 10f);
                        player.EnableEffect(EffectType.Flashed, 1, 1.5f);
                        player.AddHint("따가움", "<b><color=#FFFF00>불타는 안구</color></b>");
                    }

                    else if (player.IsLookingAt(Owner, fov: 30))
                    {
                        Hitmarker.SendHitmarkerDirectly(Owner.ReferenceHub, 0.8f);
                        player.EnableEffect(EffectType.Flashed, 1, 1.5f);
                    }
                }

                yield return Timing.WaitForOneFrame;
            }

            else
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
}
