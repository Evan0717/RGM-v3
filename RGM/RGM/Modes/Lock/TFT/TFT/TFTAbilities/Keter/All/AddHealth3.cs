namespace DAONTFT.Core.TFT.Keter.All;

[TFTAbility("운동 · 통달", "최대 체력 + 체력 -> +250❤️ (SCP x5)", TFTAbilityLevel.Keter, TFTAbilityCategory.All, TFTAbilityPoint.Once, TFTAbilityType.AddHealth3, "❤️")]
public class AddHealth3 : TFTAbility
{
    const float InitialHealth = 250f;
    public override void OnEnabled()
    {
        var health = Owner.IsScp ? InitialHealth * 5 : InitialHealth;
        Owner.MaxHealth += health;
        Owner.Health += health;
    }
    
    public override void OnDisabled()
    {
    }
}
