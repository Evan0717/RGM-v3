using System.Collections.Generic;
using System.Linq;
using LabApi.Events.Arguments.ServerEvents;
using MEC;
using SecretAPI.Features.UserSettings;

namespace RGM.UserSettings;

public static partial class ModeSettingManager
{
    public static readonly CustomHeader Header = new( "<b>모드별 설정</b>");
    public static readonly IEnumerable<CustomSetting> Settings = [];

    public static void Init() 
        => LabApi.Events.Handlers.ServerEvents.RoundStarting += OnRoundStarting;

    private static void OnRoundStarting(RoundStartingEventArgs e)
    {
        if (!Settings.Any()) return;
        Timing.CallDelayed(0.2f, () =>
        {
            CustomSetting.Register(Settings);
            CustomSetting.ResyncServer();
        });
    }
}