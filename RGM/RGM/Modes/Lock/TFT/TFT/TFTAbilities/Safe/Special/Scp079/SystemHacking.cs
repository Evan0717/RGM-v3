using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DAONTFT.Core.TFT.Safe.Scp079;

[TFTAbility("시스템 해킹", "아군 SCP의 투명도를 2분 간 90% 증가시킵니다.", TFTAbilityLevel.Safe, TFTAbilityCategory.Scp079, TFTAbilityPoint.Once, TFTAbilityType.SystemHacking, "💀")]
public class SystemHacking : TFTAbility
{
    public override void OnEnabled()
    {
        foreach (var scp in Player.List.Where(x => x.IsScp && x.Role.Type != RoleTypeId.Scp079))
        {
            scp.AddEffect(EffectType.Fade, 90, 120);
        }
    }

    public override void OnDisabled()
    {
    }
}
