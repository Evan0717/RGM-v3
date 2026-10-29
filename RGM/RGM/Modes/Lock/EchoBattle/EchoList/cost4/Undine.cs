using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using RGM.API.Features;
using System.Linq;
using UnityEngine;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.EchoBattle.EchoList.cost4;

[Echo("운디네", "사용 시 주변 15m 내에 있는 모든 상대 2초간 적응형 이동속도 -60 및 Sinkhole 효과 부여, 재사용 대기시간 40초", EchoType.Undine, EchoCost.Cost4, EchoMainStatType.HpPercent, "💧")]
public class Undine : EchoActiveAbility
{
    private const int SpeedReduction = 60;

    public override float Duration => 2f;
    public override float Cooldown => 40f;
    public override string ActiveDescription => "주변 15m 적 2초간 적응형 이동속도 -60 및 Sinkhole 효과 부여";

    protected override void OnActiveUsed()
    {
        foreach (var target in Player.List.Where(p =>
                     p != Owner
                     && p.IsAlive
                     && HitboxIdentity.IsEnemy(Owner.ReferenceHub, p.ReferenceHub)
                     && Vector3.Distance(Owner.Position, p.Position) <= 15f))
        {
            ApplySpeedReduction(target);
            target.AddEffect(EffectType.SinkHole, 1, Duration);
        }
    }

    /// <summary>
    /// 대상의 현재 이동속도(MovementBoost - Slowness)에서 60을 차감해 적용하고,
    /// 지속 시간이 끝나면 차감한 만큼 되돌린다.
    /// </summary>
    private void ApplySpeedReduction(Player target)
    {
        EchoSpeed.Reduce(target, SpeedReduction);

        Timing.CallDelayed(Duration, () => EchoSpeed.Reduce(target, -SpeedReduction));
    }
}