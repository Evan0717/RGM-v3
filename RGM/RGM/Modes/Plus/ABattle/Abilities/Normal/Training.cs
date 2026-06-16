using System.Collections.Generic;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;

namespace RGM.Modes.Abilities.Normal;

[Ability("단련", "공격력이 11% 추가됩니다.", AbilityCategory.Common, AbilityType.NORMAL_TRAINING)]
public class Training : Ability
{
    List<RoleTypeId> ignoredRoles = new List<RoleTypeId>
    {
        RoleTypeId.Scp173,
        RoleTypeId.Scp049,
        RoleTypeId.Scp106
    };  

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
        if (ev.Attacker != Owner || ignoredRoles.Contains(ev.Attacker.Role))
            return;

        ev.DamageHandler.Damage += ev.DamageHandler.Damage * 0.11f;
    }
}
