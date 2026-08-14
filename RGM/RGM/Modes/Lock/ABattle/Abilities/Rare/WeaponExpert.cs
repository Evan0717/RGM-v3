using Exiled.API.Features.Items;
using UnityEngine;

namespace RGM.Modes.Abilities.Rare;

[Ability("무기 전문가", "SCP-1853을 받습니다.", AbilityCategory.Rare, AbilityType.RARE_WEAPONEXPERT)]
public class WeaponExpert : Ability
{
    public override void OnEnabled()
    {
        Owner.AddItem(ItemType.SCP1853);
        if (Random.Range(1, 101) <= 25) {
            Owner.AddItem(ItemType.SCP1853);
        }
    }

}
