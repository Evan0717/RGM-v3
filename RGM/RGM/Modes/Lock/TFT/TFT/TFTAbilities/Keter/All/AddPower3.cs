using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using System.Collections.Generic;

namespace DAONTFT.Core.TFT.Keter.All;

[TFTAbility("단련Ⅲ", "피해 증폭을 얻습니다. (+60%)", TFTAbilityLevel.Keter, TFTAbilityCategory.All, TFTAbilityPoint.Continuous, TFTAbilityType.AddPower3, "💪")]
public class AddPower3 : TFTAbility
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

        ev.DamageHandler.Damage += ev.DamageHandler.Damage * 0.6f;
    }
}
