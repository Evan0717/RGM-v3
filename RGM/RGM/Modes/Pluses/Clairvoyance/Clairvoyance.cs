using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using Mirror;
using RGM.API.Features;
using RGM.API.DataBases;
using UnityEngine;
using Exiled.API.Features.Items;
using Exiled.API.Enums;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.Clairvoyance)]
    class Clairvoyance : Mode
    {
        public override string Name => "야간 작전";
        public override string Description => "모두가 SCP-1344 효과를 받습니다.";
        public override string Detail =>
"""
SCP-1344 효과 - 투시
""";
        public override string Color => "F4FA58";

        public static Clairvoyance Instance;

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

        public void OnSpawned(SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            if (player.IsAlive)
            {
                player.DisableEffect(EffectType.Scp1344);

                Timing.CallDelayed(1, () =>
                {
                    player.EnableEffect(EffectType.Scp1344);
                });
            }
        }
    }
}
