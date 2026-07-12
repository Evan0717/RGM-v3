using MEC;

namespace DAONTFT.Core.TFT.Euclid.Human;

[TFTAbility("초재생", "100초 후, 안티 콜라를 2개 획득합니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Human, TFTAbilityPoint.Once, TFTAbilityType.AntiScp207, "🍹")]
public class AntiScp207 : TFTAbility
{
    public override void OnEnabled()
    {
        Timing.CallDelayed(100, () =>
        {
            Owner.AddItem(ItemType.AntiSCP207);
            Owner.AddItem(ItemType.AntiSCP207);
        });
    }

    public override void OnDisabled()
    {
    }
}
