using System.Collections.Generic;
using MEC;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Rare;

[Ability("연금", "15초마다 랜덤한 아이템을 1개 획득합니다. (8회 발동 가능)", AbilityCategory.Rare, AbilityType.RARE_ALCHEMY)]
public class Alchemy : Ability
{
    private CoroutineHandle _onStarted;

    public override void OnEnabled()
    {
        _onStarted = Timing.RunCoroutine(OnStarted());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_onStarted);
    }

    private IEnumerator<float> OnStarted()
    {
        for (int i = 0; i < 8; i++)
        {
            Owner.AddRandomItem();

            yield return Timing.WaitForSeconds(15f);
        }
    }
}
