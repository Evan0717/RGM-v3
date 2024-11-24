using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features.Serializable;
using MapEditorReborn.API.Features;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Synergy;

[RequiresAbility(AbilityType.LEGEND_FLASHLIGHT, AbilityType.NORMAL_TORCH)]
[Ability("광휘", "<플래시라이트, 횃불> 당신을 쳐다보는 눈은 멀어버릴 것입니다.", AbilityCategory.Synergy, AbilityType.SYNERGY_GLORY)]
public class Glory : Ability
{
    CoroutineHandle _radiation;

    public override void OnEnabled()
    {
        _radiation = Timing.RunCoroutine(Radiation());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_radiation);
    }

    public IEnumerator<float> Radiation()
    {
        LightSourceSerializable LightSource = new LightSourceSerializable("#FFD700", 100, 20, true);
        LightSourceObject Light = ObjectSpawner.SpawnLightSource(LightSource, Vector3.zero);

        while (true)
        {
            foreach (var player in Player.List)
            {
                if (Tools.TryGetLookPlayer(player, 45f, out Player target))
                {
                    if (Owner == target && Owner.LeadingTeam != target.LeadingTeam)
                    {
                        Light.Position = Owner.Position;

                        Hitmarker.SendHitmarkerDirectly(Owner.ReferenceHub, 0.8f);
                        player.EnableEffect(EffectType.Flashed, 1, 1f);
                    }
                }
            }

            yield return Timing.WaitForSeconds(0.1f);
        }
    }
}
