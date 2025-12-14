using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Mythic;

[Ability("조커", "사망할 경우 3초간 무적이 되고, 최대 체력이 1~3배로 조정되고, 상대방의 능력 1개를 삭제시키고, 좋은 능력 2개를 얻습니다.", AbilityCategory.Mythic, AbilityType.MYTHIC_JOKER)]
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

        Timing.CallDelayed(3f, () =>
        {
            if (GodModePlayers.Contains(ev.Player))
                GodModePlayers.Remove(ev.Player);
        });

        ev.Player.MaxHealth = Random.Range(1, 4) * ev.Player.MaxHealth;

        if (ev.Attacker != null)
            ev.Attacker.RemoveAbility(ev.Attacker.GetAbilities().GetRandomValue());

        foreach (var ability in ABattle.Instance.GetRandomAbilities(AbilityCategory.Legend, 2))
            ev.Player.AddAbility(ability);

        OnDisabled();
    }
}