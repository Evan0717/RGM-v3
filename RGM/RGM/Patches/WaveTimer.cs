using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Exiled.API.Features;
using HarmonyLib;
using Respawning;
using Respawning.Waves;
using UnityEngine;

namespace RGM.Patches;

/// <summary>
/// Wavetimer 전역 패치:
/// 1) Respawn Token 공용 풀 제한 제거 (소모는 유지 → DeadmanSwitch 정상 동작)
/// 2) Primary Wave Respawn Time = Random.Range(min, max) (라운드 시작 시 MTF/CHAOS 공통, 이후 개별)
/// 3) 지원 인원 1명당 지원 시간 +3초 (기존 +10초)
/// 4) MiniWave는 지원 인원당 시간 증가 없음
/// 5) MiniWave 대기시간 고정
/// </summary>
public static class WaveTimerPatch
{
    private const float PrimarySecondsPerSpawn = 3f;
    private const float MiniWaveSecondsPerSpawn = 0f;
    private const float MiniWaveSpawnInterval = 100f;
    private const int PrimaryIntervalMin = 205;
    private const int PrimaryIntervalMax = 255;

    private static bool _useSharedPrimaryInterval = true;
    private static float _sharedPrimaryInterval;
    private static int _sharedPrimaryIntervalFrame = -1;

    public static void Apply(Harmony harmony)
    {
        try
        {
            harmony.Patch(
                AccessTools.Method(typeof(WaveTimer), nameof(WaveTimer.Reset)),
                prefix: new HarmonyMethod(typeof(WaveTimerPatch), nameof(WaveTimerResetPrefix)));

            // 토큰은 정상 소모하되, 라운드 공용 추가 토큰 풀(AvailableRespawnsLeft) 제한만 해제합니다.
            harmony.Patch(
                AccessTools.PropertySetter(typeof(RespawnTokensManager), nameof(RespawnTokensManager.AvailableRespawnsLeft)),
                prefix: new HarmonyMethod(typeof(WaveTimerPatch), nameof(AvailableRespawnsLeftSetPrefix)));

            foreach (Type miniWaveType in new[]
                     {
                         typeof(MiniWaveBase<NtfSpawnWave, ChaosMiniWave>),
                         typeof(MiniWaveBase<ChaosSpawnWave, NtfMiniWave>)
                     })
            {
                MethodInfo miniInterval = AccessTools.DeclaredMethod(miniWaveType, "get_InitialSpawnInterval");
                if (miniInterval != null)
                {
                    harmony.Patch(
                        miniInterval,
                        postfix: new HarmonyMethod(typeof(WaveTimerPatch), nameof(MiniWaveInitialSpawnIntervalPostfix)));
                }

                MethodInfo miniOnAnyWaveSpawned = AccessTools.DeclaredMethod(miniWaveType, nameof(SpawnableWaveBase.OnAnyWaveSpawned));
                if (miniOnAnyWaveSpawned != null)
                {
                    harmony.Patch(
                        miniOnAnyWaveSpawned,
                        transpiler: new HarmonyMethod(typeof(WaveTimerPatch), nameof(MiniWaveAvailabilityTranspiler)));
                }
            }

            ApplyWaveFieldOverrides();
            Log.Info("[WaveTimerPatch] Applied.");
        }
        catch (Exception e)
        {
            Log.Error($"[WaveTimerPatch] Failed to apply: {e}");
        }
    }

    /// <summary>
    /// WaitingForPlayers: 다음 라운드용 공통 Respawn Time 모드를 다시 켭니다.
    /// </summary>
    public static void ApplyWaveFieldOverrides()
    {
        try
        {
            RespawnTokensManager.AvailableRespawnsLeft = int.MaxValue;
            _useSharedPrimaryInterval = true;

            ApplyPrimaryIntervalToAll(GetOrRollSharedPrimaryInterval(forceReroll: true), includeSpawnInterval: true);
        }
        catch (Exception e)
        {
            Log.Error($"[WaveTimerPatch] ApplyWaveFieldOverrides Exception: {e}");
        }
    }

