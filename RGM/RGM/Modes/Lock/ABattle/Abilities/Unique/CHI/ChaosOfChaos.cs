namespace RGM.Modes.Abilities.Unique.CHI;

[Ability("혼돈의 카오스", "SCP-018을 2개 지급받습니다.", AbilityCategory.Normal, AbilityType.NORMAL_CHI_CHAOSOFCHAOS, RoleAbility.CHI)]
public class ChaosOfChaos : Ability
{
    public override void OnEnabled()
    {
        for (int i = 0; i < 2; i++) {
            Owner.AddItem(ItemType.SCP018);
        }
    }
}
