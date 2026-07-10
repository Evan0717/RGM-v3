namespace RGM.Modes.Abilities.Normal;

[Ability("횃불", "랜턴과 아드레날린을 받습니다.", AbilityCategory.Common, AbilityType.NORMAL_TORCH)]
public class Torch : Ability
{
    public override void OnEnabled()
    {
        Owner.AddItem(ItemType.Lantern);
        Owner.AddItem(ItemType.Adrenaline);
    }

    public override void OnDisabled()
    {
    }
}
