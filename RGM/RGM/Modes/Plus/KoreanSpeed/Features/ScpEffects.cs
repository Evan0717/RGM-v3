using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes;

public class ScpEffects
{
    private static CoroutineHandle _scp079EnergyEffect;

    public static void Start()
    {
        Cleaner();
        ActivateEffects();
    }

    public static void Stop()
    {
        Cleaner();
    }

    private static void ActivateEffects()
    {
            if (!Round.IsStarted || Round.IsEnded)
            {
                Stop();
                return;
            }

            CoroutineHandles.Scp079 = Timing.RunCoroutine(Scp079Effect());
            CoroutineHandles.Scp049 = Timing.RunCoroutine(Scp049Effect());
            CoroutineHandles.Scp939 = Timing.RunCoroutine(Scp939Effect());
            CoroutineHandles.Scp106 = Timing.RunCoroutine(Scp106Effect());
            CoroutineHandles.Scp3114 = Timing.RunCoroutine(Scp3114Effect());
            CoroutineHandles.Scp096 = Timing.RunCoroutine(Scp096Effect());
            CoroutineHandles.Scp173 = Timing.RunCoroutine(Scp173Effect());

    }

    /// <summary>
    /// 모든 SCP 플레이어 또는 해당 코루틴을 가지고 있는 유저에 대하여 코루틴 초기화를 강행합니다.
    /// <br />
    /// <b>단, SCP이면서 코루틴이 가동중일 경우 삭제와 동시에 실행될 수 있습니다.</b>
    /// <br />
    /// 코루틴은 <b>코루틴 내부 타이머 기준 10초 후, 다시 활성화됩니다.</b>
    /// </summary>
    private static void Cleaner()
    {
        try
        {
            Timing.KillCoroutines(CoroutineHandles.Scp096);
            Timing.KillCoroutines(CoroutineHandles.Scp049);
            Timing.KillCoroutines(CoroutineHandles.Scp106);
            Timing.KillCoroutines(CoroutineHandles.Scp939);
            Timing.KillCoroutines(CoroutineHandles.Scp3114);
            Timing.KillCoroutines(CoroutineHandles.Scp079);
            Timing.KillCoroutines(CoroutineHandles.Scp173);
            
            if (!PlayerManager.List.Exists(x => x.Role == RoleTypeId.Scp079) && Timing.IsRunning(_scp079EnergyEffect))
                Timing.KillCoroutines(_scp079EnergyEffect);
        }
        catch (Exception e)
        {
            Log.Error($"ScpEffects.cs(KoreanSpeed)에서 코루틴 초기화 도중 오류 발생\n내용: {e}");
        }
    }

    private static IEnumerator<float> Scp096Effect()
    {
        foreach (var player in PlayerManager.List.Where(target =>
                     target.IsScpRole() && target.Role.Type == RoleTypeId.Scp096))
        {
            if (player.Role is not Scp096Role scp096) continue;

            scp096.ChargeCooldown =
                scp096.ChargeCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier <= 0
                    ? 0
                    : scp096.ChargeCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier;

            scp096.EnrageCooldown =
                scp096.EnrageCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier <= 0
                    ? 0
                    : scp096.EnrageCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier;
        }

        yield return Timing.WaitForSeconds(Timing.WaitForOneFrame);
    }

    private static IEnumerator<float> Scp106Effect()
    {
        foreach (var player in PlayerManager.List.Where(target =>
                     target.IsScpRole() && target.Role.Type == RoleTypeId.Scp106))
        {
            if (player.Role is not Scp106Role scp106) continue;

            scp106.CaptureCooldown = scp106.CaptureCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier <= 0
                ? 0
                : scp106.CaptureCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier;


            scp106.RemainingSinkholeCooldown = scp106.CaptureCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier <= 0
                ? 0
                : scp106.RemainingSinkholeCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier;
        }

        yield return Timing.WaitForSeconds(Timing.WaitForOneFrame);
    }

