using MEC;

namespace RGM.Modes.Abilities.Synergy;

[RequiresAbility(AbilityType.NORMAL_SWIFT, AbilityType.NORMAL_SNEAK, AbilityType.RARE_TRANSPARENTCLOAK)]
[Ability("암살자", "<경공, 잠행, 투명 망토> 더 빠르게 상대를 처단하세요. 경공 능력을 3개 더 받습니다.", AbilityCategory.Synergy, AbilityType.SYNERGY_ASSASSIN)]
public class Assassin : Ability
{
    public override void OnEnabled()
    {
        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            for (int i = 0; i < 3; i++)
                Owner.AddAbility(AbilityType.NORMAL_SWIFT);
        });
    }

    public override void OnDisabled()
    {
    }
}
