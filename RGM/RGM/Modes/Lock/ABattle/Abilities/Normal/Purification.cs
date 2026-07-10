using InventorySystem.Items.Usables.Scp330;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Normal;

[Ability("정화", "초록 사탕이 포함된 SCP-330을 지급받습니다.", AbilityCategory.Common, AbilityType.NORMAL_PURIFICATION)]
public class Purification : Ability
{
    public override void OnEnabled()
    {
        Owner.AddCandy(CandyKindID.Green);
    }

    public override void OnDisabled()
    {
    }
}
