using Exiled.API.Enums;
using Exiled.API.Features;
using RGM.API.Features;
using System.Linq;
using UnityEngine;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.EchoBattle.EchoList.cost4;

[Echo("운디네", "사용 시 주변 15m 내에 있는 모든 상대 3초간 40% 감속, 재사용 대기시간 60초", EchoType.Undine, EchoCost.Cost4, EchoMainStatType.HpPercent, "💧")]
public class Undine : EchoActiveAbility
{
    public override float Duration => 3f;
    public override float Cooldown => 60f;
    public override string ActiveDescription => "주변 15m 적 3초간 40% 감속";

    protected override void OnActiveUsed()
    {
        foreach (var target in Player.List.Where(p =>
                     p != Owner
                     && p.IsAlive
                     && HitboxIdentity.IsEnemy(Owner.ReferenceHub, p.ReferenceHub)
                     && Vector3.Distance(Owner.Position, p.Position) <= 15f))
        {
            target.AddEffect(EffectType.Slowness, 40, Duration);
        }
    }
}