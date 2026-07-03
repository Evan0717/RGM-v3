using System.Collections.Generic;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Legend;

[Ability("마약 중독자", "8초마다 랜덤한 사탕이 지급됩니다.", AbilityCategory.Legend, AbilityType.LEGEND_CANDYADDICT)]
public class CandyAddict : Ability
{
    CoroutineHandle _candyAddict;

    public override void OnEnabled()
    {
        _candyAddict = Timing.RunCoroutine(candyParty());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_candyAddict);
    }

    public IEnumerator<float> candyParty()
    {
        while (true)
        {
            Owner.AddCandy(Tools.GetRandomValue(Tools.EnumToList<CandyKindID>()));

            yield return Timing.WaitForSeconds(8f);
        } 
    }
}
