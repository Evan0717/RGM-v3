using System.Collections.Generic;
using Exiled.API.Features.Roles;
using MEC;

namespace RGM.Modes.Abilities.Unique.Scp079;

[Ability("과전류", "1분 간 전력이 무제한이 됩니다.", AbilityCategory.Scp079, AbilityType.SCP079_OVERCURRENT)]
public class OverCurrent : Ability
{
    CoroutineHandle _onStarted;

    public override void OnEnabled()
    {
        _onStarted = Timing.RunCoroutine(OnStarted());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_onStarted);
    }

    public IEnumerator<float> OnStarted()
    {
        for (int i = 1; i < 61; i++)
        {
            if (Owner.Role is Scp079Role scp0791)
                scp0791.Energy = scp0791.MaxEnergy;

            yield return Timing.WaitForSeconds(1f);
        }
    }
}
