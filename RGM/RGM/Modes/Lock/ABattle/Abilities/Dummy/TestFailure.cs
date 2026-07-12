namespace RGM.Modes.Abilities.Dummy;

[Ability("시험 실패", "안타깝네요, 낙제했습니다.", AbilityCategory.Dummy, AbilityType.DUMMY_TESTFAILURE)]
public class TestFailure : Ability
{
    public override void OnEnabled()
    {
    }

    public override void OnDisabled()
    {
    }
}