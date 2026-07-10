using System.Collections.Generic;
using Exiled.API.Extensions;
using MEC;

namespace RGM.Modes.Abilities.Unique.Scp079;

[Ability("자동 방어 시스템", "테슬라가 가끔씩 자동으로 작동됩니다.", AbilityCategory.Scp079, AbilityType.SCP079_AUTOTESLA)]
public class AutoTesla : Ability
{
    CoroutineHandle _autoTesla;

    public override void OnEnabled()
    {
        _autoTesla = Timing.RunCoroutine(autoTesla());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_autoTesla);
    }

    IEnumerator<float> autoTesla()
    {
        while (true)
        {
            LabApi.Features.Wrappers.Tesla tesla = LabApi.Features.Wrappers.Tesla.List.GetRandomValue();
            tesla.Trigger();
            tesla.InstantTrigger();

            yield return Timing.WaitForSeconds(UnityEngine.Random.Range(1, 60));
        }
    }
}
