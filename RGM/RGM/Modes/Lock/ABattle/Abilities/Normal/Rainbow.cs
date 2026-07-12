using InventorySystem.Items.Usables.Scp330;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Normal;

[Ability("무지개", "무지개 사탕이 포함된 SCP-330을 지급받습니다.", AbilityCategory.Common, AbilityType.NORMAL_RAINBOW)]
public class Rainbow : Ability
{
    public override void OnEnabled()
    {
        Owner.AddCandy(CandyKindID.Rainbow);
    }

    public override void OnDisabled()
    {
    }
}
