namespace RGM.Modes.Abilities.Dummy;

[Ability("자리 다비움", "자리 비움을 완료했습니다.", AbilityCategory.Dummy, AbilityType.DUMMY_NOAFK)]
public class NoAFK : Ability
{
    public override void OnEnabled()
    {
    }

    public override void OnDisabled()
    {
    }
}