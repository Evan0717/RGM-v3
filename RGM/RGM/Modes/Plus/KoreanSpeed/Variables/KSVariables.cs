using System;

namespace RGM.Modes;

public class SpeedStore
{
    public static byte Count { get; set; }
    public static bool IsEnabled { get; private set; }

    public static void Clear()
    {
        Count = 0;
    }

    public static void Ignition()
    {
        Count = 0;
        IsEnabled = true;
    }

    public static void Disable()
    {
        Count = 0;
        IsEnabled = false;
    }

    public static bool TryRemove(byte value,
        out string message)
    {
        try
        {
            if (Count - value < 0)
            {
                message = $"값이 너무 큽니다. 현재 횟수 {Count}보다 작거나 같게 하세요.";
                return false;
            }

            Count -= value;
            message = "해당 삭제 명령이 성공적으로 처리되었습니다.";
            KoreanSpeed.AddEffects();
            return true;
        }
        catch (Exception e)
        {
            message = "알 수 없는 값입니다.";
            return false;
        }
    }
}