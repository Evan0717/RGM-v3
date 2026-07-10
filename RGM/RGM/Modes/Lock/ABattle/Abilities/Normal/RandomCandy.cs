using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Normal;

[Ability("트릭 오어 트릿", "랜덤한 SCP-330을 받습니다. 운이 좋다면 더 받을수도 있겠죠..", AbilityCategory.Common, AbilityType.NORMAL_RANDOMCANDY)]
public class RandomCandy : Ability
{
    public override void OnEnabled()
    {
        Owner.AddRandomCandy();

        if (Random.Range(1, 8) == 1)
            Owner.AddRandomCandy();
    }

    public override void OnDisabled()
    {
    }
}
