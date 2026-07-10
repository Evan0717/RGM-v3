namespace RGM.Modes.Abilities.Epic;

[Ability("마체테", "SCP-1509를 지급받습니다.", AbilityCategory.Epic, AbilityType.EPIC_SCP1509)]
public class Scp1509 : Ability
{
    public override void OnEnabled()
    {
        Owner.AddItem(ItemType.SCP1509);
    }

    public override void OnDisabled()
    {
    }
}
