using Exiled.Events.EventArgs.Player;
using MEC;

namespace RGM.Modes.Abilities.Rare;

[Ability("매복자", "10% 확률로 공격에 100% 추가 데미지를 입히는 치명타가 적용됩니다.", AbilityCategory.Rare, AbilityType.RARE_AMBUSH)]
public class Ambush : Ability
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
        if (ev.Attacker == null || ev.Attacker != Owner || !HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub))
            return;

        if (UnityEngine.Random.Range(1, 11) == 1)
        {
            ev.DamageHandler.Damage *= 2;

            Timing.CallDelayed(Timing.WaitForOneFrame, () =>
            {
                ev.Attacker.ShowHitMarker(2);
            });
        }
    }
}
