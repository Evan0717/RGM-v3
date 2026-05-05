using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using System.Collections.Generic;
using UnityEngine;

namespace RGM.Modes.SubClass
{
    public static class Foxy
    {
        public static List<Player> Players = new();

        public static void Create(Player player)
        {
            IEnumerator<float> main()
            {
                Server.ExecuteCommand($"/slw suit {player.Id} Foxy");

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
