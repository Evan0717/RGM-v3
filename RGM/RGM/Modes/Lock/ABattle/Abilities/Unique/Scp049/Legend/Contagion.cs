using System.Collections.Generic;
using System.Linq;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.Scp049.Legend;

[Ability("전염병",
    """
    SCP-049의 F 스킬이 적용된 대상 주변 7m 이내에 2초 이상 머문 모든 대상에게 『전염』 효과를 부여합니다.
    자신과 아군은 『전염』 효과를 받지 않으며, 자신은 『집결』 효과가 적용됩니다.
    <color=#BF40BF>[전용 영웅]</color> 의료 사고 능력을 이미 보유 중인 경우, <color=#FFC000>[전용 전설]</color> 전염병 능력으로 대체됩니다.
    """,
    AbilityCategory.Legend,
    AbilityType.LEGEND_SCP049_CONTAGION,
    RoleAbility.Scp049)]

public class Contagion : Ability
{
    private const float ContagionRange = 7f;
    private const float RequiredExposureDuration = 2f;
    private const float ExposureCheckInterval = 0.1f;
    private const float PermanentDuration = float.MaxValue;
    private const float ProficiencyConversionDelay = 0.1f;
    private const float TeleportDelay = ProficiencyConversionDelay + 0.1f;

    private readonly HashSet<Player> _contagiousPlayers = [];
    private readonly Dictionary<Player, float> _exposureDurations = [];
    private CoroutineHandle _contagionCoroutine;

    public override void OnEnabled()
    {
        if (Owner.HasAbility(AbilityType.EPIC_SCP049_MEDICALACCIDENT)) {
            Owner.RemoveAbility(AbilityType.EPIC_SCP049_MEDICALACCIDENT);
        }
        Owner.AddAbility(AbilityType.RARE_SCP049_PROFICIENCY);

        ABattle.Instance.AddingAbility += OnAddingAbility;
        Exiled.Events.Handlers.Scp049.ActivatingSense += OnActivatingSense;
        Exiled.Events.Handlers.Player.Died += OnDied;
    }

    public override void OnDisabled()
    {
        ABattle.Instance.AddingAbility -= OnAddingAbility;
        Exiled.Events.Handlers.Scp049.ActivatingSense -= OnActivatingSense;
        Exiled.Events.Handlers.Player.Died -= OnDied;
        Timing.KillCoroutines(_contagionCoroutine);
        _contagiousPlayers.Clear();
        _exposureDurations.Clear();
    }

    private void OnAddingAbility(AddingAbilityEventArgs ev)
    {
        if (ev.Player == Owner && ev.AbilityType == AbilityType.EPIC_SCP049_MEDICALACCIDENT)
            ev.IsAllowed = false;
    }

    private void OnActivatingSense(ActivatingSenseEventArgs ev)
    {
        if (!ev.IsAllowed || ev.Player != Owner || ev.Target is null)
            return;

        if (IsEnemy(ev.Target))
            ApplyContagion(ev.Target);

        Timing.KillCoroutines(_contagionCoroutine);
        _contagionCoroutine = Timing.RunCoroutine(ApplyContagionAfterExposure());
    }

    private bool IsEnemy(Player player) =>
        player != Owner &&
        player.IsAlive &&
        player.Role.Team != Owner.Role.Team;

    private IEnumerator<float> ApplyContagionAfterExposure()
    {
        _exposureDurations.Clear();

        while (_contagiousPlayers.Any(HasActiveCardiacArrest))
        {
            List<Player> contagiousSources = _contagiousPlayers
                .Where(HasActiveCardiacArrest)
                .ToList();

            if (contagiousSources.Count == 0)
                break;

            foreach (Player player in PlayerManager.List)
            {
                if (!IsEnemy(player) ||
                    _contagiousPlayers.Contains(player) ||
                    !IsNearContagiousSource(player, contagiousSources))
                {
                    _exposureDurations.Remove(player);
                    continue;
                }

                _exposureDurations.TryGetValue(player, out float exposureDuration);
                exposureDuration += ExposureCheckInterval;
                _exposureDurations[player] = exposureDuration;

                if (exposureDuration >= RequiredExposureDuration)
                    ApplyContagion(player);
            }

            yield return Timing.WaitForSeconds(ExposureCheckInterval);
        }

        _exposureDurations.Clear();
    }

    private static bool HasActiveCardiacArrest(Player player) =>
        player.IsAlive &&
        player.TryGetEffect<CardiacArrest>(out var cardiacArrest) &&
        cardiacArrest.IsEnabled;

    private static bool IsNearContagiousSource(Player player, IEnumerable<Player> contagiousSources) =>
        contagiousSources.Any(source =>
            Vector3.Distance(player.Position, source.Position) <= ContagionRange);

    private void ApplyContagion(Player player)
    {
        player.EnableEffect(EffectType.CardiacArrest, 1, PermanentDuration);

        if (player.TryGetEffect<CardiacArrest>(out var cardiacArrest))
        {
            cardiacArrest.SetAttacker(Owner.ReferenceHub);
            _contagiousPlayers.Add(player);
        }
    }

    private void OnDied(DiedEventArgs ev)
    {
        if (ev.Attacker != Owner ||
            ev.DamageHandler.Type != DamageType.CardiacArrest ||
            !_contagiousPlayers.Remove(ev.Player))
        {
            return;
        }

        Timing.CallDelayed(TeleportDelay, () =>
        {
            if (Owner.IsAlive && ev.Player.IsAlive)
                ev.Player.Position = Owner.Position;
        });
    }
}