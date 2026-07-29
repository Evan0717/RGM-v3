using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

namespace RGM.Modes.Abilities.Unique.Scp079.Epic;

[Ability("가호", "자신을 포함한 SCP들에게 [<color=#FF00FF>영웅</color>]럭키비키, [일반] 행운 능력을 지급합니다.(79는 럭키비키만 지급)", AbilityCategory.Epic, AbilityType.EPIC_SCP079_SURVIVALORDER, RoleAbility.Scp079)]
public class Blessing : Ability
{
    public override void OnEnabled()
    {
        foreach (var scp in PlayerManager.List.Where(x => x.IsScpRole()))
        {
            scp.AddAbility(AbilityType.NORMAL_LUCKY);
            if (scp.Role != RoleTypeId.Scp079)  scp.AddAbility(AbilityType.EPIC_LUCKYVIKEY);
        }
    }

    public override void OnDisabled()
    {
    }
}
