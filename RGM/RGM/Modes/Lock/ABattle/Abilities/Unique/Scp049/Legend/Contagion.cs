using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.Scp049.Legend;

[Ability("전염병",
    """
    자신과 아군을 제외한 상대 중, 서로 6m 이내에 있는 대상에게 『사회적 거리두기 · 봉쇄』 효과를 적용합니다.
    해당 효과는 디버프 제거 효과와 무적 효과를 무시합니다.
    """,
    AbilityCategory.Legend,
    AbilityType.LEGEND_SCP049_CONTAGION,
    RoleAbility.Scp049)]
public class Contagion : Ability
{
    private const float EffectRange = 6f;
    private const float DamageInterval = 1f;
    private const float DebuffDuration = 0.2f;
    private const float EffectRefreshInterval = 0.1f;

    private CoroutineHandle _pandemicCoroutine;

    public override void OnEnabled()
    {
        _pandemicCoroutine = Timing.RunCoroutine(ApplyContagion());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_pandemicCoroutine);
    }

    private IEnumerator<float> ApplyContagion()
    {
        float elapsedDamageTime = DamageInterval;

        while (Owner.IsAlive)
        {
            bool applyDamage = elapsedDamageTime >= DamageInterval;

            foreach (Player player in PlayerManager.List.Where(IsAffected))
            {
                // 짧은 지속시간으로 계속 갱신하여 디버프 제거 후에도 즉시 다시 감염시킨다.
                player.EnableEffect(EffectType.Poisoned, 1, DebuffDuration);
                player.EnableEffect(EffectType.Exhausted, 1, DebuffDuration);

                if (!applyDamage)
                    continue;

                float damage = player.IsScpRole() ? player.MaxHealth * 0.09f : player.MaxHealth * 0.04f;

                // Hurt 이벤트를 거치지 않아 피해 무효화 능력 및 GodMode를 우회한다.
                player.Health -= damage;

                if (player.Health <= 0f)
                    player.Kill("전염병을 이겨내지 못했습니다.");
            }

            elapsedDamageTime = applyDamage ? 0f : elapsedDamageTime + EffectRefreshInterval;

            yield return Timing.WaitForSeconds(EffectRefreshInterval);
        }
    }

    private bool IsEnemy(Player player) =>
        player != Owner &&
        player.IsAlive &&
        player.Role.Type != RoleTypeId.Scp079 &&
        player.Role.Team != Owner.Role.Team;

    private bool IsAffected(Player player) =>
        IsEnemy(player) &&
        PlayerManager.List.Any(other =>
            other != player &&
            IsEnemy(other) &&
            Vector3.Distance(player.Position, other.Position) <= EffectRange);
}