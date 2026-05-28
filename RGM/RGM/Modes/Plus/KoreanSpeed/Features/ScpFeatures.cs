using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using RGM.Modes.Interfaces;
using UnityEngine;

namespace RGM.Modes;

public class ScpFeatures : ILogicFeatures
{
    private static bool _isRunning;
    private static CoroutineHandle _isRunning079;
    private static CoroutineHandle _isRunning173;

    public void Run() => Timing.RunCoroutine(RegisterFeatures());

    private static IEnumerator<float> RegisterFeatures()
    {
        while (SpeedStore.IsEnabled)
        {
            if (!_isRunning)
            {
                yield return Timing.WaitForSeconds(30.5f);
                Tools.MessageTranslated(
                    "SCP move system has been Activated",
                    "SCP 움직임 시스템이 가동되었습니다.",
                    true);
                _isRunning = true;
            }
            else
            {
                Scp096Effect();
                Scp049Effect();
                Scp106Effect();
                Scp079Effect();
                Scp173Effect();
                Scp3114Effect();
                Scp939Effect();
                yield return Timing.WaitForSeconds(.4f);
            }
        }

        Cleaner();
    }

    private static void Cleaner()
    {
        try
        {
            _isRunning = false;
        }
        catch (Exception e)
        {
            Log.Error($"ScpEffects.cs(KoreanSpeed)에서 코루틴 초기화 도중 오류 발생\n내용: {e}");
        }
    }

    private static void Scp096Effect()
    {
        foreach (var player in PlayerManager.List.Where(target =>
                     target.IsScpRole() && target.Role.Type == RoleTypeId.Scp096))
        {
            if (player.Role is not Scp096Role scp096) continue;

            if (scp096.ChargeCooldown != 0)
                scp096.ChargeCooldown =
                    Math.Max(0.1f, scp096.ChargeCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier);
        }
    }

    private static void Scp106Effect()
    {
        foreach (var player in PlayerManager.List.Where(target =>
                     target.IsScpRole() && target.Role.Type == RoleTypeId.Scp106))
        {
            if (player.Role is not Scp106Role scp106) continue;

            if (scp106.CaptureCooldown != 0)
                scp106.CaptureCooldown =
                    Math.Max(0.1f, scp106.CaptureCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier);

            if (scp106.RemainingSinkholeCooldown != 0)
                scp106.RemainingSinkholeCooldown = Math.Max(0.1f,
                    scp106.RemainingSinkholeCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier);
        }
    }

    private static void Scp939Effect()
    {
        foreach (var player in PlayerManager.List.Where(target =>
                     target.IsScpRole() && target.Role.Type == RoleTypeId.Scp939))
        {
            if (player.Role is not Scp939Role scp939) continue;


            Timing.CallDelayed(.3f, () =>
            {
                if (scp939.AmnesticCloudCooldown != 0 && scp939.AmnesticCloudDuration == 0.0f)
                    scp939.AmnesticCloudCooldown = Math.Max(0.1f,
                        scp939.AmnesticCloudCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier);
            });
            
            if (scp939.AttackCooldown != 0)
                scp939.AttackCooldown =
                    Math.Max(0.1f, scp939.AttackCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier);

            if (scp939.MimicryCooldown != 0)
                scp939.MimicryCooldown =
                    Math.Max(0.1f, scp939.MimicryCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier);
        }
    }

    private static void Scp049Effect()
    {
        foreach (var player in PlayerManager.List.Where(target =>
                     target.IsScpRole() && target.Role.Type == RoleTypeId.Scp049))
        {
            if (player.Role is not Scp049Role scp049) continue;

            if (scp049.CallCooldown != 0)
                scp049.CallCooldown = Math.Max(0.1f, scp049.CallCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier);

            if (scp049.GoodSenseCooldown != 0)
                scp049.GoodSenseCooldown =
                    Math.Max(0.2f, scp049.GoodSenseCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier); 

            if (scp049.RemainingAttackCooldown != 0)
                scp049.RemainingAttackCooldown = Math.Max(0.1f,
                    scp049.RemainingAttackCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier);
        }
    }

    private static void Scp079Effect()
    {
        foreach (var player in PlayerManager.List.Where(target =>
                     target.IsScpRole() && target.Role.Type == RoleTypeId.Scp079))
        {
            if (player.Role is not Scp079Role scp079) continue;

            if (scp079.BlackoutZoneCooldown != 0)
                scp079.BlackoutZoneCooldown =
                    Math.Max(0.1f, scp079.BlackoutZoneCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier);

            if (!Timing.IsRunning(_isRunning079))
                _isRunning079 = Timing.RunCoroutine(EnergyCreator());
        }

        return;

        IEnumerator<float> EnergyCreator()
        {
            while (SpeedStore.IsEnabled)
            {
                foreach (var player in PlayerManager.List.Where(target =>
                             target.IsScpRole() && target.Role.Type == RoleTypeId.Scp079))
                {
                    if (player.Role is not Scp079Role scp079) continue;

                    if (Mathf.Approximately(scp079.MaxEnergy, scp079.Energy))
                        continue;

                    scp079.Energy += 1;
                }

                yield return 
                    Timing.WaitForSeconds(Math.Max(.1f, 2.5f - SpeedStore.Count * SpeedStore.ScpMultiplier));
            } 
        }
    }

    private static void Scp173Effect()
    {
        foreach (var player in PlayerManager.List.Where(target =>
                     target.IsScpRole() && target.Role.Type == RoleTypeId.Scp173))
        {
            if (player.Role is not Scp173Role scp173) continue;

            if (scp173.RemainingBreakneckCooldown != 0 && !(scp173.RemainingBreakneckCooldown <= .3f))
                scp173.RemainingBreakneckCooldown = Math.Max(0.1f,
                    scp173.RemainingBreakneckCooldown - SpeedStore.Count * 0.1f);

            if (scp173.RemainingTantrumCooldown != 0)
                scp173.RemainingTantrumCooldown = Math.Max(0.1f,
                    scp173.RemainingTantrumCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier);
        }
        
        if (!Timing.IsRunning(_isRunning173))
            _isRunning173 = Timing.RunCoroutine(BlinkCreator());
        return;

        IEnumerator<float> BlinkCreator()
        {
            while (SpeedStore.IsEnabled)
            {
                foreach (var player in PlayerManager.List.Where(target =>
                             target.IsScpRole() && target.Role.Type == RoleTypeId.Scp173))
                {
                    if (player.Role is not Scp173Role scp173) continue;
                    
                    if (scp173.BlinkCooldown != 0)  
                        scp173.BlinkCooldown = Math.Max(0.1f,
                            scp173.BlinkCooldown - SpeedStore.Count * .01f);
                }
                yield return Timing.WaitForSeconds(.1f);
            }
        }
    }

    private static void Scp3114Effect()
    {
        foreach (var player in
                 PlayerManager.List.Where(target => target.IsScp && target.Role.Type == RoleTypeId.Scp3114))
        {
            if (player.Role is not Scp3114Role scp3114) continue;

            scp3114.StaminaRegenMultiplier += SpeedStore.Count * SpeedStore.ScpMultiplier;
        }
    }
}