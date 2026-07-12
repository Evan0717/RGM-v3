namespace RGM.Modes.Abilities.Dummy;

[Ability("기말고사 낙제", "안타깝네요, 낙제했습니다.", AbilityCategory.Dummy, AbilityType.DUMMY_FINALEXAMFAIL)]
public class FinalExamFail : Ability
{
    public override void OnEnabled()
    {
    }

    public override void OnDisabled()
    {
    }
}