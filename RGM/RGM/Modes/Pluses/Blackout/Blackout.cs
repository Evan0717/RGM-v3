using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using UnityEngine;
using MEC;
using RGM.API.Features;

namespace RGM.Modes
{
    [Mode(ModeCategory.OnlySub, ModeInfo.Plus, ModeType.Blackout)]
    public class Blackout : Mode
    {
        public override string Name => "블랙아웃";
        public override string Description => "시설 곳곳이 정전됩니다.";
        public override string Detail =>
"""
각 방마다 <color=#FF00FF>기믹</color>이 적용될 수 있습니다.

50% - 방이 정전됩니다.
50% - 방의 색상이 변경됩니다.
(소숫점 버림)
""";
        public override string Color => "2A0A0A";

        public static Blackout Instance;

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;

            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            foreach (var room in Room.List)
            {
                switch (UnityEngine.Random.Range(1, 3))
                {
                    case 1:
                        room.TurnOffLights();
                        break;
                    case 2:
                        room.Color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                        break;
                }
            }

            foreach (var player in PlayerManager.List)
            {
                Spawned(player);
            }

            yield return 0f;
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            if (UnityEngine.Random.Range(1, 11) == 1)
                player.AddItem(ItemType.Lantern);

            else
                player.AddItem(ItemType.Flashlight);
        }
    }
}