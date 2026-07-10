using System.Collections.Generic;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Normal;

[Ability("보급", "탄약이 랜덤하게 3세트 지급됩니다.", AbilityCategory.Common, AbilityType.NORMAL_SUPPLY)]
public class Supply : Ability
{
    public override void OnEnabled()
    {
        List<ItemType> Ammos = new List<ItemType>
        {
            ItemType.Ammo12gauge,
            ItemType.Ammo44cal,
            ItemType.Ammo9x19,
            ItemType.Ammo556x45,
            ItemType.Ammo762x39
        };

        for (int i = 1; i < 4; i++)
            Owner.AddItem(Tools.GetRandomValue(Ammos));
    }

    public override void OnDisabled()
    {
    }
}
