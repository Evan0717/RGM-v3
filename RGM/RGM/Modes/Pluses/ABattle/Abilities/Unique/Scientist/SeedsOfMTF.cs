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

namespace RGM.Modes.Abilities.Unique.Scientist;

[Ability("구미호의 씨앗", "NTF 스폰 영향력을 12 추가합니다.", AbilityCategory.Scientist, AbilityType.SCIENTIST_SEEDSOFMTF)]
public class SeedsOfMTF : Ability
{
    public override void OnEnabled()
    {
        Respawn.GrantInfluence(Faction.FoundationStaff, 12);
    }

    public override void OnDisabled()
    {
    }
}
