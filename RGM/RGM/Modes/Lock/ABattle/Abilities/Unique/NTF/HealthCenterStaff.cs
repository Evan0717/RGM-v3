using System.Collections.Generic;
using System.Linq;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.NTF;

[Ability("보건소 직원", "주변에 있는 아군들에게 의료 아이템을 랜덤하게 지급합니다.", AbilityCategory.Common, AbilityType.COMMON_NTF_HEALTHCENTERSTAFF, RoleAbility.NTF)]
public class HealthCenterStaff : Ability
{
    public override void OnEnabled()
    {
        List<ItemType> HealItem = new List<ItemType>()
        {
            ItemType.Medkit,
            ItemType.Painkillers,
            ItemType.Adrenaline,
            ItemType.SCP500,
            ItemType.SCP330
        };

        foreach (var team in PlayerManager.List.Where(x => !x.IsNPC && x.IsAlive && x.LeadingTeam == Owner.LeadingTeam && Vector3.Distance(Owner.Position, x.Position) < 11))
            team.AddItem(Tools.GetRandomValue(HealItem));
    }

    public override void OnDisabled()
    {
    }
}
