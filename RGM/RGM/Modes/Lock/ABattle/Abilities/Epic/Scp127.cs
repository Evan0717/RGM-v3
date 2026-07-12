namespace RGM.Modes.Abilities.Epic;

[Ability("인생의 동반자", "SCP-127과 섬광탄을 얻습니다.", AbilityCategory.Epic, AbilityType.EPIC_SCP127)]
public class Scp127 : Ability
{
    public override void OnEnabled()
    {
        Owner.AddItem(ItemType.GrenadeFlash);
        Owner.AddItem(ItemType.GunSCP127);
    }

    public override void OnDisabled()
    {
    }
}
