using MEC;

namespace RGM.Modes.Abilities.Synergy;

[RequiresAbility(AbilityType.NORMAL_WORKOUT, AbilityType.NORMAL_TRAINING, AbilityType.NORMAL_SWIFT, AbilityType.NORMAL_EVOLUTION)]
[Ability("4대 운동", "<운동, 진화, 경공, 단련> 4대 운동을 모두 마쳤습니다! 능력을 하나 더 획득할 시간입니다.", AbilityCategory.Synergy, AbilityType.SYNERGY_FOURMAJOREXERCISES)]
public class FourMajorExercises : Ability
{
    public override void OnEnabled()
    {
        Timing.CallDelayed(1, () =>
        {
            ABattle.Instance.StartSelect(Owner);
        });
    }

    public override void OnDisabled()
    {
    }
}
