using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using UnityEngine;

namespace RGM.Modes.Abilities.Normal;

[Ability("단련", "공격력이 12% 추가됩니다.", AbilityCategory.Common, AbilityType.NORMAL_TRAINING)]
public class Training : Ability
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
        if (ev.Attacker != Owner || ev.Attacker.Role.Type == RoleTypeId.Scp173)
            return;

        ev.DamageHandler.Damage += ev.DamageHandler.Damage * 0.12f;
    }
}
