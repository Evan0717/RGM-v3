using Exiled.API.Extensions;
using MEC;
using System.Collections.Generic;

namespace DAONTFT.Core.TFT.Euclid.Human;

[TFTAbility("무기고", "2분 뒤 특별한 무기 중 하나를 얻습니다. 해당 무기의 탄약을 4세트 받습니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Human, TFTAbilityPoint.Once, TFTAbilityType.Armory1, "📦")]
public class Armory1 : TFTAbility
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
        Timing.CallDelayed(120, () =>
        {
            Dictionary<ItemType, ItemType> items = new()
            {
                { ItemType.GunA7, ItemType.Ammo762x39 },
                { ItemType.GunCom45, ItemType.Ammo9x19 }
            };
            var item = items.GetRandomValue();

            Owner.AddItem(item.Key);
            Owner.AddItem(item.Value, 4);
        });

        yield break;
    }
}
