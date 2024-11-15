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

namespace RGM.Modes.Abilities.Epic;

[Ability("수어사이드 봄버맨", "사망할 경우 즉시 폭발합니다.", AbilityCategory.Epic, AbilityType.EPIC_SUICIDEBOMBER)]
public class SuicideBomber : Ability
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

        var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, Owner);
        g.FuseTime = 0.1f;
        g.SpawnActive(Tools.GetRandomValue(Player.List.ToList().Where(x => x.IsAlive && x.Role.Team != Owner.Role.Team && Owner != x).ToList()).Position, Owner);
    }
}