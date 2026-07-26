using System.Collections.Generic;

namespace RGM.Modes.Abilities.Unique.NTF;

[Ability("관리 의무자", " E11sr, 방탄복, 5.56mm 3세트, 섬광탄을 지급받습니다.", AbilityCategory.Common, AbilityType.NORMAL_NTF_MANAGERIALOBLIGATIONPERSON, RoleAbility.NTF)]
public class ManagerialObligationPerson : Ability
{
    public override void OnEnabled()
    {
        List<ItemType> manageDuty =
        [
            ItemType.GunE11SR,
            ItemType.ArmorCombat,
            ItemType.GrenadeFlash,
            ItemType.Ammo556x45,
            ItemType.Ammo556x45,
            ItemType.Ammo556x45
        ];

        foreach (var item in manageDuty)
            Owner.AddItem(item);
    }

    public override void OnDisabled()
    {
    }
}
