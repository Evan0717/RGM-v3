namespace RGM.Modes.Abilities.Unique.Tutorial;

[Ability("세치 혀", "SCP-1576을 지급받습니다.", AbilityCategory.Normal, AbilityType.NORMAL_TUTORIAL_TONGUE, RoleAbility.Tutorial)]
public class Tongue : Ability
{
    public override void OnEnabled()
    {
        Owner.AddItem(ItemType.SCP1576);
    }
}
