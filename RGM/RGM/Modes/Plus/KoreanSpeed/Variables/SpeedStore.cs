namespace RGM.Modes;

public class SpeedStore
{
    /// <summary>
    /// 모드 "한국인이 좋아하는 속도"에서 속도 관련 계산을 처리할 때 사용합니다.
    /// </summary>
    public static byte Count { get; set; }
    
    public static float ScpMultiplier { get; internal set; } = 0.1f;
    
    /// <summary>
    /// 모드의 활성화 유무를 따질 때 사용합니다.
    /// </summary>
    public static bool IsEnabled { get; private set; }
    
    public static void Clear() => Count = 0;

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
            if (Count - value < 0)
            {
                message = $"값이 너무 큽니다. 현재 횟수 {Count}보다 작거나 같게 하세요.";
                return false;
            }

            Count -= value;
            message = "해당 삭제 명령이 성공적으로 처리되었습니다.";
            PlayerEffects.AddEffects();
            return true;
    }
}