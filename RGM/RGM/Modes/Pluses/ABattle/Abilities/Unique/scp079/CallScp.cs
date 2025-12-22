using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Sources;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using Exiled.Events.EventArgs.Scp106;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using PlayerRoles;
using RGM.API.DataBases;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.Scp079;

// [Ability("SCP 지원 호출기", "A.I. 지능이 탑재된 SCP를 1개체 부릅니다. 이 개체는 모든 데미지를 50%만 받습니다.", AbilityCategory.Scp079, AbilityType.SCP079_CALLSCP)]
public class CallScp : Ability
{
    public override void OnEnabled()
    {
        RoleTypeId _role = Tools.GetRandomValue(Datas.AIRoles.Where(x => !PlayerManager.List.ToList().Where(x => x.IsNPC).Select(x1 => x1.Role.Type).ToList().Contains(x)).ToList());

        Server.ExecuteCommand($"/spawnai {_role.ToString()}");

        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
    }

    public void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Player.IsScpRole() && ev.Player.IsNPC)
            ev.DamageHandler.Damage /= 2;
    }
}
