using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using System.Collections.Generic;

namespace DAONTFT.Core.TFT.Euclid.All;

[TFTAbility("단련Ⅱ", "피해 증폭을 얻습니다. (+30%)", TFTAbilityLevel.Euclid, TFTAbilityCategory.All, TFTAbilityPoint.Continuous, TFTAbilityType.AddPower2, "💪")]
public class AddPower2 : TFTAbility
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

        ev.DamageHandler.Damage += ev.DamageHandler.Damage * 0.3f;
    }
}
