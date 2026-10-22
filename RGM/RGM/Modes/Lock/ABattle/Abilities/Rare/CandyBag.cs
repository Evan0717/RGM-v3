using Exiled.API.Extensions;
using InventorySystem.Items.Usables.Scp330;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Rare;

[Ability("사탕 봉지", "랜덤한 사탕을 6개 받습니다.", 
    AbilityCategory.Rare, AbilityType.RARE_HALLOWEEN_CANDYBAG, RoleAbility.None, false, AbilityHolidayType.Halloween)]
public class CandyBag : Ability
{
    public override void OnEnabled()
    {
        for (int i = 0; i < 6; i++)
        {
            Owner.AddCandy(Tools.EnumToList<CandyKindID>().GetRandomValue());
        }
    }
}
