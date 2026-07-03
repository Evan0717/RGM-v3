using UnityEngine;

namespace DAONTFT.Core.TFT.Euclid.Human;

[TFTAbility("소인화 · 숙련", "몸의 크기가 16% 줄어듭니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Human, TFTAbilityPoint.Once, TFTAbilityType.Miniaturization2, "👤")]
public class Miniaturization2 : TFTAbility
{
    public override void OnEnabled()
    {
        Owner.Scale = new Vector3(0.84f, 0.84f, 0.84f);
    }

    public override void OnDisabled()
    {
    }
}