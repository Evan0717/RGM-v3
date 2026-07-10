using System.Linq;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.Scp173;

[Ability("공포", "인간을 죽이면 근처에 있는 인간들이 0.75초 동안 움직일 수 없게 됩니다.", AbilityCategory.Scp173, AbilityType.SCP173_FEAR)]
public class Fear : Ability
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

        foreach (var player in PlayerManager.List.Where(x => !x.IsNPC && !x.IsScpRole()))
        {
            if (Vector3.Distance(player.Position, ev.Attacker.Position) <= 10)
            {
                player.EnableEffect(EffectType.Ensnared, 1, 0.75f * Owner.AbilityCount(AbilityType.SCP173_FEAR));
            }
        }
    }
}
