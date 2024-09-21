using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using UnityEngine;
using Exiled.API.Features.Roles;
using Exiled.API.Enums;
using PlayerRoles;
using MultiBroadcast.API;

namespace RGM.Modes
{
    public class DeathRun
    {
        public static DeathRun Instance;

        Player Tagger = null;

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(1f);

            Server.ExecuteCommand("/mp load DeathRun");

            Tagger = RGM.GetRandomValue(Player.List.ToList());

            Tagger.Role.Set(RoleTypeId.Scientist);
            Tagger.Position = new Vector3(44.57422f, 999.7067f, 78.52734f);
            Tagger.ClearInventory();

            for (int i=1; i<6; i++)
            {
                foreach (var player in Player.List)
                {
                    player.ClearPlayerBroadcasts();
                    player.AddBroadcast(2, $"<size=30><b><color=red>{6 - i}</color>초 후 게임이 시작됩니다. 준비하세요!</size>");
                }
                yield return Timing.WaitForSeconds(1f);
            }

            foreach (var player in Player.List.Where(x => x != Tagger))
            {
                player.Role.Set(RoleTypeId.ClassD);
                player.Position = new Vector3(48.86719f, 999.6483f, 86.34375f);
            }
        }
    }
}
