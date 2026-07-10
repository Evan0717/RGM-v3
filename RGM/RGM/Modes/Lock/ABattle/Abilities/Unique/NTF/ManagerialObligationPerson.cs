using System.Collections.Generic;

namespace RGM.Modes.Abilities.Unique.NTF;

[Ability("관리 의무자", "손전등, Crossvec, 9x19mm 2세트, 섬광탄을 지급받습니다.", AbilityCategory.NTF, AbilityType.NTF_MANAGERIALOBLIGATIONPERSON)]
public class ManagerialObligationPerson : Ability
{
    public override void OnEnabled()
    {
        List<ItemType> ManageDuty = new List<ItemType>()
        {
            ItemType.GunCrossvec,
            ItemType.Flashlight,
            ItemType.Ammo9x19,
            ItemType.Ammo9x19,
            ItemType.Flashlight,
        };

        foreach (var item in ManageDuty)
            Owner.AddItem(item);
    }

    public override void OnDisabled()
    {
    }
}
