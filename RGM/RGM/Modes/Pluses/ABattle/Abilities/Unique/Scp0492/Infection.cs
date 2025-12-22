using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Sources;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using PlayerRoles;
using RGM.API.DataBases;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.Scp0492;

[Ability("감염", "사망시키면 같은 진영으로 만듭니다.", AbilityCategory.Scp0492, AbilityType.SCP_0492_INFECTION)]
public class Infection : Ability
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

        if (ev.Attacker.IsScpRole())
        {
            ev.Player.Role.Set(RoleTypeId.Scp0492, RoleSpawnFlags.None);
        }
        else
        {
            ev.Player.Role.Set(ev.Attacker.Role.Type, RoleSpawnFlags.None);
        }

        ev.Player.AddHint("감염", $"{ev.Attacker.DisplayNickname}에 의해 감염되었습니다.", 5);
    }
}
