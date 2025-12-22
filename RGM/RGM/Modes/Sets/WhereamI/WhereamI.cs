using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using MEC;
using UnityEngine;
using Exiled.API.Enums;
using RGM.API.Features;
using Exiled.Events.EventArgs.Server;

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
""";
        public override string Color => "B40486";

        public static WhereamI Instance;

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            Exiled.Events.Handlers.Player.Spawned += OnSpawned;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;

            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            foreach (var player in PlayerManager.List)
            {
                Spawned(player);
            }

            yield return Timing.WaitForSeconds(1f);
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            Door SelectedDoor = null;

            if (Exiled.API.Features.Map.IsLczDecontaminated)
                SelectedDoor = Tools.GetRandomValue(Door.List.Where(x => !x.IsElevator && x.Zone != ZoneType.LightContainment && !x.Type.ToString().Contains("Scp079")).ToList());

            else
                SelectedDoor = Tools.GetRandomValue(Door.List.Where(x => !x.IsElevator && !x.Type.ToString().Contains("Scp079")).ToList());

            player.Position = new Vector3(SelectedDoor.Position.x, SelectedDoor.Position.y + 2, SelectedDoor.Position.z);
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
