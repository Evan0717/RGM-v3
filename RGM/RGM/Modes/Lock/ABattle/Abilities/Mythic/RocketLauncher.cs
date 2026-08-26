using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.DataBases;
using RGM.API.Features;
using System.Collections.Generic;
using UnityEngine;

namespace RGM.Modes.Abilities.Mythic;

[Ability("로켓 런처", "공격 시, 10% 확률로 상대방을 하늘로 승천시킬 수 있습니다! (<color=red>SCP</color>는 40%)",
    AbilityCategory.Mythic, AbilityType.MYTHIC_ROCKETLAUNCHER)]
public class RocketLauncher : Ability
{
    private readonly List<Player> _isInRocket = [];

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
    }

    private void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Attacker == null || ev.Attacker != Owner || !HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub))
            return;

        if (_isInRocket.Contains(ev.Player)) return;
        if (ev.Attacker.IsScpRole())
        {
            if (Random.Range(1, 101) > 10) return;
            _isInRocket.Add(ev.Player);

            Timing.RunCoroutine(Tools.DoRocket(Owner, ev.Player, 1));
            Tools.MessageTranslated("", $"{ev.Player.DisplayNickname}(<color={ev.Player.Role.Color.ToHex()}>{( Trans.Role[ev.Player.Role.Type])}</color>)(이)가 하늘로 승천했습니다.");

            Timing.CallDelayed(1, () =>
            {
                _isInRocket.Remove(ev.Player);
            });
        }
        else
        {
            if (Random.Range(1, 101) > 40) return;
            _isInRocket.Add(ev.Player);

            Timing.RunCoroutine(Tools.DoRocket(Owner, ev.Player, 1));
            Tools.MessageTranslated("", $"{ev.Player.DisplayNickname}(<color={ev.Player.Role.Color.ToHex()}>{( Trans.Role[ev.Player.Role.Type])}</color>)(이)가 하늘로 승천했습니다.");

            Timing.CallDelayed(1, () =>
            {
                _isInRocket.Remove(ev.Player);
            });
        }
    }
}