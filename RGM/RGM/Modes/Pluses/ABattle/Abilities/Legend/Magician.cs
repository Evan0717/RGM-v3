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

using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Legend;

[Ability("마술사", "사망 시, 공격자의 현재 또는 최대 체력이 자신보다 더 낮았다면, 공격자의 모든 능력을 무시하고 영혼이 교체됩니다.\n피해를 입으면 1/5 비례하여 최대 체력이 늘어납니다.", AbilityCategory.Legend, AbilityType.LEGEND_MAGICIAN)]
public class Magician : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Hurt += OnHurt;
        Exiled.Events.Handlers.Player.Dying += OnDying;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurt -= OnHurt;
        Exiled.Events.Handlers.Player.Dying -= OnDying;
    }

    public void OnHurt(HurtEventArgs ev)
    {
        if (ev.Player != Owner || !HitboxIdentity.IsEnemy(ev.Player.ReferenceHub, ev.Attacker.ReferenceHub))
            return;

        float add = ev.DamageHandler.Damage / 5;
        ev.Player.MaxHealth += add;
        ev.Player.Health += add;
    }

    public void OnDying(DyingEventArgs ev)
    {
        if (ev.Player != Owner || ABattle.Instance.IsLifeUsed[Owner] || ev.Attacker == null || (ev.Attacker.Health > Owner.Health || ev.Attacker.MaxHealth > Owner.MaxHealth))
            return;

        ev.IsAllowed = false;
        ev.Player.RemoveAllAbilities();

        ev.Player.Role.Set(ev.Attacker.Role, SpawnReason.ForceClass, RoleSpawnFlags.None);
        ev.Player.Health = ev.Attacker.Health;
        foreach (Item Item in ev.Attacker.Items)
            ev.Player.AddItem(Item.Type);

        foreach (Ability ability in ABattle.Instance.PlayerAbilities[ev.Attacker])
            ev.Player.AddAbility(ability.Data.AbilityType);

        if (GodModePlayers.Contains(ev.Attacker))
            GodModePlayers.Remove(ev.Attacker);

        ev.Attacker.RemoveAllAbilities();
        ev.Attacker.Kill($"영혼이 교체되는 마술에 당했네요!");

        ABattle.Instance.IsLifeUsed[Owner] = true;

        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            ABattle.Instance.IsLifeUsed[Owner] = false;
        });
    }
}
