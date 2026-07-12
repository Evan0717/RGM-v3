using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using System.Collections.Generic;

namespace RGM.Modes;

/// <summary>
/// 반복 가능 내부 Quest.
/// 1) 30초 생존 → 60 XP
/// 2) 적에게 100 데미지 → 100 XP
/// 3) 50 데미지 받기 → 150 XP
/// 4) SCP 아이템 획득 → 400 XP
/// 5) 적 1명 처치 → 200 XP
/// 6) SCP로 적 1회 타격 → 50 XP
/// 7) SCP 격리(049-2 제외) → 4000 XP
/// 8) SCP-049-2 처치 → 400 XP
/// </summary>
public static class EchoQuest
{
    public const int SurviveSeconds = 30;
    public const int SurviveReward = 60;

    public const float DealDamageThreshold = 100f;
    public const int DealDamageReward = 100;

    public const float TakeDamageThreshold = 50f;
    public const int TakeDamageReward = 150;

    public const int ScpItemReward = 400;
    public const int KillEnemyReward = 200;
    public const int ScpHitReward = 50;
    public const int ContainScpReward = 4000;
    public const int KillScp0492Reward = 400;

    enum QuestSide
    {
        Common,
        Human,
        Scp
    }

    static readonly Dictionary<Player, QuestProgress> Progress = new();
    static readonly Dictionary<Player, CoroutineHandle> SurviveHandles = new();
    static readonly HashSet<ushort> ClaimedScpItemSerials = new();

    public class QuestProgress
    {
        public float DamageDealt;
        public float DamageTaken;
        public float SurviveTimer;
    }

