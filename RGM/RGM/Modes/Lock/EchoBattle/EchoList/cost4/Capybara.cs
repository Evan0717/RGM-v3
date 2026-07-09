using MEC;
using System.Collections.Generic;

namespace RGM.RGM.Modes.Lock.EchoBattle.EchoList.cost4;

[Echo("카피바라", "사용 시 20초간 최대 체력의 3%씩 회복, 재사용 대기시간 60초", EchoType.Capybara, EchoCost.Cost4, EchoMainStatType.HpPercent, "🦫")]
public class Capybara : EchoActiveAbility
{
    public override float Duration => 20f;
    public override float Cooldown => 60f;
    public override string ActiveDescription => "20초간 초당 최대 HP 3% 회복";

    protected override void OnActiveUsed()
    {
        Timing.RunCoroutine(HealRoutine(), $"EchoCapybara_{Owner.UserId}");
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines($"EchoCapybara_{Owner?.UserId}");
        base.OnDisabled();
    }

    IEnumerator<float> HealRoutine()
    {
        for (int i = 0; i < (int)Duration; i++)
        {
            if (Owner == null || !Owner.IsAlive)
                yield break;

            float heal = Owner.MaxHealth * 0.03f;
            Owner.Health = System.Math.Min(Owner.Health + heal, Owner.MaxHealth);
            yield return Timing.WaitForSeconds(1f);
        }
    }
}