    private static IEnumerator<float> Scp939Effect()
    {
        foreach (var player in PlayerManager.List.Where(target =>
                     target.IsScpRole() && target.Role.Type == RoleTypeId.Scp939))
        {
            if (player.Role is not Scp939Role scp939) continue;

            scp939.AmnesticCloudCooldown =
                scp939.AmnesticCloudCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier <= 0
                    ? 0
                    : scp939.AmnesticCloudCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier;

            scp939.AttackCooldown = scp939.AttackCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier <= 0
                ? 0
                : scp939.AttackCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier;

            scp939.MimicryCooldown = scp939.MimicryCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier <= 0
                ? 0
                : scp939.MimicryCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier;
        }

        yield return Timing.WaitForSeconds(Timing.WaitForOneFrame);
    }

    private static IEnumerator<float> Scp049Effect()
    {
        foreach (var player in PlayerManager.List.Where(target =>
                     target.IsScpRole() && target.Role.Type == RoleTypeId.Scp049))
        {
            if (player.Role is not Scp049Role scp049) continue;

            scp049.CallCooldown = scp049.CallCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier <= 0
                ? 0
                : scp049.CallCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier;

            scp049.GoodSenseCooldown = scp049.GoodSenseCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier <= 0
                ? 0
                : scp049.GoodSenseCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier;

            scp049.RemainingAttackCooldown =
                scp049.RemainingAttackCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier <= 0
                    ? 0
                    : scp049.RemainingAttackCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier;
        }

        yield return Timing.WaitForSeconds(Timing.WaitForOneFrame);
    }

    private static IEnumerator<float> Scp079Effect()
    {
        foreach (var player in PlayerManager.List.Where(target =>
                     target.IsScpRole() && target.Role.Type == RoleTypeId.Scp079))
        {
            if (player.Role is not Scp079Role scp079) continue;

            scp079.BlackoutZoneCooldown = scp079.BlackoutZoneCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier <= 0
                ? 0
                : scp079.BlackoutZoneCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier;

            scp079.RoomLockdownCooldown = scp079.RoomLockdownCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier <= 0
                ? 0
                : scp079.RoomLockdownCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier;

            if (!Timing.IsRunning(_scp079EnergyEffect))
                _scp079EnergyEffect = Timing.RunCoroutine(EnergyCreator());
        }

        yield return Timing.WaitForSeconds(Timing.WaitForOneFrame);

        IEnumerator<float> EnergyCreator()
        {
            foreach (var player in PlayerManager.List.Where(target =>
                         target.IsScpRole() && target.Role.Type == RoleTypeId.Scp079))
            {
                if (player.Role is not Scp079Role scp079) continue;

                if (Mathf.Approximately(scp079.MaxEnergy, scp079.Energy))
                    yield return Timing.WaitForSeconds(Timing.WaitForOneFrame);

                scp079.Energy += 1;

                yield return Timing.WaitForSeconds(Timing.WaitForOneFrame);
            }
        }
    }

    private static IEnumerator<float> Scp173Effect()
    {
        foreach (var player in PlayerManager.List.Where(target =>
                     target.IsScpRole() && target.Role.Type == RoleTypeId.Scp173))
        {
            if (player.Role is not Scp173Role scp173) continue;

            scp173.BlinkCooldown = scp173.BlinkCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier <= 0
                ? 0
                : scp173.BlinkCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier;

            scp173.RemainingBreakneckCooldown =
                scp173.RemainingBreakneckCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier <= 0
                    ? 0
                    : scp173.RemainingBreakneckCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier;

            scp173.RemainingTantrumCooldown =
                scp173.RemainingTantrumCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier <= 0
                    ? 0
                    : scp173.RemainingTantrumCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier;
        }

        yield return Timing.WaitForSeconds(Timing.WaitForOneFrame);
    }

    private static IEnumerator<float> Scp3114Effect()
    {
        foreach (var player in
                 PlayerManager.List.Where(target => target.IsScp && target.Role.Type == RoleTypeId.Scp096))
        {
            if (player.Role is not Scp3114Role scp3114) continue;

            scp3114.StaminaRegenMultiplier += SpeedStore.Count * SpeedStore.ScpMultiplier;
        }

        yield return Timing.WaitForSeconds(Timing.WaitForOneFrame);
    }
}