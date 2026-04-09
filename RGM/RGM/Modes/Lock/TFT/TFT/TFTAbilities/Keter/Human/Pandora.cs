using Exiled.API.Extensions;
using MEC;
using RGM.API.Features;
using System.Collections.Generic;
using System.Linq;

namespace DAONTFT.Core.TFT.Keter.Human;

[TFTAbility("판도라의 인벤토리", "1분마다 인벤토리의 아이템들이 유형이 같은 무작위 아이템들로 교체됩니다.", TFTAbilityLevel.Keter, TFTAbilityCategory.Human, TFTAbilityPoint.Continuous, TFTAbilityType.Pandora, "❓")]
public class Pandora : TFTAbility
{
    CoroutineHandle _loop;

    public override void OnEnabled()
    {
        _loop = Timing.RunCoroutine(loop());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_loop);
    }

    IEnumerator<float> loop()
    {
        while (true)
        {
            foreach (var item in Owner.Items.ToList())
            {
                ItemCategory category = item.Category;

                Owner.RemoveItem(item);
                Owner.AddItem(Tools.EnumToList<ItemType>().Where(x => x.GetCategory() == category).GetRandomValue());
            }

            yield return Timing.WaitForSeconds(60);
        }
    }
}
