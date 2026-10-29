using Exiled.API.Enums;
using Exiled.API.Features;
using UnityEngine;

namespace RGM.Modes;

/// <summary>
/// 에코 배틀의 이동속도 증감 유틸.
/// MovementBoost는 양수, Slowness는 음수로 취급하여 하나의 값으로 합산한 뒤 다시 나누어 적용한다.
/// </summary>
public static class EchoSpeed
{
    /// <summary>
    /// 대상의 현재 이동속도(MovementBoost - Slowness)에서 <paramref name="reduction"/>만큼 차감해 적용한다.
    /// 계산 값이 양수면 MovementBoost, 음수면 Slowness, 0이면 둘 다 적용하지 않는다.
    /// <paramref name="reduction"/>이 음수이면 이전에 차감했던 이동속도를 되돌린다.
    /// </summary>
    public static void Reduce(Player target, int reduction)
    {
        if (target == null || !target.IsAlive || reduction == 0)
            return;

        int speed = GetIntensity(target, EffectType.MovementBoost)
                    - GetIntensity(target, EffectType.Slowness)
                    - reduction;

        SetIntensity(target, EffectType.MovementBoost, Mathf.Max(speed, 0));
        SetIntensity(target, EffectType.Slowness, Mathf.Max(-speed, 0));
    }

    static int GetIntensity(Player target, EffectType type)
    {
        var effect = target.GetEffect(type);
        return effect is { IsEnabled: true } ? effect.Intensity : 0;
    }

    static void SetIntensity(Player target, EffectType type, int intensity)
    {
        if (intensity <= 0)
        {
            target.DisableEffect(type);
            return;
        }

        // 이미 적용 중인 효과는 기존 지속 시간을 유지한다.
        var effect = target.GetEffect(type);
        float duration = effect is { IsEnabled: true } ? effect.Duration : 0f;
        target.EnableEffect(type, (byte)Mathf.Min(intensity, byte.MaxValue), duration);
    }
}
