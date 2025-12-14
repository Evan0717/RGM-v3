using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Sources;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
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

using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Unique.Scp079;

// [Ability("자폭 시퀸스", "즉시 핵 가동을 실시합니다. 이 핵은 중지할 수 없습니다.", AbilityCategory.Scp079, AbilityType.SCP079_STARTWARHEAD)]
public class StartWarhead : Ability
{
    public override void OnEnabled()
    {
        AutoNuke = true;

        Warhead.Start();
    }

    public override void OnDisabled()
    {
    }
}
