using RGM.API.Features;

namespace RGM.Modes.Abilities.Normal;

[Ability("랜덤박스", "랜덤한 아이템을 지급받습니다.", AbilityCategory.Common, AbilityType.NORMAL_RANDOMBOX)]
public class RandomBox : Ability
{
    public override void OnEnabled()
    {
        Owner.AddRandomItem();
    }

    public override void OnDisabled()
    {

    }
}
