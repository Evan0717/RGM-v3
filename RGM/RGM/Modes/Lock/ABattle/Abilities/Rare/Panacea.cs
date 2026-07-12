using Exiled.API.Features.Items;

namespace RGM.Modes.Abilities.Rare;

[Ability("만병통치약", "SCP-500을 받습니다.", AbilityCategory.Rare, AbilityType.RARE_PANACEA)]
public class Panacea : Ability
{
    public override void OnEnabled()
    {
        Item scp500 = Owner.AddItem(ItemType.SCP500);
    }

    public override void OnDisabled()
    {
    }
}
