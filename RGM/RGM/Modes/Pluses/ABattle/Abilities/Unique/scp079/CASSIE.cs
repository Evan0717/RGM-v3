using CommandSystem;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Sources;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.Scp079;

[Ability("C.A.S.S.I.E.", "서버 내의 모두에게 말을 전달할 수 있는 [.캐시 (할 말}] 명령어 사용권을 1회 추가합니다.", AbilityCategory.Scp079, AbilityType.SCP079_CASSIE)]
public class CASSIE : Ability
{
    public override void OnEnabled()
    {
        if (ABattleVar.CASSIE.ContainsKey(Owner))
            ABattleVar.CASSIE[Owner]++;

        else
            ABattleVar.CASSIE.Add(Owner, 1);
    }

    public override void OnDisabled()
    {
    }
}
