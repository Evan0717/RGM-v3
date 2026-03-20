using System.Collections.Generic;
using System.Linq;
using DAONTFT.Core.Classes;
using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace DAONTFT.Core.TFT.Euclid.Human;

[TFTAbility("꿈나무+", "자신의 진영의 지원 영향력을 4 추가합니다. (죄수나 과학자의 경우 +2)", TFTAbilityLevel.Euclid, TFTAbilityCategory.Human, TFTAbilityPoint.Once, TFTAbilityType.Seed2, "🌳")]
public class Seed2 : TFTAbility
{
    public override void OnEnabled()
    {
        if (new List<RoleTypeId> 
        {
            RoleTypeId.ClassD,
            RoleTypeId.Scientist
        }.Contains(Owner.Role.Type))
        {
            Respawn.GrantInfluence(Owner.Role.Team.GetFaction(), 2);
        }

        Respawn.GrantInfluence(Owner.Role.Team.GetFaction(), 4);
    }

    public override void OnDisabled()
    {
    }
}