    public static void Register()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
        Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
        Exiled.Events.Handlers.Player.ItemAdded += OnItemAdded;
        Exiled.Events.Handlers.Player.Died += OnDied;
        Exiled.Events.Handlers.Scp049.Attacking += OnScp049Attacking;
        Exiled.Events.Handlers.Scp106.Attacking += OnScp106Attacking;
    }

    public static void Unregister()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpItem;
        Exiled.Events.Handlers.Player.ItemAdded -= OnItemAdded;
        Exiled.Events.Handlers.Player.Died -= OnDied;
        Exiled.Events.Handlers.Scp049.Attacking -= OnScp049Attacking;
        Exiled.Events.Handlers.Scp106.Attacking -= OnScp106Attacking;

        foreach (var handle in SurviveHandles.Values)
            Timing.KillCoroutines(handle);

        SurviveHandles.Clear();
        ClaimedScpItemSerials.Clear();
        Progress.Clear();
    }

    public static QuestProgress GetOrCreate(Player player)
    {
        if (!Progress.TryGetValue(player, out var progress))
        {
            progress = new QuestProgress();
            Progress[player] = progress;
        }

        return progress;
    }

    public static void StartSurviveTracking(Player player)
    {
        StopSurviveTracking(player);

        if (player == null || !player.IsAlive)
            return;

        SurviveHandles[player] = Timing.RunCoroutine(SurviveRoutine(player), $"EchoQuestSurvive_{player.UserId}");
    }

    /// <summary>
    /// 이미 생존 추적 중이면 타이머를 유지합니다. (레벨업 재적용 시 리셋 방지)
    /// 장착 Echo가 전부 MaxLevel이면 추적을 중지합니다.
    /// </summary>
    public static void EnsureSurviveTracking(Player player)
    {
        if (player == null || !player.IsAlive)
            return;

        if (!CanProgressQuests(player))
        {
            StopSurviveTracking(player);
            return;
        }

        if (SurviveHandles.TryGetValue(player, out var handle) && handle.IsRunning)
            return;

        StartSurviveTracking(player);
    }

    /// <summary>
    /// Echo 장착 + 성장 가능한(Max 미만) Echo가 있거나, 전용무기가 성장 가능하면 퀘스트 진행.
    /// </summary>
    public static bool CanProgressQuests(Player player)
    {
        return CanProgressQuests(player, QuestSide.Common);
    }

    static bool CanProgressQuests(Player player, QuestSide questSide)
    {
        if (player == null)
            return false;

        if (!CanProgressQuestSide(player, questSide))
            return false;

        bool echoGrowable = EchoInfo.PlayerEchoes.TryGetValue(player, out var echoes)
            && echoes.Count > 0
            && EchoInfo.PlayerLoadouts.TryGetValue(player, out var loadout)
            && loadout.HasGrowableEquipped();

        if (echoGrowable)
            return true;

        return ExclusiveWeaponGrowth.CanGrow(player);
    }

    static bool CanProgressQuestSide(Player player, QuestSide questSide)
    {
        return questSide switch
        {
            QuestSide.Human => !player.IsScpRole(),
            QuestSide.Scp => player.IsScpRole(),
            _ => true
        };
    }

    public static void StopSurviveTracking(Player player)
    {
        if (player == null)
            return;

        Timing.KillCoroutines($"EchoQuestSurvive_{player.UserId}");

        if (SurviveHandles.ContainsKey(player))
            SurviveHandles.Remove(player);

        if (Progress.TryGetValue(player, out var progress))
            progress.SurviveTimer = 0f;
    }

    public static void ClearPlayer(Player player)
    {
        StopSurviveTracking(player);
        Progress.Remove(player);
    }

    static IEnumerator<float> SurviveRoutine(Player player)
    {
        var progress = GetOrCreate(player);

        while (player != null && player.IsAlive)
        {
            // 미장착이거나 장착 Echo가 전부 Max면 생존 퀘스트 중단
            if (!CanProgressQuests(player))
            {
                progress.SurviveTimer = 0f;
                SurviveHandles.Remove(player);
                yield break;
            }

            progress.SurviveTimer += 1f;

            if (progress.SurviveTimer >= SurviveSeconds)
            {
                progress.SurviveTimer = 0f;
                GrantQuestReward(player, SurviveReward, "30초 생존");
            }

            yield return Timing.WaitForSeconds(1f);
        }
    }

    static void OnHurting(HurtingEventArgs ev)
    {
        float damage = ev.DamageHandler?.Damage ?? 0f;
        bool isDamagingAttack = damage > 0f || ev.IsInstantKill;
        bool isDirectEnemyAttack = ev.Attacker != null
            && ev.Attacker != ev.Player
            && HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub)
            && ev.DamageHandler?.Type != DamageType.Tesla;

        // SCP 타격: 049/106은 전용 Attacking 이벤트로 처리해 중복 지급을 막습니다.
        if (isDirectEnemyAttack
            && isDamagingAttack
            && CanProgressQuests(ev.Attacker, QuestSide.Scp)
            && !UsesSpecialScpAttackingEvent(ev.Attacker.Role.Type))
        {
            GrantQuestReward(ev.Attacker, ScpHitReward, "SCP 적 1회 타격");
        }

        if (damage <= 0f)
            return;

        // 가해: 적에게 데미지
        if (isDirectEnemyAttack && CanProgressQuests(ev.Attacker))
        {
            var atkProgress = GetOrCreate(ev.Attacker);
            atkProgress.DamageDealt += damage;

            while (atkProgress.DamageDealt >= DealDamageThreshold)
            {
                atkProgress.DamageDealt -= DealDamageThreshold;
                GrantQuestReward(ev.Attacker, DealDamageReward, "데미지 80 가함");
            }
        }

        // 피격: 적 플레이어에게 직접 받은 데미지만 인정 (아군 사격/환경/Tesla 제외)
        if (isDirectEnemyAttack && CanProgressQuests(ev.Player))
        {
            var defProgress = GetOrCreate(ev.Player);
            defProgress.DamageTaken += damage;

            while (defProgress.DamageTaken >= TakeDamageThreshold)
            {
                defProgress.DamageTaken -= TakeDamageThreshold;
                GrantQuestReward(ev.Player, TakeDamageReward, "데미지 40 받음");
            }
        }
    }

    static void OnPickingUpItem(PickingUpItemEventArgs ev)
    {
        if (ev.Player == null || ev.Pickup == null)
            return;

        TryGrantScpItemReward(ev.Player, ev.Pickup.Type, ev.Pickup.Serial, ev.IsAllowed);
    }

    static void OnItemAdded(ItemAddedEventArgs ev)
    {
        if (ev.Player == null || ev.Item == null)
            return;

        TryGrantScpItemReward(ev.Player, ev.Item.Type, ev.Item.Serial, true);
    }

    static void OnScp049Attacking(Exiled.Events.EventArgs.Scp049.AttackingEventArgs ev)
    {
        TryGrantSpecialScpHitReward(ev.Player, ev.Target, ev.IsAllowed);
    }

    static void OnScp106Attacking(Exiled.Events.EventArgs.Scp106.AttackingEventArgs ev)
    {
        TryGrantSpecialScpHitReward(ev.Player, ev.Target, ev.IsAllowed);
    }

    static void OnDied(DiedEventArgs ev)
    {
        if (ev.Attacker == null
            || ev.Player == null
            || ev.Attacker == ev.Player
            || !CanProgressQuests(ev.Attacker, QuestSide.Common)
            || !IsEnemyRole(ev.Attacker.Role.Type, ev.TargetOldRole))
            return;

        GrantQuestReward(ev.Attacker, KillEnemyReward, "적 1명 처치");

        if (!CanProgressQuests(ev.Attacker, QuestSide.Human))
            return;

        if (ev.TargetOldRole == RoleTypeId.Scp0492)
        {
            GrantQuestReward(ev.Attacker, KillScp0492Reward, "SCP-049-2 처치");
        }
        else if (ev.TargetOldRole.IsScpRole())
        {
            GrantQuestReward(ev.Attacker, ContainScpReward, "SCP 격리");
        }
    }

    static bool IsScpItem(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.SCP018
                or ItemType.SCP1576
                or ItemType.SCP2176
                or ItemType.SCP207
                or ItemType.AntiSCP207
                or ItemType.SCP268
                or ItemType.SCP500
                or ItemType.SCP1344
                or ItemType.SCP1853
                or ItemType.SCP330
                or ItemType.SCP244a
                or ItemType.SCP244b
                or ItemType.GunSCP127
                or ItemType.SCP1507Tape => true,
            _ => false
        };
    }

    static bool UsesSpecialScpAttackingEvent(RoleTypeId roleType)
    {
        return roleType is RoleTypeId.Scp049 or RoleTypeId.Scp106;
    }

    static void TryGrantScpItemReward(Player player, ItemType itemType, ushort serial, bool isAllowed)
    {
        if (!isAllowed
            || !CanProgressQuests(player, QuestSide.Human)
            || !IsScpItem(itemType))
            return;

        if (!ClaimedScpItemSerials.Add(serial))
            return;

        GrantQuestReward(player, ScpItemReward, "SCP 아이템 획득");
    }

    static bool IsEnemyRole(RoleTypeId attackerRole, RoleTypeId targetRole)
    {
        return HitboxIdentity.IsEnemy(RoleExtensions.GetTeam(attackerRole), RoleExtensions.GetTeam(targetRole));
    }

    static void TryGrantSpecialScpHitReward(Player attacker, Player target, bool isAllowed)
    {
        if (!isAllowed
            || attacker == null
            || target == null
            || attacker == target
            || !CanProgressQuests(attacker, QuestSide.Scp)
            || !HitboxIdentity.IsEnemy(attacker.ReferenceHub, target.ReferenceHub))
            return;

        GrantQuestReward(attacker, ScpHitReward, "SCP 적 1회 타격");
    }

    static void GrantQuestReward(Player player, int reward, string questName)
    {
        EchoGrowth.GrantExpToEquipped(player, reward, questName);
        ExclusiveWeaponGrowth.GrantExp(player, reward, questName);
        EchoBattleCore.ShowNotification(
            player,
            $"<color=#88aaff>Quest</color> {questName} <color=#7CFC00>+{reward} XP</color>",
            2);
    }
}
