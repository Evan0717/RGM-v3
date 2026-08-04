using Exiled.Events.EventArgs.Player;
using MEC;

namespace RGM.Modes.Abilities.Epic;

[Ability("샤프 아이즈", "자신의 모든 공격에 크리티컬이 적용됩니다. 추가로, 능력 갯수 만큼 크리티컬 데미지가 80% 증가합니다.", AbilityCategory.Epic, AbilityType.EPIC_SHARPEYES)]
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

    public void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Attacker == null || 
            ev.Attacker != Owner || 
            !HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub))
            return;

        ev.DamageHandler.Damage *= 1 + 0.8f * Owner.AbilityCount(AbilityType.EPIC_SHARPEYES);

        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            ev.Attacker.ShowHitMarker(1.6f);
        });
    }
}
