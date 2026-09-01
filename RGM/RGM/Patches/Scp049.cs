using CustomPlayerEffects;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Scp049;
using HarmonyLib;
using PlayerRoles.PlayableScps.Scp049;
using PlayerStatsSystem;
using System;

namespace RGM.Patches;

/// <summary>
/// SCP-049 전역 패치:
/// 1) CardiacArrest 대상 즉사 공격
///    바닐라 즉사는 Damage = -1 센티넬로 처리되므로, 공격력을 배율로 조정하는 모드가 하나라도 끼어들면
///    센티넬이 깨져(-1 * 배율) 피해가 전혀 들어가지 않습니다.
///    최대 체력만큼의 실피해를 직접 넣어 즉사를 보장합니다.
/// </summary>
public static class Scp049Patch
{
    public static void Apply(Harmony harmony)
    {
        try
        {
            // 다른 모드가 공격을 취소했는지 확인한 뒤 개입해야 하므로,
            // 이벤트 구독 대신 모든 구독자가 끝난 시점인 디스패처 후처리에서 동작합니다.
            harmony.Patch(
                AccessTools.Method(typeof(Exiled.Events.Handlers.Scp049),
                    nameof(Exiled.Events.Handlers.Scp049.OnAttacking)),
                postfix: new HarmonyMethod(typeof(Scp049Patch), nameof(OnAttackingPostfix)));

            Log.Info("[Scp049Patch] Applied.");
        }
        catch (Exception e)
        {
            Log.Error($"[Scp049Patch] Failed to apply: {e}");
        }
    }

    public static void OnAttackingPostfix(AttackingEventArgs ev)
    {
        try
        {
            if (ev is not { IsAllowed: true } || ev.Player == null || ev.Target == null)
                return;

            if (!ev.Target.TryGetEffect(out CardiacArrest cardiacArrest) || !cardiacArrest.IsEnabled)
                return;

            // 바닐라 즉사 경로를 막고 동일한 후처리(쿨타임·히트마커)를 직접 수행합니다.
            ev.IsAllowed = false;

            ev.Scp049.RemainingAttackCooldown = Scp049AttackAbility.CooldownTime;

            ev.Target.Hurt(new Scp049DamageHandler(
                ev.Player.ReferenceHub,
                ev.Target.MaxHealth + ev.Target.MaxArtificialHealth + ev.Target.MaxHumeShield,
                Scp049DamageHandler.AttackType.Instakill));

            Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 1f, false);
        }
        catch (Exception e)
        {
            Log.Error($"[Scp049Patch] OnAttackingPostfix Exception: {e}");
        }
    }
}
