using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using System.Collections.Generic;

namespace RGM.Modes.SubClass
{
    public static class Steve
    {
        public static List<Player> Players = new();

        public static void Create(Player player)
        {
            IEnumerator<float> main()
            {
                Server.ExecuteCommand($"/slw suit {player.Id} Steve");

                yield break;
            }

            var main_c = Timing.RunCoroutine(main());

            void OnChangingRole(ChangingRoleEventArgs ev)
            {
                if (ev.Player == player)
                {
                    if (Players.Contains(player))
                        Players.Remove(player);

                    Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                    {
                        Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
                    });
                }
            }


            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
        }
    }
}
