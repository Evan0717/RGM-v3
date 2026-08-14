using System.Collections.Generic;
using Exiled.API.Extensions;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Legend;

[Ability("마약 중독자", "8초마다 랜덤한 사탕이 지급됩니다.", AbilityCategory.Legend, AbilityType.LEGEND_CANDYADDICT)]
public class CandyAddict : Ability
{
    private CoroutineHandle _candyAddict;

    public override void OnEnabled() => _candyAddict = Timing.RunCoroutine(CandyParty());

    public override void OnDisabled() => Timing.KillCoroutines(_candyAddict);

    private IEnumerator<float> CandyParty()
    {
        while (true)
        {
            Owner.AddCandy(Tools.EnumToList<CandyKindID>().GetRandomValue());

            yield return Timing.WaitForSeconds(8f);
        } 
    }
}
