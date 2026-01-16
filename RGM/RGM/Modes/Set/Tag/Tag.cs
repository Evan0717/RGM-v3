using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using Mirror;
using MultiBroadcast;

using PlayerRoles;
using Respawning;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.Tag)]
    class Tag : Mode
    {
        public override string Name => "술래잡기";
        public override string Description => "사냥개를 피해 천천히 도망다니세요. 제한 시간동안 버티세요!";
        public override string Detail =>
"""
고도의 심리전 싸움입니다.

최후의 승자는 과연 누가 될 것인가..

TIP. [ALT] 키를 통해 아군을 밀칠 수 있습니다.
""";
        public override string Color => "F5A9E1";
        public override string Map => "HideAndSeek1205";

        List<Player> finders = new List<Player>();

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            Round.IsLocked = true;
            Respawn.PauseWaves(); 
            Server.FriendlyFire = true;

            Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;

            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.TogglingNoClip -= OnTogglingNoClip;

            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            for (float i = 1; i < PlayerManager.List.Count / 10 + 2; i++)
                finders.Add(Tools.GetRandomValue(PlayerManager.List.Where(x => !finders.Contains(x)).ToList()));

            PlayerManager.List.ToList().ForEach(x => x.IsGodModeEnabled = true);

            foreach (var player in PlayerManager.List.Where(x => !finders.Contains(x)))
            {
                player.Role.Set(RoleTypeId.ClassD);
                player.Position = GameObject.Find("StartPoint").transform.position;
            }

            for (int i = 1; i < 10; i++)
            {
                MultiBroadcast.API.MultiBroadcast.AddMapBroadcast(1, $"<size=25><b><color=red>{10 - i}초 뒤 술래가 출몰합니다.</color></b></size>");

                yield return Timing.WaitForSeconds(1f);
            }

            int remaining = 75;

            foreach (var Finder in finders)
            {
                Finder.Role.Set(RoleTypeId.Scp939);
                Finder.Position = GameObject.Find("StartPoint").transform.position;
            }

            yield return Timing.WaitForSeconds(1f);

            PlayerManager.List.ToList().ForEach(x => x.IsGodModeEnabled = false);
            Round.IsLocked = false;

            for (int i = 1; i < remaining; i++)
            {
                foreach (var player in PlayerManager.List)
                {
                    player.AddBroadcast(1, $"<size=25><b><color=#2EFEF7>{remaining - i}초 뒤 술래가 패배합니다.</color></b></size>");
                }

                yield return Timing.WaitForSeconds(1f);
            }

            if (!Round.IsEnded)
                finders.ForEach(x => x.Kill($"제한 시간 안에 생존자를 전부 죽이지 못했습니다."));
        }

        void OnTogglingNoClip(TogglingNoClipEventArgs ev)
        {
            if (Tools.TryGetLookPlayer(ev.Player, 2, out Player target, out RaycastHit? hit))
            {
                if (!target.IsScpRole())
                {
                    ev.Player.Push(target);
                }
            }
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            IEnumerable<Player> players = PlayerManager.List.Where(x => x.IsAlive && !x.IsNPC);

            if (players.Count() == 1)
                Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 5));

            else if (players.Count() > 1)
                Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 1));
        }
    }
}
