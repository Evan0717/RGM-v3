using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Sources;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp096;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.DataBases;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.Scp096;

[Ability("천리안", "분노 시에 30m 내의 인간들을 목격자에 포함시킵니다. (최대 4명)", AbilityCategory.Scp096, AbilityType.SCP096_SEER)]
public class Sear : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp096.Enraging += OnEnraging;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp096.Enraging -= OnEnraging;
    }

    public void OnEnraging(EnragingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        int Stack = 0;

        foreach (var player in PlayerManager.List.Where(x => x.IsHuman))
        {
            if (Stack == 4)
                break;

            if (Vector3.Distance(player.Position, ev.Player.Position) < 31)
            {
                Stack += 1;

                ev.Scp096.AddTarget(player);

                player.AddHint("천리안", $"<color={ABattle.RatingColor["전용"]}>천리안</color>에 의해 강제로 목격자에 포함되었습니다. 도망가세요!");
            }
        }

        ev.Player.AddHint("천리안", $"<color={ABattle.RatingColor["전용"]}>천리안</color> 능력으로 {Stack}명의 인간들을 추가로 탐색했습니다.");
    }
}
