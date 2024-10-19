using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using Mirror;
using MultiBroadcast;
using PlayerRoles;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes
{
    class HideAndSeek
    {
        public static HideAndSeek Instance;

        List<Player> Finders = new List<Player>();

        public void OnEnabled()
        {
            Respawn.TimeUntilNextPhase = 10000;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            Server.ExecuteCommand($"/mp load HideAndSeek");

            for (float i = 1; i < Player.List.Count / 10 + 2; i++)
                Finders.Add(Tools.GetRandomValue(Player.List.Where(x => !Finders.Contains(x)).ToList()));

            Player.List.ToList().ForEach(x => Server.ExecuteCommand($"/god {x.Id} 1"));

            foreach (var player in Player.List.Where(x => !Finders.Contains(x)))
            {
                player.Role.Set(RoleTypeId.ClassD);
                player.Position = GameObject.Find("StartPoint").transform.position;
            }

            for (int i = 1; i < 10; i++)
            {
                foreach (var player in Player.List)
                {
                    player.ClearBroadcasts();
                    player.Broadcast(2, $"<size=25><b><color=red>{10 - i}초 뒤 술래가 출몰합니다.</color></b></size>");
                }

                yield return Timing.WaitForSeconds(1f);
            }

            int Remaining = 75;

            foreach (var Finder in Finders)
            {
                Finder.Role.Set(RoleTypeId.Scp939);
                Finder.Position = GameObject.Find("StartPoint").transform.position;
            }

            yield return Timing.WaitForSeconds(1f);

            Player.List.ToList().ForEach(x => Server.ExecuteCommand($"/god {x.Id} 0"));

            for (int i = 1; i < Remaining; i++)
            {
                foreach (var player in Player.List)
                {
                    player.ClearBroadcasts();
                    player.Broadcast(2, $"<size=25><b><color=#2EFEF7>{Remaining - i}초 뒤 술래가 패배합니다.</color></b></size>");
                }

                yield return Timing.WaitForSeconds(1f);
            }

            Finders.ForEach(x => x.Kill($"제한 시간 안에 생존자를 전부 죽이지 못했습니다."));
        }
    }
}