    /// <summary>
    /// RoundStarted: 공통 Respawn Time을 한 번 확정한 뒤, 이후부터는 진영별 개별 난수로 전환합니다.
    /// </summary>
    public static void OnRoundStarted()
    {
        try
        {
            ApplyPrimaryIntervalToAll(GetOrRollSharedPrimaryInterval(forceReroll: true), includeSpawnInterval: true);
            _useSharedPrimaryInterval = false;
        }
        catch (Exception e)
        {
            Log.Error($"[WaveTimerPatch] OnRoundStarted Exception: {e}");
        }
    }

    public static void WaveTimerResetPrefix(WaveTimer __instance, bool resetSpawnInterval)
    {
        try
        {
            if (!resetSpawnInterval || __instance?._wave == null)
                return;

            if (__instance._wave is IMiniWave)
            {
                __instance.DefaultSpawnInterval = MiniWaveSpawnInterval;
                return;
            }

            if (_useSharedPrimaryInterval)
            {
                float sharedPrimaryInterval = GetOrRollSharedPrimaryInterval();
                ApplyPrimaryIntervalToAll(sharedPrimaryInterval, includeSpawnInterval: false);
                __instance.DefaultSpawnInterval = sharedPrimaryInterval;
                return;
            }

            // 라운드 시작 이후: 진영별 개별 Respawn Time
            __instance.DefaultSpawnInterval = UnityEngine.Random.Range(PrimaryIntervalMin, PrimaryIntervalMax);
        }
        catch (Exception e)
        {
            Log.Error($"[WaveTimerPatch] WaveTimerResetPrefix Exception: {e}");
        }
    }

    private static float GetOrRollSharedPrimaryInterval(bool forceReroll = false)
    {
        int frame = Time.frameCount;
        if (forceReroll || _sharedPrimaryIntervalFrame != frame || _sharedPrimaryInterval <= 0f)
        {
            _sharedPrimaryInterval = UnityEngine.Random.Range(PrimaryIntervalMin, PrimaryIntervalMax);
            _sharedPrimaryIntervalFrame = frame;
        }

        return _sharedPrimaryInterval;
    }

    private static void ApplyPrimaryIntervalToAll(float primaryInterval, bool includeSpawnInterval)
    {
        foreach (SpawnableWaveBase wave in WaveManager.Waves)
        {
            if (wave is not TimeBasedWave timeBasedWave)
                continue;

            bool isMini = wave is IMiniWave;
            timeBasedWave.AdditionalSecondsPerSpawn = isMini ? MiniWaveSecondsPerSpawn : PrimarySecondsPerSpawn;

            if (timeBasedWave.Timer == null)
                continue;

            float interval = isMini ? MiniWaveSpawnInterval : primaryInterval;
            timeBasedWave.Timer.DefaultSpawnInterval = interval;

            if (includeSpawnInterval)
                timeBasedWave.Timer.SpawnIntervalSeconds = interval;
        }
    }

    /// <summary>
    /// 초기값(2) 및 마일스톤 차감으로 풀이 고갈되지 않도록 항상 무제한으로 유지합니다.
    /// 진영별 RespawnTokens 소모/고갈은 그대로 두어 DeadmanSwitch가 동작합니다.
    /// </summary>
    public static void AvailableRespawnsLeftSetPrefix(ref int value)
    {
        value = int.MaxValue;
    }

    public static void MiniWaveInitialSpawnIntervalPostfix(ref float __result)
    {
        __result = MiniWaveSpawnInterval;
    }

    public static IEnumerable<CodeInstruction> MiniWaveAvailabilityTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldc_R4 && instruction.operand is float value && Mathf.Approximately(value, 150f))
            {
                instruction.operand = MiniWaveSpawnInterval;
            }

            yield return instruction;
        }
    }
}
