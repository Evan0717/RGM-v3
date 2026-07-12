using System.Collections.Generic;
using Exiled.API.Enums;
using MEC;

namespace RGM.Modes.Abilities.Epic;

[Ability("잠수부", "시야가 개선되고 스테미나가 줄어들지 않습니다.", AbilityCategory.Epic, AbilityType.EPIC_DIVER)]
public class Diver : Ability
{
    CoroutineHandle _diverRotation;

    public override void OnEnabled()
    {
        _diverRotation = Timing.RunCoroutine(DiverRotation());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_diverRotation);

        Owner.DisableEffect(EffectType.Invigorated);
    }

    public IEnumerator<float> DiverRotation()
    {
        while (true)
        {
            Owner.EnableEffect(EffectType.Invigorated);

            yield return Timing.WaitForSeconds(1f);
        }
    }
}
