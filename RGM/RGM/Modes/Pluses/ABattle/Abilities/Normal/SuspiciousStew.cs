using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
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
            EffectType.SpawnProtected
        };

        EffectType Effect = Tools.GetRandomValue(Effects.Where(x => !ignoredEffect.Contains(x)).ToList());
        byte Intensity = (byte)Random.Range(1, Random.Range(12, Random.Range(48, Random.Range(64, Random.Range(100, 255)))));

        Owner.EnableEffect(Effect, Intensity);
        Owner.RemoveAbility(this);
        Owner.AddAbility(AbilityType.DUMMY_COLDSTEW);
    }

    public override void OnDisabled()
    {
    }
}
