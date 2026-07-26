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

[Ability("희생 명령", "SCP들에게 [<color=#FF00FF>영웅</color>]수어사이드 봄버맨, [<color=#FF00FF>영웅</color>]극독 능력을 지급합니다.", AbilityCategory.Epic, AbilityType.EPIC_SCP079_SUICIDEORDER, RoleAbility.Scp079)]
public class SuicideOrder : Ability
{
    public override void OnEnabled()
    {
        foreach (var scp in PlayerManager.List.Where(x => x.IsScpRole() && x.Role.Type != RoleTypeId.Scp079))
        {
            scp.AddAbility(AbilityType.EPIC_SUICIDEBOMBER);
            scp.AddAbility(AbilityType.EPIC_EXTREMEPOISON);
        }
    }

    public override void OnDisabled()
    {
    }
}