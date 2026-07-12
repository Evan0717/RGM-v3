using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;
using System.Collections.Generic;

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

    public IEnumerator<float> OnDying(DyingEventArgs ev)
    {
        if (ev.Attacker == null || ev.Attacker != Owner)
            yield break;

        List<Ability> _abilities = ABattle.Instance.PlayerAbilities[ev.Player];

        yield return Timing.WaitForOneFrame;

        if (ev.Player.IsDead)
        {
            foreach (var _ability in _abilities)
                ev.Attacker.AddAbility(_ability.Data.AbilityType);

            ev.Player.AddHint("차원 강탈자", "능력을 강탈당했습니다!");
        }
    }
}