using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using Mirror;
using MultiBroadcast;
using MultiBroadcast.API;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes
{
    [Mode(ModeCategory.Private, ModeInfo.Set, ModeType.Cell)]
    class Cell : Mode
    {
        public override string Name => "고문";
        public override string Description => "고문을 최대한 오래 버티는 플레이어가 승리합니다.";
        public override string Detail =>
"""
좁은 방에서 <color=#FE2E2E>SCP-018</color>이 던져집니다!

최대한 잘 피해 보세요!
""";
        public override string Color => "D7DF01";

        public static Cell Instance;

        public List<Player> pl = new List<Player>();

        public override void OnEnabled()
        {
            Round.IsLocked = true;

            Exiled.Events.Handlers.Player.Died += OnDied;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            Server.ExecuteCommand("/mp load cell");
            Player BadLucky = Tools.GetRandomValue(Player.List.ToList());
            Player.List.ToList().CopyTo(pl);

            foreach (var player in Player.List)
            {
                player.Role.Set(PlayerRoles.RoleTypeId.Tutorial);
                player.Position = new Vector3(118.7332f, 1000.379f, -41.59417f);

                if (player == BadLucky)
                    Server.ExecuteCommand($"/drop {player.Id} 31 1");
            }

            yield break;
        }

        public void OnDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            if (pl.Contains(ev.Player))
            {
                pl.Remove(ev.Player);

                if (pl.Count() < 2)
                {
                    Round.IsLocked = false;

                    Player.List.ToList().ForEach(x => x.AddBroadcast(20, $"승리자 : {pl[0].Nickname}"));
                    Timing.RunCoroutine(Tools.SetWinner(new List<Player>() { pl[0] }, 5));
                }
            }
        }
    }
}
