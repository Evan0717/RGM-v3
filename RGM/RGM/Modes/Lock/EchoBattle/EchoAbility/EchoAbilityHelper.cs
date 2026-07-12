using System;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.EchoBattle.EchoAbility;

/// <summary>
/// 부가 옵션(EchoAbility) 샘플 헬퍼.
/// 실제 부여/합산은 EchoStats.GenerateSubOptions / ApplySubOption에서 처리합니다.
/// 이후 개별 부가 옵션 클래스가 필요하면 이 디렉토리에 추가하세요.
/// </summary>
public static class EchoAbilityHelper
{
    public static string FormatOption(EchoSubOptionInstance option)
    {
        string name = EchoStats.GetSubOptionDisplayName(option.Type);
        return $"[G{option.Grade}] {name} +{option.Value:0.#}";
    }

    public static int GetMaxSlots(int level) => EchoStats.Clamp(level / 5, 0, 5);
}