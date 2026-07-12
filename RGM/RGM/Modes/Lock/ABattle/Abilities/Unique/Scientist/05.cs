namespace RGM.Modes.Abilities.Unique.Scientist;

[Ability("05 평의회", "05등급 키카드를 지급받습니다.", AbilityCategory.Scientist, AbilityType.SCIENTIST_05)]
public class Level05 : Ability
{
    public override void OnEnabled()
    {
        Owner.AddItem(ItemType.KeycardO5);
    }

    public override void OnDisabled()
    {
    }
}
