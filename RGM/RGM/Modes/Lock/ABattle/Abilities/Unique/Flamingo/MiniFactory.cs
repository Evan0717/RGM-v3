using System.Collections.Generic;
using System.Linq;
using Exiled.API.Extensions;
using Exiled.API.Features;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.Flamingo;

[Ability("미니 공장", "죽은 플레이어 중 하나를 플라밍고로 부활시킵니다.", AbilityCategory.Common, AbilityType.COMMON_FLAMINGO_MINIFACTORY, RoleAbility.Flamingo)]
public class MiniFactory : Ability
{
    public override void OnEnabled()
    {
        List<Player> deadPlayers = PlayerManager.List.Where(x => x.IsDead).ToList();
        Player player = deadPlayers.GetRandomValue();

        player.Role.Set(Owner.Role.Type);
        player.Position = Owner.Position;
    }

    public override void OnDisabled()
    {
    }
}
