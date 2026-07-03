using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using System.Collections.Generic;

namespace DAONTFT.Core.TFT.Safe.All;

[TFTAbility("단련 · 입문", "피해 증폭을 얻습니다. (+12%)", TFTAbilityLevel.Safe, TFTAbilityCategory.All, TFTAbilityPoint.Continuous, TFTAbilityType.AddPower1, "💪")]
public class AddPower1 : TFTAbility
{
    List<RoleTypeId> ignoredRoles = new List<RoleTypeId>
    {
        RoleTypeId.Scp173,
        RoleTypeId.Scp049
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

        ev.DamageHandler.Damage += ev.DamageHandler.Damage * 0.12f;
    }
}
