namespace DAONTFT.Core.TFT.Euclid.All;

[TFTAbility("운동Ⅱ", "최대 체력 + 체력 -> +100❤️ (SCP x6)", TFTAbilityLevel.Euclid, TFTAbilityCategory.All, TFTAbilityPoint.Once, TFTAbilityType.AddHealth2, "❤️")]
public class AddHealth2 : TFTAbility
{
    public override void OnEnabled()
    {
        float health = Owner.IsScp ? 100 * 6 : 100;
        Owner.MaxHealth += health;
        Owner.Health += health;
    }

    public override void OnDisabled()
    {
    }
}
