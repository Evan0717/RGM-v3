using System.Linq;
using Exiled.API.Enums;
using PlayerRoles;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.Scp079;

[Ability("시스템 해킹", "아군 SCP의 투명도를 2분 간 36% 증가시킵니다.", AbilityCategory.Scp079, AbilityType.SCP079_SYSTEMHACKING)]
public class SystemHacking : Ability
{
    public override void OnEnabled()
    {
        foreach (var scp in PlayerManager.List.Where(x => x.IsScpRole() && x.Role.Type != RoleTypeId.Scp079))
        {
            scp.AddEffect(EffectType.Fade, 92, 120);
        }
    }

    public override void OnDisabled()
    {
    }
}
