using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Epic;

[Ability("!!마쉬멜로우!!", """
                      즉시 마쉬멜로우맨이 됩니다. 
                      최대 체력이 400 증가하고, 기본 공격 데미지가 60 증가하며, 이동 속도가 20%p 증가합니다.
                      추가로, 피해량의 10% 만큼 회복하며, SCP 진영이 사용 시 버프 효과가 50% 감소합니다.
                      """,
    AbilityCategory.Epic, AbilityType.EPIC_MARSHMELLOW,
    RoleAbility.None, false, AbilityHolidayType.Halloween, isUnique: true)]
public class MarshMellow : Ability
{
    private CoroutineHandle _marshmallowCoroutine;
    
    public override void OnEnabled()
    {
        var isScpRole = Owner.IsScpRole();

        Owner.MaxHealth += isScpRole ? 200 : 400;
        Owner.Health += isScpRole ? 200 : 400;
        for (int i = 0; i < (isScpRole ? 2 : 4); i++)
        {
            Owner.AddAbility(AbilityType.NORMAL_SWIFT);
        }

        Exiled.Events.Handlers.Player.Hurting += OnHurting;
        
        _marshmallowCoroutine = Timing.RunCoroutine(MarshmallowCoroutine());
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;

        Timing.KillCoroutines(_marshmallowCoroutine);
    }

    private void OnHurting(HurtingEventArgs ev)
    {
        var isScpRole = Owner.IsScpRole();
        
        if (ev.Attacker != Owner || ev.Attacker == null) return;
        if (ev.DamageHandler.Type != DamageType.Marshmallow) return;

        ev.DamageHandler.Damage += isScpRole ? 30 : 60;
        ev.Attacker.Heal(ev.DamageHandler.Damage * (isScpRole ? 0.05f : 0.1f));
    }

    private IEnumerator<float> MarshmallowCoroutine()
    {
        while (true)
        {
            Owner.EnableEffect(EffectType.Marshmallow);

            yield return Timing.WaitForSeconds(0.2f);
        }        
    }
}
