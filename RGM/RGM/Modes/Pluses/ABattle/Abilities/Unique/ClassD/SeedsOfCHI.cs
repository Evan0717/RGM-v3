using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.ClassD;

[Ability("반란의 씨앗", "CHI 스폰 영향력을 12 추가합니다.", AbilityCategory.ClassD, AbilityType.CLASSD_SEEDSOFCHI)]
public class SeedsOfCHI : Ability
{
    public override void OnEnabled()
    {
        Respawn.GrantInfluence(Faction.FoundationEnemy, 12);
    }

    public override void OnDisabled()
    {
    }
}
