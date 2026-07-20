namespace RGM.Modes.Abilities.Rare;

[Ability("도비는 자유에요", "석탄?을 지급받습니다.", AbilityCategory.Rare, AbilityType.RARE_DOBBYISFREE, RoleAbility.None, false, AbilityHolidayType.Christmas)]
public class DobbyIsFree : Ability
{
    public override void OnEnabled()
    {
        Owner.AddItem(ItemType.SpecialCoal);
    }

    public override void OnDisabled()
    {
    }
}
