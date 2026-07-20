using System.Collections.Generic;

namespace RGM.Modes.Abilities.Unique.NTF;

[Ability("격리 의무자", "고폭 수류탄과 섬광탄을 지급받습니다.", AbilityCategory.Common, AbilityType.COMMON_NTF_QUARANTINEOBLIGATION, RoleAbility.NTF)]
public class QuarantineObligation : Ability
{
    public override void OnEnabled()
    {
        List<ItemType> ContainDuty = new List<ItemType>()
        {
            ItemType.GrenadeFlash,
            ItemType.GrenadeHE
        };

        foreach (var item in ContainDuty)
            Owner.AddItem(item);
    }

    public override void OnDisabled()
    {
    }
}
