namespace RGM.Modes.Abilities.Dummy;

[Ability("대학수학능력시험 9등급", "안타깝네요, 낙제했습니다.", AbilityCategory.Dummy, AbilityType.DUMMY_CSTCFAIL)]
public class CSTCFail : Ability
{
    public override void OnEnabled()
    {
    }

    public override void OnDisabled()
    {
    }
}