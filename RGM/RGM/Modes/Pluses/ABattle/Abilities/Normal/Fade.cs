using System.Collections.Generic;
using Exiled.API.Enums;

namespace RGM.Modes.Abilities.Normal;

[Ability("낮은 존재감", "투명도가 10% 증가합니다.", AbilityCategory.Common, AbilityType.NORMAL_FADE)]
public class Fade : Ability
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.Fade, 10);
    }

    public override void OnDisabled()
    {
    }
}