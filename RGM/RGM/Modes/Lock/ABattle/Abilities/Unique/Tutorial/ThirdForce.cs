using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.Tutorial;

[Ability("제 3세력", "뱀의 손 지원을 3명 더 부릅니다.", AbilityCategory.Common, AbilityType.NORMAL_TUTORIAL_THIRDFORCE, RoleAbility.Tutorial)]
public class ThirdForce : Ability
{
    public override void OnEnabled()
    {
        List<Player> deadPlayers = [.. PlayerManager.List.Where(x => x.IsDead)];
        deadPlayers.ShuffleList();

        Tools.CallSnakeHand(Owner, [.. deadPlayers.Take(3)]);
    }
}
