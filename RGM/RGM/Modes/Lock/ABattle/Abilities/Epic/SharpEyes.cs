using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Epic;

[Ability("샤프 아이즈", """
                   크리티컬 확률이 50% 증가합니다.
                   크리티컬 발동 시 50%p의 추가 피해를 입히며, 추가 획득 시 크리티컬 데미지가 75%p씩 증가합니다.
                   """, AbilityCategory.Epic, AbilityType.EPIC_SHARPEYES)]
public class SharpEyes : Ability
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
        // ApplyInstantKill 의 사망 피해(재시도 포함)는 일반 공격이 아니므로 크리티컬/히트마커를 적용하지 않는다.
        if (ApplyInstantKill.IsApplying ||
            ev.Attacker == null || 
            ev.Attacker != Owner || 
            !HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub))
            return;

        int count = Owner.AbilityCount(AbilityType.EPIC_SHARPEYES);
        int chance = Mathf.Min(100, 50 * count);

        if (Random.Range(1, 101) > chance)
            return;

        ev.DamageHandler.Damage *= 0.5f + 0.75f * count;

        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            ev.Attacker.ShowHitMarker(1.5f);
        });
    }
}
