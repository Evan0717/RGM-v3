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

    private static CoroutineHandle _cleaner;

    private static CoroutineHandle _coreModule;
    
    public static void Start()
    {
        if (!Timing.IsRunning(_cleaner)) 
            _cleaner = Timing.RunCoroutine(CleanerCoroutine());
        Cleaner();
        _coreModule = Timing.RunCoroutine(ActivateEffects());
    }

    public static void Stop()
    {
        if (Timing.IsRunning(_cleaner))
            Timing.KillCoroutines(_cleaner);
        Timing.KillCoroutines(_coreModule);
        Cleaner();
    }

    private static IEnumerator<float> ActivateEffects()
    {
        foreach (var player in PlayerManager.List.Where(x => x.IsScpRole())) {
            if (!Round.IsStarted || Round.IsEnded || !player.IsScp || player.IsNonePlayer()) 
                yield return Timing.WaitForSeconds(Timing.WaitForOneFrame);

            switch (player.Role.Type)
            {
                case RoleTypeId.Scp173:
                    if (SpeedStore.ScpCoroutines[player].ContainsKey(ScpEffectEnum.Scp173)) break;
                    SpeedStore.ScpCoroutines.Add(player, new()
                    {
                        { ScpEffectEnum.Scp173, Timing.RunCoroutine(Scp173Effect(player)) }
                    });
                    break;

                case RoleTypeId.Scp049:
                    if (SpeedStore.ScpCoroutines[player].ContainsKey(ScpEffectEnum.Scp049)) break;
                    SpeedStore.ScpCoroutines.Add(player, new()
                    {
                        { ScpEffectEnum.Scp049, Timing.RunCoroutine(Scp049Effect(player)) }
                    });
                    break;

                case RoleTypeId.Scp939:
                    if (SpeedStore.ScpCoroutines[player].ContainsKey(ScpEffectEnum.Scp939)) break;
                    SpeedStore.ScpCoroutines.Add(player, new()
                    {
                        { ScpEffectEnum.Scp939, Timing.RunCoroutine(Scp939Effect(player)) }
                    });
                    break;

                case RoleTypeId.Scp079:
                    if (SpeedStore.ScpCoroutines[player].ContainsKey(ScpEffectEnum.Scp079)) break;
                    SpeedStore.ScpCoroutines.Add(player, new()
                    {
                        { ScpEffectEnum.Scp079, Timing.RunCoroutine(Scp079Effect(player)) }
                    });
                    break;

                case RoleTypeId.Scp106:
                    if (SpeedStore.ScpCoroutines[player].ContainsKey(ScpEffectEnum.Scp106)) break;
                    SpeedStore.ScpCoroutines.Add(player, new()
                    {
                        { ScpEffectEnum.Scp106, Timing.RunCoroutine(Scp106Effect(player)) }
                    });
                    break;

                case RoleTypeId.Scp3114:
                    if (SpeedStore.ScpCoroutines[player].ContainsKey(ScpEffectEnum.Scp3114)) break;
                    SpeedStore.ScpCoroutines.Add(player, new()
                    {
                        { ScpEffectEnum.Scp3114, Timing.RunCoroutine(Scp3114Effect(player)) }
                    });
                    break;

                case RoleTypeId.Scp096:
                    if (SpeedStore.ScpCoroutines[player].ContainsKey(ScpEffectEnum.Scp096)) break;
                    SpeedStore.ScpCoroutines.Add(player, new()
                    {
                        { ScpEffectEnum.Scp096, Timing.RunCoroutine(Scp096Effect(player)) }
                    });
                    break;
            }

            yield return Timing.WaitForSeconds(10.0f);
        }
    }

    /// <summary>
    /// 모든 SCP 플레이어 또는 해당 코루틴을 가지고 있는 유저에 대하여 코루틴 초기화를 강행합니다.
    /// <br />
    /// <b>단, SCP이면서 코루틴이 가동중일 경우 삭제와 동시에 실행될 수 있습니다.</b>
    /// <br />
    /// 코루틴은 <b>코루틴 내부 타이머 기준 10초 후, 다시 활성화됩니다.</b>
    /// </summary>
    public static void Cleaner()
    {
        try
        {
            foreach (var player in PlayerManager.List.Where(target =>
                         target.IsScpRole() || SpeedStore.ScpCoroutines.ContainsKey(target)))
            {
                foreach (var items in SpeedStore.ScpCoroutines[player].Values.ToList())
                {
                    Timing.KillCoroutines(items);
                }

                SpeedStore.ScpCoroutines.Remove(player);
            }

            Timing.KillCoroutines(_scp079EnergyEffect);
        }
        catch (Exception e)
        {
            Log.Error($"ScpEffects.cs(KoreanSpeed)에서 코루틴 초기화 도중 오류 발생\n내용: {e}");
        }
    }
    
    private static IEnumerator<float> CleanerCoroutine()
    {
        try
        {
            foreach (var player in PlayerManager.List.Where(target => SpeedStore.ScpCoroutines.ContainsKey(target) && !target.IsScpRole()))
            {
                foreach (var items in SpeedStore.ScpCoroutines[player].Values.ToList())
                {
                    Timing.KillCoroutines(items);
                }
                if (Timing.IsRunning(_scp079EnergyEffect) && !PlayerManager.List.Exists(x => x.Role == RoleTypeId.Scp079))
                    Timing.KillCoroutines(_scp079EnergyEffect);
                
                SpeedStore.ScpCoroutines.Remove(player);
            }

            
        }
        catch (Exception e)
        {
            Log.Error($"ScpEffects.cs(KoreanSpeed)에서 코루틴 초기화 도중 오류 발생\n내용: {e}");
        }

        yield return Timing.WaitForSeconds(60.0f);
    }

    private static IEnumerator<float> Scp096Effect(Player player)
    {
        if (player.Role is not Scp096Role scp096) yield break;

        scp096.ChargeCooldown =
            scp096.ChargeCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier <= 0
                ? 0
                : scp096.ChargeCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier;

        scp096.EnrageCooldown =
            scp096.EnrageCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier <= 0
                ? 0
                : scp096.EnrageCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier;

        yield return Timing.WaitForSeconds(Timing.WaitForOneFrame);
    }

    private static IEnumerator<float> Scp106Effect(Player player)
    {
        if (player.Role is not Scp106Role scp106) yield break;

        scp106.CaptureCooldown = scp106.CaptureCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier <= 0
            ? 0
            : scp106.CaptureCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier;


        scp106.RemainingSinkholeCooldown = scp106.CaptureCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier <= 0
            ? 0
            : scp106.RemainingSinkholeCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier;

        yield return Timing.WaitForSeconds(Timing.WaitForOneFrame);
    }

    private static IEnumerator<float> Scp939Effect(Player player)
    {
        if (player.Role is not Scp939Role scp939) yield break;

        scp939.AmnesticCloudCooldown = scp939.AmnesticCloudCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier <= 0
            ? 0
            : scp939.AmnesticCloudCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier;

        scp939.AttackCooldown = scp939.AttackCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier <= 0
            ? 0
            : scp939.AttackCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier;

        scp939.MimicryCooldown = scp939.MimicryCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier <= 0
            ? 0
            : scp939.MimicryCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier;
    }

    private static IEnumerator<float> Scp049Effect(Player player)
    {
        if (player.Role is not Scp049Role scp049) yield break;

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

        yield return Timing.WaitForSeconds(Timing.WaitForOneFrame);
    }

    private static IEnumerator<float> Scp079Effect(Player player)
    {
        if (player.Role is not Scp079Role scp079) yield break;

        scp079.BlackoutZoneCooldown = scp079.BlackoutZoneCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier <= 0
            ? 0
            : scp079.BlackoutZoneCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier;

        scp079.RoomLockdownCooldown = scp079.RoomLockdownCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier <= 0
            ? 0
            : scp079.RoomLockdownCooldown - SpeedStore.Count * SpeedStore.ScpMultiplier;

        if (!Timing.IsRunning(_scp079EnergyEffect))
            _scp079EnergyEffect = Timing.RunCoroutine(EnergyCreator());

        yield return Timing.WaitForSeconds(Timing.WaitForOneFrame);
        yield break;

        IEnumerator<float> EnergyCreator()
        {
            if (Mathf.Approximately(scp079.MaxEnergy, scp079.Energy))
                yield return Timing.WaitForSeconds(Timing.WaitForOneFrame);

            scp079.Energy += 1;

            yield return Timing.WaitForSeconds(Timing.WaitForOneFrame);
        }
    }

    private static IEnumerator<float> Scp173Effect(Player player)
    {
        if (player.Role is not Scp173Role scp173) yield break;

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

        yield return Timing.WaitForSeconds(Timing.WaitForOneFrame);
    }

    private static IEnumerator<float> Scp3114Effect(Player player)
    {
        if (player.Role is not Scp3114Role scp3114) yield break;

        scp3114.StaminaRegenMultiplier += SpeedStore.Count * SpeedStore.ScpMultiplier;
    }
}