using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.DataBases;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.Scp173;

[Ability("신기루", "데미지를 입을 때 5% 확률로 1.25초 동안 투명화가 됩니다.", AbilityCategory.Scp173, AbilityType.SCP173_MIRAGE)]
public class Mirage : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
    }

    public void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        if (UnityEngine.Random.Range(1, 21) == 1)
        {
            Owner.EnableEffect(EffectType.Invisible, 1, 1.25f * Owner.AbilityCount(AbilityType.SCP173_MIRAGE));
        }
    }
}
