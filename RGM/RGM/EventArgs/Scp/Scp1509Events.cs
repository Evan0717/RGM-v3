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
            if (!ev.Player.IsScpRole()) return;
            ev.IsAllowed = false;
                
            Timing.CallDelayed(Timing.WaitForOneFrame, () =>
            {
                if (ev.Player.IsScp)
                    ev.Victim.Role.Set(RoleTypeId.Scp0492, RoleSpawnFlags.None);

                else
                    ev.Victim.Role.Set(ev.Player.Role.Type, RoleSpawnFlags.None);
            });
        }
    }
}
