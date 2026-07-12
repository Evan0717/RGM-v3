using Exiled.Events.EventArgs.Player;
using MEC;

namespace DAONTFT.Core.TFT.Euclid.Human;

[TFTAbility("매복자", "21% 확률로 100% 추가 피해를 줍니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Human, TFTAbilityPoint.Continuous, TFTAbilityType.Ambush, "🌿")]
public class Ambush : TFTAbility
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

        if (UnityEngine.Random.Range(1, 101) <= 21)
        {
            ev.DamageHandler.Damage *= 2;

            Timing.CallDelayed(Timing.WaitForOneFrame, () =>
            {
                ev.Attacker.ShowHitMarker(2);
            });
        }
    }
}
