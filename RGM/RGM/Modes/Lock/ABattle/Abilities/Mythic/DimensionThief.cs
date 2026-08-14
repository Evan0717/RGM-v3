using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;
using System.Collections.Generic;
using System.Linq;

namespace RGM.Modes.Abilities.Mythic;

[Ability("차원 강탈자", "처치한 자의 능력을 모조리 흡수합니다!", AbilityCategory.Mythic, AbilityType.MYTHIC_DIMENSIONTHIEF)]
public class DimensionThief : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Dying += OnDying;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Dying -= OnDying;
    }

    private IEnumerator<float> OnDying(DyingEventArgs ev)
    {
        if (ev.Attacker == null || ev.Attacker != Owner)
            yield break;

        // 처치 시점의 능력 타입을 고정값으로 스냅샷 (반사경/복제 연쇄로 개수가 늘지 않도록)
        List<AbilityType> abilityTypes =
        [
            .. ABattle.Instance.PlayerAbilities[ev.Player]
                .Select(a => a.Data.AbilityType)
        ];

        yield return Timing.WaitForOneFrame;

        if (!ev.Player.IsDead) yield break;
        foreach (var abilityType in abilityTypes)
            ABattle.Instance.AddAbility(ev.Attacker, abilityType, allowReflector: false);

        ev.Player.AddHint("차원 강탈자", "능력을 강탈당했습니다!");
    }
}
