using Exiled.API.Extensions;
using MEC;

namespace RGM.Modes.Abilities.Synergy;

[RequiresAbility(AbilityType.DUMMY_LEGENDTRANSITIONSUCCESS, AbilityType.DUMMY_EPICTRANSITIONSUCCESS, AbilityType.DUMMY_RARETRANSITIONSUCCESS)]
[Ability("승리자", "<하급 변이 성공, 변이 성공, 상급 변이 성공> 오늘은 운수가 좋군요.", AbilityCategory.Synergy, AbilityType.SYNERGY_WINNER)]
public class Winner : Ability
{
    public override void OnEnabled()
    {
        Timing.CallDelayed(1, () =>
        {
            Owner.AddAbility(ABattle.Instance.GetRandomAbilities(Owner, AbilityCategory.Mythic, 2).GetRandomValue());
        });
    }

    public override void OnDisabled()
    {
    }
}
