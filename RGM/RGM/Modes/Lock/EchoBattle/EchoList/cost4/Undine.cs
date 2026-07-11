using Exiled.API.Enums;
using Exiled.API.Features;
using RGM.API.Features;
using System.Linq;
using UnityEngine;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.EchoBattle.EchoList.cost4;

[Echo("운디네", "사용 시 주변 15m 내에 있는 모든 상대 2초간 50% 감속 및 Sinkhole 효과 부여, 재사용 대기시간 30초", EchoType.Undine, EchoCost.Cost4, EchoMainStatType.HpPercent, "💧")]
public class Undine : EchoActiveAbility
{
    public override float Duration => 2f;
    public override float Cooldown => 30f;
    public override string ActiveDescription => "주변 15m 적 2초간 50% 감속 및 Sinkhole 효과 부여";

    protected override void OnActiveUsed()
    {
        foreach (var target in Player.List.Where(p =>
                     p != Owner
                     && p.IsAlive
                     && HitboxIdentity.IsEnemy(Owner.ReferenceHub, p.ReferenceHub)
                     && Vector3.Distance(Owner.Position, p.Position) <= 15f))
        {
            target.AddEffect(EffectType.Slowness, 50, Duration);
            target.AddEffect(EffectType.SinkHole, 1, Duration);
        }
    }
}