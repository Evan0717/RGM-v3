using AdminToys;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using Mirror;
using PlayerRoles;
using System.Collections.Generic;
using UnityEngine;

namespace RGM.Modes.SubClass
{
    public class CapybaraPet
    {
        public static List<Player> Players = new();

        public static void Create(Player player)
        {
            List<Vector3> pos = new();

            CapybaraToy capybara = PrefabHelper.Spawn(PrefabType.CapybaraToy, player.Position).GetComponent<CapybaraToy>();

            capybara.NetworkCollisionsEnabled = false;
            capybara.NetworkMovementSmoothing = 100;
            capybara.NetworkScale = new Vector3(0.5f, 0.5f, 0.5f);

            void update(Vector3 pos, Quaternion rot)
            {
                capybara.NetworkPosition = pos;
                capybara.NetworkRotation = rot;
                capybara.UpdatePositionClient();
                capybara.UpdatePositionServer();
            }

            void OnDisabled()
            {
                NetworkServer.Destroy(capybara.gameObject);

                Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
            }

            IEnumerator<float> main()
            {
                Players.Add(player);

                while (true)
                {
                    if (!Players.Contains(player))
                    {
                        OnDisabled();
                        break;
                    }

                    Vector3 pos = player.Position + new Vector3(0, -1, 0);
                    Quaternion rot = Vector3.Distance(player.Position, capybara.NetworkPosition) < 1.2f
                                    ? Quaternion.Euler(0, player.Rotation.eulerAngles.y, 0)
                                    : Quaternion.LookRotation(new Vector3(player.Position.x - capybara.transform.position.x, 0, player.Position.z - capybara.transform.position.z));

                    Timing.CallDelayed(1f, () =>
                    {
                        update(pos, rot);
                    });

                    yield return Timing.WaitForSeconds(0.1f);
                }
            }

            void OnChangingRole(ChangingRoleEventArgs ev)
            {
                if (ev.Player == player)
                {
                    if (ev.NewRole.IsAlive())
                    {
                        capybara.NetworkMovementSmoothing = 255;

                        update(player.Position, player.Rotation);

                        capybara.NetworkMovementSmoothing = 100;
                    }
                }
            }

            Timing.RunCoroutine(main());

            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
        }
    }
}
