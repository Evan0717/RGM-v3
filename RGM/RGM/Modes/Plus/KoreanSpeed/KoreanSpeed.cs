using System;
using HarmonyLib;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp049;
using RGM.API.Features;
using RGM.Modes.Patches;

namespace RGM.Modes;

[Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.KoreanSpeed)]
public class KoreanSpeed : Mode
{
    public override string Name => "한국인이 좋아하는 속도";
    public override string Description => "누군가가 사망할 때마다 모두의 속도가 증가합니다.";

    public override string Detail =>
        "<b><color=#FB00FF>슈</color><color=#D200D5>우</color><color=#A901AB>우</color><color=#800282>우</color><color=#570358>웅</color><color=#2E042E>화</color></b>";

    public override string Color => "5882FA";

    public static KoreanSpeed Instance;

    private ScpFeatures _scpFeatures;

    private static Harmony _harmony = new($"Harmony - {DateTime.Now.Ticks} - KoreanSpeed");

    public override void OnDisabled()
    {
        SpeedStore.Disable();
        PlayerFeatures.DeActivate();
        SpeedStore.Disable();
        ScpFeatures.Start -= AddPatches;
        RemovePatches();
        _scpFeatures = null;
        ScpFeatures.Start -= AddPatches;
    }

    public override void OnEnabled()
    {
        SpeedStore.Ignition();
        PlayerFeatures.Activate();
        ScpFeatures.Start += AddPatches;

        _scpFeatures = new ScpFeatures();
        _scpFeatures.Run();
    }

    ///<summary>    
    /// Harmony 패치를 활성화하기 위한 Event 호환 매서드입니다.
    /// </summary>
    private static void AddPatches(object sender, System.EventArgs e)
    {
        Scp049Patch();
        LogicPatch();
    }

    ///<summary>
    /// 이 내부모듈의 harmony 패치를 제거합니다.
    /// </summary>
    private static void RemovePatches()
    {
        _harmony?.UnpatchAll();
        _harmony = null;
    }

    ///<summary>
    /// SCP-049 관련 Harmony 패치입니다.
    /// </summary>
    private static void Scp049Patch()
    {
            if (!PlayerManager.List.Exists(player =>
                    player.IsAlive && !player.IsNonePlayer() && !player.IsNPC &&
                    player.Role.Type == RoleTypeId.Scp049)) return;

            _harmony.Patch(AccessTools.PropertyGetter(
                    typeof(Scp049ResurrectAbility), nameof(Scp049ResurrectAbility.Duration)),
                postfix: new HarmonyMethod(typeof(ScpPatch), nameof(ScpPatch.Scp049Postfix)));
    }

    ///<summary>
    ///시스템 로직 관련 Harmony 패치입니다.
    /// </summary>
    private static void LogicPatch()
    {
        _harmony.Patch(AccessTools.PropertyGetter(
                typeof(CustomPlayerEffects.Scp1853), nameof(CustomPlayerEffects.Scp1853.MaxIntensity)),
            postfix: new HarmonyMethod(typeof(SystemPatch), nameof(SystemPatch.Scp1853MaxIntensityPostfix)));
    }

}