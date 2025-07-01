using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using MultiBroadcast.API;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Normal;

[Ability("수상한 스튜", "효과 세기가 랜덤인 영구 지속 무작위 효과를 하나 받습니다.", AbilityCategory.Common, AbilityType.NORMAL_SUSPICIOUSSTEW)]
public class SuspiciousStew : Ability
{
    public override void OnEnabled()
    {
        List<EffectType> Effects = Tools.EnumToList<EffectType>();
        List<EffectType> ignoredEffect = new List<EffectType>
        {
            EffectType.PocketCorroding,
            EffectType.PitDeath,
            EffectType.CardiacArrest,
            EffectType.Poisoned,
            EffectType.SpawnProtected,
            EffectType.Ensnared,
            EffectType.Flashed,
            EffectType.SeveredHands
        };

        EffectType Effect = Owner.HasAbility(AbilityType.EPIC_FOODRESEARCHER) ? Effects.Where(x => x.IsPositive()).GetRandomValue() : Effects.Where(x => !ignoredEffect.Contains(x)).GetRandomValue();
        byte Intensity = (byte)Random.Range(1, Random.Range(12, Random.Range(48, Random.Range(64, Random.Range(100, 255)))));

        Owner.EnableEffect(Effect, Intensity);
        Owner.AddHint("효과 알리미", $"<color=#D0FA58>{Effect}</color> 효과가 {Intensity}만큼 적용되는 중입니다.", 5);
        Owner.AddBroadcast(5, $"<size=20><color=#D0FA58>{Effect}</color> 효과가 {Intensity}만큼 적용되는 중입니다.</size>");
        Timing.CallDelayed(1, () =>
        {
            Owner.RemoveAbility(this);
        });
        Owner.AddAbility(AbilityType.DUMMY_COLDSTEW);
    }

    public override void OnDisabled()
    {
    }
}
