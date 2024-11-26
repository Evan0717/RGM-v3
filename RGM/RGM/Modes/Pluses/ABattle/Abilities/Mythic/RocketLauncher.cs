using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace RGM.Modes.Abilities.Mythic;

[Ability("로켓 런처", "공격 시, 20% 확률로 상대방을 하늘로 승천시킬 수 있습니다!", AbilityCategory.Mythic, AbilityType.MYTHIC_ROCKETLAUNCHER)]
public class RocketLauncher : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
    }

    public void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Attacker == null || ev.Attacker != Owner)
            return;

        if (Random.Range(1, 6) == 1)
            Server.ExecuteCommand($"/rocket {ev.Player.Id} 1");
    }
}