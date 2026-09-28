using Exiled.Events.EventArgs.Scp1509;
using MEC;
using PlayerRoles;
using RGM.API.Features;

namespace RGM.EventArgs
{
    public static class Scp1509Events
    {
        public static void OnResurrecting(ResurrectingEventArgs ev)
        {
            // SCP-1509에 의해 부활한 대상은 1509Resurrected 이펙트를 적용받지 않게 함.
            Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                ev.Player.DisableEffect<CustomPlayerEffects.Scp1509Resurrected>());
        }
    }
}
