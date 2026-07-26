using System.Collections.Generic;
using System.Linq;
using Exiled.API.Extensions;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.NTF;

[Ability("보건소 직원", "모든 아군들에게 SCP-500을 지급합니다.", AbilityCategory.Common, AbilityType.NORMAL_NTF_HEALTHCENTERSTAFF, RoleAbility.NTF)]
public class HealthCenterStaff : Ability
{
    public override void OnEnabled()
    {
        List<ItemType> healItem =
        [
            ItemType.Medkit,
            ItemType.Painkillers,
            ItemType.Adrenaline,
            ItemType.SCP500,
            ItemType.SCP330
        ];

        foreach (var team in PlayerManager.List.Where(x => !x.IsNPC && x.IsAlive && x.LeadingTeam == Owner.LeadingTeam && Vector3.Distance(Owner.Position, x.Position) < 11))
            team.AddItem(healItem.GetRandomValue());
    }
}
