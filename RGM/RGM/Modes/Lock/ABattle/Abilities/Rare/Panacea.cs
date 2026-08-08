using UnityEngine;

namespace RGM.Modes.Abilities.Rare;

[Ability("만병통치약", "SCP-500을 받습니다. 20% 확률로 1개를 추가로 받습니다.", AbilityCategory.Rare, AbilityType.RARE_PANACEA)]
public class Panacea : Ability
{
    public override void OnEnabled()
    {
        Owner.AddItem(ItemType.SCP500);
        if (Random.Range(1, 101) <= 20) Owner.AddItem(ItemType.SCP500);
    }

    public override void OnDisabled()
    {
    }
}
