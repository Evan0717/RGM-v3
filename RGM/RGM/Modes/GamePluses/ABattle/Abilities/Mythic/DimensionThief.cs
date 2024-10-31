using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace RGM.Modes.Abilities.Mythic;

[Ability("차원 강탈자", "처치한 자의 능력을 모조리 흡수합니다!", AbilityCategory.Mythic, AbilityType.MYTHIC_DIMENSIONTHIEF)]
public class DimensionThief : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Died += OnDied;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Died -= OnDied;
    }

    public void OnDied(DiedEventArgs ev)
    {
        if (ev.Attacker == null || ev.Attacker != Owner)
            return;

        foreach (var Ability in ABattle.Instance.PlayerAbilities[ev.Player])
            ev.Attacker.AddAbility(Ability.Data.AbilityType);

        ev.Player.ShowHint("능력을 강탈당했습니다!");
    }
}