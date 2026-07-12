using System.Collections.Generic;
using Exiled.API.Enums;
using MEC;

namespace RGM.Modes.Abilities.Epic;

[Ability("고스트룰", "유령이 되어 문을 통과할 수 있게 됩니다.", AbilityCategory.Epic, AbilityType.EPIC_GHOSTRULE)]
public class GhostRule : Ability
{
    CoroutineHandle _ghostRotation;
    public override void OnEnabled()
    {
        _ghostRotation = Timing.RunCoroutine(GhostRotation());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_ghostRotation);
        Owner.DisableEffect(EffectType.Ghostly);
    }
    
    public IEnumerator<float> GhostRotation()
    {
        while (true)
        {
            Owner.EnableEffect(EffectType.Ghostly);

            yield return Timing.WaitForSeconds(1f);
        }
    }
}
