namespace RGM.Modes.Abilities.Unique.Scientist;

[Ability("공학 전공", "SCP-2176을 지급받습니다.", AbilityCategory.Normal, AbilityType.NORMAL_SCIENTIST_ENGINEERINGMAJOR, RoleAbility.Scientist)]
public class EngineeringMajor : Ability
{
    public override void OnEnabled()
    {
        Owner.AddItem(ItemType.SCP2176);
    }
}
