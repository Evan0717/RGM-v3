using System.Collections.Generic;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Synergy;

[RequiresAbility(AbilityType.NORMAL_ESCAPE, AbilityType.NORMAL_MILK, AbilityType.NORMAL_RUSH)]
[Ability("부자Ⅰ", "<위기 탈출, 우유, 황소> 랜덤코인 1개를 얻으세요.", AbilityCategory.Synergy, AbilityType.SYNERGY_RICH1)]
public class Rich1 : Ability
{
    public override void OnEnabled()
    {
        List<string> uc = UsersManager.UsersCache[Owner.UserId];

        Owner.UserId.SetRC(1 + int.Parse(uc[1]), out string response);
    }

    public override void OnDisabled()
    {
    }
}
