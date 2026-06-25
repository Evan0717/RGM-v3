namespace DAONTFT.Core.TFT.Safe.All;

[TFTAbility("운동Ⅰ", "최대 체력 + 체력 -> +40❤️ (SCP x8)", TFTAbilityLevel.Safe, TFTAbilityCategory.All, TFTAbilityPoint.Once, TFTAbilityType.AddHealth1, "❤️")]
public class AddHealth1 : TFTAbility
{
    public override void OnEnabled()
    {
        float health = Owner.IsScp ? 40 * 8 : 40;
        Owner.MaxHealth += health;
        Owner.Health += health;
    }

    public override void OnDisabled()
    {
    }
}
