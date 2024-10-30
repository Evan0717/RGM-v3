using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Rare;

[Ability("순교", "사망할 시 해당 지역에 점화된 수류탄을 떨굽니다.", AbilityCategory.Rare, AbilityType.RARE_MARTYRDOM)]
public class Martyrdom : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Died += OnDied;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Died -= OnDied;
    }

    public void OnDied(DiedEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, ev.Player);
        g.FuseTime = 3f;
        g.SpawnActive(ev.Player.Position, ev.Player);
    }
}
