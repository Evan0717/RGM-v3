using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using System.Collections.Generic;

namespace RGM.RGM.Modes.Lock.EchoBattle;

/// <summary>
/// 반복 가능 내부 Quest.
/// 1) 30초 생존 → 130 XP
/// 2) 적에게 50 데미지 → 100 XP
/// 3) 25 데미지 받기 → 100 XP
/// </summary>
public static class EchoQuest
{
    public const int SurviveSeconds = 30;
    public const int SurviveReward = 130;

    public const float DealDamageThreshold = 50f;
    public const int DealDamageReward = 100;

    public const float TakeDamageThreshold = 25f;
    public const int TakeDamageReward = 100;

    static readonly Dictionary<Player, QuestProgress> Progress = new();
    static readonly Dictionary<Player, CoroutineHandle> SurviveHandles = new();

    public class QuestProgress
    {
        public float DamageDealt;
        public float DamageTaken;
        public float SurviveTimer;
    }

    public static void Register()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public static void Unregister()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;

        foreach (var handle in SurviveHandles.Values)
            Timing.KillCoroutines(handle);

        SurviveHandles.Clear();
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
            // Echo 미장착 시에는 생존 퀘스트 진행하지 않음
            if (!EchoInfo.PlayerEchoes.TryGetValue(player, out var echoes) || echoes.Count == 0)
            {
                progress.SurviveTimer = 0f;
                yield return Timing.WaitForSeconds(1f);
                continue;
            }

            progress.SurviveTimer += 1f;

            if (progress.SurviveTimer >= SurviveSeconds)
            {
                progress.SurviveTimer = 0f;
                EchoGrowth.GrantExpToEquipped(player, SurviveReward, "30초 생존");
                player.ShowHint($"<color=#88aaff>Quest</color> 30초 생존 <color=#7CFC00>+{SurviveReward} XP</color>", 2);
            }

            yield return Timing.WaitForSeconds(1f);
        }
    }

    static void OnHurting(HurtingEventArgs ev)
    {
        float damage = ev.DamageHandler?.Damage ?? 0f;
        if (damage <= 0f)
            return;

        // 가해: 적에게 데미지
        if (ev.Attacker != null
            && ev.Attacker != ev.Player
            && EchoInfo.PlayerEchoes.TryGetValue(ev.Attacker, out var atkEchoes)
            && atkEchoes.Count > 0
            && HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub))
        {
            var atkProgress = GetOrCreate(ev.Attacker);
            atkProgress.DamageDealt += damage;

            while (atkProgress.DamageDealt >= DealDamageThreshold)
            {
                atkProgress.DamageDealt -= DealDamageThreshold;
                EchoGrowth.GrantExpToEquipped(ev.Attacker, DealDamageReward, "데미지 가함");
                ev.Attacker.ShowHint($"<color=#88aaff>Quest</color> 데미지 50 가함 <color=#7CFC00>+{DealDamageReward} XP</color>", 2);
            }
        }

        // 피격: 데미지 받기
        if (EchoInfo.PlayerEchoes.TryGetValue(ev.Player, out var defEchoes) && defEchoes.Count > 0)
        {
            var defProgress = GetOrCreate(ev.Player);
            defProgress.DamageTaken += damage;

            while (defProgress.DamageTaken >= TakeDamageThreshold)
            {
                defProgress.DamageTaken -= TakeDamageThreshold;
                EchoGrowth.GrantExpToEquipped(ev.Player, TakeDamageReward, "데미지 받음");
                ev.Player.ShowHint($"<color=#88aaff>Quest</color> 데미지 25 받음 <color=#7CFC00>+{TakeDamageReward} XP</color>", 2);
            }
        }
    }
}
