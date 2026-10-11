namespace RGM.Modes.Abilities.Epic;

[Ability("잠수부", "스테미나가 무제한이 됩니다.", AbilityCategory.Epic, AbilityType.EPIC_DIVER,
    isUnique: true)]
public class Diver : Ability
{
    public override void OnEnabled()
    {
        Owner.IsUsingStamina = false;
    }

    public override void OnDisabled()
    {
        Owner.IsUsingStamina = true;
    }
}
