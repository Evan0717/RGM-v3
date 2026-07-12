using Exiled.API.Features.Items;

namespace RGM.Modes.Abilities.Unique.Tutorial;

[Ability("세치 혀", "SCP-1576을 지급받습니다.", AbilityCategory.Tutorial, AbilityType.TUTORIAL_TONGUE)]
public class Tongue : Ability
{
    public override void OnEnabled()
    {
        Item Scp1576 = Owner.AddItem(ItemType.SCP1576);
    }

    public override void OnDisabled()
    {
    }
}
