using System.Collections.Generic;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Synergy;

[RequiresAbility(AbilityType.RARE_SPACETRAVEL, AbilityType.RARE_CONTRACT, AbilityType.RARE_STOPWATCH)]
[Ability("부자Ⅱ", "<이차원 도약, 계약, 회중시계> 랜덤코인 6개를 받으세요.", AbilityCategory.Synergy, AbilityType.SYNERGY_RICH2)]
public class Rich2 : Ability
{
    public override void OnEnabled()
    {
        List<string> uc = UsersManager.UsersCache[Owner.UserId];

        Owner.UserId.SetRC(4 + int.Parse(uc[1]), out string response);
    }

    public override void OnDisabled()
    {
    }
}
