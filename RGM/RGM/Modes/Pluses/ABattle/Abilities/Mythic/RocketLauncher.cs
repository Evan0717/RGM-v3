using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.DataBases;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Mythic;

[Ability("로켓 런처", "공격 시, 5% 확률로 상대방을 하늘로 승천시킬 수 있습니다! (<color=red>SCP</color>는 20%)", AbilityCategory.Mythic, AbilityType.MYTHIC_ROCKETLAUNCHER)]
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
        if (ev.Attacker == null || ev.Attacker != Owner || !HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub))
            return;

        if (ev.Attacker.IsScp)
        {
            if (Random.Range(1, 6) == 1)
            {
                Timing.RunCoroutine(Tools.DoRocket(Owner, ev.Player, 1));
                Server.ExecuteCommand($"/cassie_sl {ev.Player.DisplayNickname}(<color={ev.Player.Role.Color.ToHex()}>{Trans.Role[ev.Player.Role.Type]}</color>)(이)가 하늘로 승천했습니다.");
            }
        }
        else
        {
            if (Random.Range(1, 21) == 1)
            {
                Timing.RunCoroutine(Tools.DoRocket(Owner, ev.Player, 1));
                Server.ExecuteCommand($"/cassie_sl {ev.Player.DisplayNickname}(<color={ev.Player.Role.Color.ToHex()}>{Trans.Role[ev.Player.Role.Type]}</color>)(이)가 하늘로 승천했습니다.");
            }
        }
    }
}