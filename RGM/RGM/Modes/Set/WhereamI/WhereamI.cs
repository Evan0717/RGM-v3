using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using MEC;
using UnityEngine;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using RGM.API.Features;
using Exiled.Events.EventArgs.Server;
using RGM.Patches;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.WhereamI)]
    public class WhereamI : Mode
    {
        public override string Name => "여긴 어디?";
        public override string Description => "랜덤한 곳에서 스폰됩니다.";
        public override string Detail =>
"""
여긴 어디? []

* 게임 시작 10분 뒤 <color=red>자동핵</color>이 작동됩니다.
""";
        public override string Color => "B40486";

        public static WhereamI Instance;

        private CoroutineHandle _onModeStarted;
        private readonly AutoWarhead _autoWarhead = new(10, 1);

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            Exiled.Events.Handlers.Player.Spawned += OnSpawned;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
            _autoWarhead.RunCoroutine();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;

            Timing.KillCoroutines(_onModeStarted);
            _autoWarhead.KillCoroutine();
        }

        private IEnumerator<float> OnModeStarted()
        {
            foreach (var player in PlayerManager.List)
            {
                Spawned(player);
            }

            yield return Timing.WaitForSeconds(1f);
        }

        private void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        private void Spawned(Player player)
        {
            if (Warhead.IsDetonated) return;

            var selectedDoor = Exiled.API.Features.Map.IsLczDecontaminated
                ? Door.List.Where(x => !x.IsElevator && x.Zone != ZoneType.LightContainment && !x.Type.ToString().Contains("Scp079")).ToList().GetRandomValue()
                : Door.List.Where(x => !x.IsElevator && !x.Type.ToString().Contains("Scp079")).ToList().GetRandomValue();

            player.Position = new Vector3(selectedDoor.Position.x, selectedDoor.Position.y + 2, selectedDoor.Position.z);
        }

        private void OnRoundEnded(RoundEndedEventArgs ev)
        {
            List<Player> players = [.. PlayerManager.List.Where(x => x.IsAlive && !x.IsNPC)];

            switch (players.Count)
            {
                case 1:
                    Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 5));
                    break;
                case > 1:
                    Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 1));
                    break;
            }
        }
    }
}
