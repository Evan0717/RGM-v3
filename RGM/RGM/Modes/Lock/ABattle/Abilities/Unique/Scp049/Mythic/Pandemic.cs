using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.Scp049.Mythic;

[Ability("PANDEMIC",
    """
    자신과 아군을 제외한 모든 상대에게 『사회적 거리두기 · 봉쇄』 효과를 적용합니다.
    해당 효과는 디버프 제거 효과와 무적 효과를 무시합니다.
    """,
    AbilityCategory.Mythic,
    AbilityType.MYTHIC_SCP049_PANDEMIC,
    RoleAbility.Scp049)]
public class Pandemic : Ability
{
    private const float DamageInterval = 1f;
    private const float DebuffDuration = 0.2f;
    private const float EffectRefreshInterval = 0.1f;

    private CoroutineHandle _pandemicCoroutine;

    public override void OnEnabled()
    {
        _pandemicCoroutine = Timing.RunCoroutine(ApplyPandemic());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_pandemicCoroutine);
    }

    private IEnumerator<float> ApplyPandemic()
    {
        float elapsedDamageTime = DamageInterval;

        while (Owner.IsAlive)
        {
            bool applyDamage = elapsedDamageTime >= DamageInterval;

            foreach (Player player in PlayerManager.List.Where(IsEnemy))
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
                    player.Kill("사회가 당신과 거리를 두었습니다.");
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
}