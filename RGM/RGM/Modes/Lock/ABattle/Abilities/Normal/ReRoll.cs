using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using MEC;

namespace RGM.Modes.Abilities.Normal;

[Ability("리롤", "7초 동안 손이 잘립니다. 7초 후 랜덤한 일반 능력을 하나 획득합니다.", AbilityCategory.Common, AbilityType.NORMAL_REROLL)]
public class ReRoll : Ability
{
    public override void OnEnabled()
    {
        Owner.EnableEffect(EffectType.SeveredHands, 1, 7);

        Timing.CallDelayed(7f, () =>
        {
            Owner.AddAbility(ABattle.Instance.GetRandomAbilities(Owner, AbilityCategory.Common, 3).Where(x => x != AbilityType.NORMAL_REROLL).GetRandomValue());
        });
    }

    public override void OnDisabled()
    {
    }
}
