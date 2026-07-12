using MEC;

namespace DAONTFT.Core.TFT.Euclid.Human;

[TFTAbility("무기 전문가", "100초 후, 녹즙을 2개 획득합니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Human, TFTAbilityPoint.Once, TFTAbilityType.Scp1853, "🔫")]
public class Scp1853 : TFTAbility
{
    public override void OnEnabled()
    {
        Timing.CallDelayed(60, () =>
        {
            Owner.AddItem(ItemType.SCP1853);
            Owner.AddItem(ItemType.SCP1853);
        });
    }

    public override void OnDisabled()
    {
    }
}
