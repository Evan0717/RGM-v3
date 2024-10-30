using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Legend;

[Ability("마술사", "누군가에게 죽으면, 공격자의 모든 능력을 무시하고 영혼이 교체됩니다.", AbilityCategory.Legend, AbilityType.LEGEND_MAGICIAN)]
public class Magician : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Dying += OnDying;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Dying -= OnDying;
    }

    public void OnDying(DyingEventArgs ev)
    {
        if (ev.Player != Owner || ABattle.Instance.IsLifeUsed[Owner])
            return;

        if (ev.Attacker != null)
            return;

        ev.IsAllowed = false;
        ev.Player.RemoveAllAbilities();

        ev.Player.Role.Set(ev.Attacker.Role, SpawnReason.ForceClass, RoleSpawnFlags.None);
        ev.Player.Health = ev.Attacker.Health;
        foreach (Item Item in ev.Attacker.Items)
            ev.Player.AddItem(Item.Type);

        foreach (Ability ability in ABattle.Instance.PlayerAbilities[ev.Attacker])
            ev.Player.AddAbility(ability.Data.AbilityType);

        while (ev.Attacker.IsAlive)
            ev.Attacker.Kill($"영혼이 교체되는 마술에 당했네요!");

        ABattle.Instance.IsLifeUsed[Owner] = true;

        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            ABattle.Instance.IsLifeUsed[Owner] = false;
        });
    }
}
