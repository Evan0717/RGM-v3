using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using MEC;
using Mirror;
using RGM.API.Features;
using RGM.API.DataBases;
using UnityEngine;
using Exiled.API.Features.Items;

namespace RGM.Modes
{
    [Mode(ModeCategory.OnlySub, ModeInfo.Plus, ModeType.RandomSize)]
    class RandomSize : Mode
    {
        public override string Name => "랜덤사이즈";
        public override string Description => "스폰 시 랜덤한 크기로 조정됩니다.";
        public override string Detail =>
"""
x: 0.1 ~ 1.2
y: 0.3 ~ 1.2
z: 0.1 ~ 1.2
""";
        public override string Color => "BFFF00";

        public static RandomSize Instance;

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
            foreach (var player in PlayerManager.List)
            {
                Spawned(player);
            }

            yield break;
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            Timing.CallDelayed(1f, () => 
            {
                player.Scale = new Vector3(UnityEngine.Random.Range(0.1f, 1.2f), UnityEngine.Random.Range(0.3f, 1.2f), UnityEngine.Random.Range(0.1f, 1.2f));
            });
        }
    }
}
