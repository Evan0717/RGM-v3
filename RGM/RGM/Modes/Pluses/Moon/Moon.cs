using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using RGM.API.Features;
using InventorySystem.Items.Usables.Scp330;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Enums;

namespace RGM.Modes
{
    [Mode(ModeCategory.OnlySub, ModeInfo.Plus, ModeType.Moon)]
    public class Moon : Mode
    {
        public override string Name => "달";
        public override string Description => "점프력이 증가합니다.";
        public override string Detail =>
"""
점프력이 증가하며, 낙사 데미지를 받지 않습니다.
""";
        public override string Color => "d1d6c6";

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            foreach (var player in Player.List)
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
                player.EnableEffect(EffectType.Lightweight, 255);
            }
        }

        void OnHurting(HurtingEventArgs ev)
        {
            if (ev.DamageHandler.Type == DamageType.Falldown)
            {
                ev.IsAllowed = false;
            }
        }
    }
};
