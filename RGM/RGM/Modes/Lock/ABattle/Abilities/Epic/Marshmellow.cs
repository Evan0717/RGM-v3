namespace RGM.Modes.Abilities.Epic;

[Ability("!!마쉬멜로우!!", "즉시 마쉬멜로우맨이 됩니다. 체력은 500이며, \"경공\" 능력을 2개 얻습니다.", AbilityCategory.Epic, AbilityType.EPIC_MARSHMELLOW, AbilityHolidayType.Halloween)]
public class MarshMellow : Ability
{
    public override void OnEnabled()
    {
        Owner.MaxHealth += 500;
        Owner.Health += 500;
        Owner.AddAbility(AbilityType.NORMAL_SWIFT);
        Owner.AddAbility(AbilityType.NORMAL_SWIFT);
        Owner.AddItem(ItemType.MarshmallowItem);
    }

    public override void OnDisabled()
    {
    }
}
