using System.Collections.Generic;
using Exiled.API.Features;
using SecretAPI.Features.UserSettings;

namespace RGM.UserSettings;

public class SettingVariables
{
    public static readonly Dictionary<Player, CustomSetting> CurrentPlayerSettings = [];
    public static readonly Dictionary<ushort, IEnumerable<CustomSetting>> Settings = [];
    // -----------------------------------------------------------
    public static CustomHeader Setting { get; } = new("<b>랜덤게임모드</b>");
}