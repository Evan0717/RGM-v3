using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Mythic;

[Ability("조커", "사망할 경우 5초간 무적이 되고, 최대 체력이 2~4배로 조정되고, 상대방의 능력 1개를 삭제시키고,\n전설 이상 등급 능력 4개를 얻습니다.", AbilityCategory.Mythic, AbilityType.MYTHIC_JOKER)]
public class Joker : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Dying += OnDying;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Dying -= OnDying;
    }

    private void OnDying(DyingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        ev.IsAllowed = false;

        GodModePlayers.Add(ev.Player);

        Timing.CallDelayed(5f, () =>
        {
            if (GodModePlayers.Contains(ev.Player))
                GodModePlayers.Remove(ev.Player);
        });

        ev.Player.MaxHealth = Random.Range(2, 5) * ev.Player.MaxHealth;

        if (ev.Attacker != null)
            ev.Attacker.RemoveAbility(ev.Attacker.GetAbilities().GetRandomValue());

        foreach (var ability in ABattle.Instance.GetRandomAbilities(Owner, AbilityCategory.Legend, 4))
            ev.Player.AddAbility(ability);

        OnDisabled();
    }
}