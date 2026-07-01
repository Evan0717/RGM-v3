using Exiled.API.Extensions;
using MEC;
using System.Collections.Generic;

namespace DAONTFT.Core.TFT.Keter.Human;

[TFTAbility("무기고+", "1분 뒤 특수 무기(제일버드, 입자분열기) 중 하나를 얻습니다.", TFTAbilityLevel.Keter, TFTAbilityCategory.Human, TFTAbilityPoint.Once, TFTAbilityType.Armory2, "📦")]
public class Armory2 : TFTAbility
{
    CoroutineHandle _waiting;

    public override void OnEnabled()
    {
        _waiting = Timing.RunCoroutine(waiting());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_waiting);
    }

    IEnumerator<float> waiting()
    {
        Timing.CallDelayed(60, () =>
        {
            List<ItemType> items = new()
            {
                ItemType.ParticleDisruptor,
                ItemType.Jailbird
            };

            Owner.AddItem(items.GetRandomValue());
        });

        yield break;
    }
}
