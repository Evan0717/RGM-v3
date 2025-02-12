using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Normal;

[Ability("수상한 스튜", "적용 시간과 강도가 랜덤인 무작위 효과를 하나 받습니다.", AbilityCategory.Common, AbilityType.NORMAL_SUSPICIOUSSTEW)]
public class SuspiciousStew : Ability
{
    public override void OnEnabled()
    {
        List<EffectType> Effects = Tools.EnumToList<EffectType>();

        EffectType Effect = Tools.GetRandomValue(Effects.Where(x => x != EffectType.PocketCorroding).ToList());
        byte Intensity = (byte)Random.Range(1, Random.Range(12, Random.Range(48, Random.Range(64, Random.Range(100, 255)))));
        float Duration = Random.Range(1, Random.Range(12, Random.Range(24, Random.Range(48, 61))));

        Owner.EnableEffect(Effect, Intensity, Duration);
        Owner.ShowHint($"<color=#D0FA58>{Effect}</color> 효과가 {Intensity}만큼 {Duration}초 동안 적용되는 중입니다..", Duration);
    }

    public override void OnDisabled()
    {
    }
}
