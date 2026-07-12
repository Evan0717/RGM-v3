using Exiled.API.Features.Items;

namespace RGM.Modes.Abilities.Unique.CHI;

[Ability("혼돈의 카오스", "SCP-018을 지급받습니다.", AbilityCategory.CHI, AbilityType.CHI_CHAOSOFCHAOS)]
public class ChaosOfChaos : Ability
{
    public override void OnEnabled()
    {
        Item c = Owner.AddItem(ItemType.SCP018);
    }

    public override void OnDisabled()
    {
    }
}
