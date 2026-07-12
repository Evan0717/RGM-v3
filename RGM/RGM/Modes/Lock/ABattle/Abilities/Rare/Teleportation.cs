using System.Linq;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Rare;

[Ability("순간이동", "랜덤한 유저의 위치로 순간이동합니다.", AbilityCategory.Rare, AbilityType.RARE_TELEPORTATION)]
public class Teleportation : Ability
{
    public override void OnEnabled()
    {
        Player target = Tools.GetRandomValue(PlayerManager.List.Where(x => x != Owner && x.IsAlive && x.Role.Type != RoleTypeId.Scp079).ToList());
        Owner.Position = target.Position;

        Timing.CallDelayed(1, () =>
        {
            Owner.RemoveAbility(this);
            Owner.AddAbility(AbilityType.DUMMY_TELEPORTED);
        });
    }

    public override void OnDisabled()
    {
    }
}
