using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using MEC;

namespace RGM.Modes.Abilities.Normal;

[Ability("리롤", "5초 동안 손이 잘립니다. 이후 랜덤한 일반 능력을 하나 획득합니다.", AbilityCategory.Normal, AbilityType.NORMAL_REROLL)]
public class ReRoll : Ability
{
    public override void OnEnabled()
    {
        Owner.EnableEffect(EffectType.SeveredHands, 1, 5);

        Timing.CallDelayed(5f, () =>
        {
            Owner.AddAbility(ABattle.Instance.GetRandomAbilities(Owner, AbilityCategory.Normal, 1, [AbilityType.NORMAL_REROLL]).First());
        });
    }
}
