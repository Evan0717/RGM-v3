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

namespace DAONTFT.Core.TFT.Safe.Human;

[TFTAbility("꿈나무", "자신의 진영의 지원 영향력을 2 추가합니다. (죄수나 과학자의 경우 +1)", TFTAbilityLevel.Safe, TFTAbilityCategory.Human, TFTAbilityPoint.Once, TFTAbilityType.Seed1, "🌳")]
public class Seed1 : TFTAbility
{
    public override void OnEnabled()
    {
        if (new List<RoleTypeId>
        {
            RoleTypeId.ClassD,
            RoleTypeId.Scientist
        }.Contains(Owner.Role.Type))
        {
            Respawn.GrantInfluence(Owner.Role.Team.GetFaction(), 1);
        }

        Respawn.GrantInfluence(Owner.Role.Team.GetFaction(), 2);
    }

    public override void OnDisabled()
    {
    }
}
