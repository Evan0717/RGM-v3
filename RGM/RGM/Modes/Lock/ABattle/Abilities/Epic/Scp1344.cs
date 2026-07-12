using Exiled.API.Enums;

namespace RGM.Modes.Abilities.Epic;

[Ability("투시", "SCP-1344 효과를 받습니다.", AbilityCategory.Epic, AbilityType.EPIC_SCP1344)]
public class Scp1344 : Ability
{
    public override void OnEnabled()
    {
        Owner.EnableEffect(EffectType.Scp1344, 1);
    }

    public override void OnDisabled()
    {

    }
}
