using System.Linq;
using Exiled.API.Enums;
using PlayerRoles;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.Scp079.Rare;

[Ability("시스템 해킹", "아군 SCP의 투명도를 영구적으로 50% 증가시킵니다.(중첩 불가)", AbilityCategory.Rare, AbilityType.RARE_SCP079_SYSTEMHACKING, RoleAbility.Scp079)]
public class SystemHacking : Ability
{
    public override void OnEnabled()
    {
        foreach (var scp in PlayerManager.List.Where(x => x.IsScpRole() && x.Role.Type != RoleTypeId.Scp079))
        {
            scp.EnableEffect(EffectType.Fade, 127);
        }
    }

    public override void OnDisabled()
    {
    }
}
