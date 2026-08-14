using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;

namespace RGM.Modes.Abilities.Unique.Scp049;

[Ability("메스", "자신의 공격에 추가 데미지 70이 적용됩니다.", AbilityCategory.Common, AbilityType.NORMAL_SCP049_MEDICALKNIFE, RoleAbility.Scp049)]
public class MedicalKnife : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
    }

    private void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Attacker != Owner || ev.Player == ev.Attacker)
            return;

        if (ev.DamageHandler.Type != DamageType.Scp049)
            return;

        ev.DamageHandler.Damage += 70f;
    }
}
