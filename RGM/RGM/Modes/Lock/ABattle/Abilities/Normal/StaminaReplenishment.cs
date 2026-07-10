using InventorySystem.Items.Usables.Scp330;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Normal;

[Ability("체력 보충", "파란 사탕을 받습니다.", AbilityCategory.Common, AbilityType.NORMAL_STAMINAREPLENISHMENT)]
public class StaminaReplenishment : Ability
{
    public override void OnEnabled()
    {
        Owner.AddCandy(CandyKindID.Blue);
    }

    public override void OnDisabled()
    {
    }
}
