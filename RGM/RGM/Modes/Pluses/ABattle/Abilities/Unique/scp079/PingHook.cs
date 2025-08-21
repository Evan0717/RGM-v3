using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Sources;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using Exiled.Events.EventArgs.Scp106;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.DataBases;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.Scp079;

[Ability("핑 갈고리", "다음 핑의 위치에 랜덤한 인간을 소환시킵니다.", AbilityCategory.Scp079, AbilityType.SCP079_PINGHOOK)]
public class PingHook : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp079.Pinging += OnPinging;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp079.Pinging -= OnPinging;
    }

    public void OnPinging(PingingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        OnDisabled();

        Vector3 pos = ev.Position;

        Player.List.Where(x => x.IsAlive && !x.IsNPC && !x.IsScp).GetRandomValue().Position = new Vector3(pos.x, pos.y + 2, pos.z);
    }
}
