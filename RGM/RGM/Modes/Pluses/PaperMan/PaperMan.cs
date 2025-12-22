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
    [Mode(ModeCategory.OnlySub, ModeInfo.Plus, ModeType.PaperMan)]
    class PaperMan : Mode
    {
        public override string Name => "종이 인간";
        public override string Description => "종이처럼 펄럭펄럭";
        public override string Detail =>
"""
xㅣ0.01
yㅣ1
zㅣ1
의 몸을 가졌습니다!
""";
        public override string Color => "F5A9BC";

        public static PaperMan Instance;

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
                player.Scale = new Vector3(0.01f, 1, 1f);
            });
        }
    }
}
