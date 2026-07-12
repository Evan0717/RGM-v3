using System.Linq;
using Exiled.API.Extensions;
using PlayerRoles;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Epic;

[Ability("성직자", "관전석에서 3명을 뽑아 아군으로 편입합니다. (SCP의 경우 SCP-049-2로 대체)", AbilityCategory.Epic, AbilityType.EPIC_PRIEST)]
public class Priest : Ability
{
    public override void OnEnabled()
    {
        for (int i = 0; i < 3; i++)
        {
            var dead = PlayerManager.List.Where(x => x.IsDead).ToList();

            if (dead.Count() != 0)
            {
                var revive = dead.GetRandomValue();

                if (Owner.IsScpRole())
                    revive.Role.Set(RoleTypeId.Scp0492);

                else
                    revive.Role.Set(Owner.Role.Type);

                revive.Position = Owner.Position;
            }
        }
    }

    public override void OnDisabled()
    {

    }
}
