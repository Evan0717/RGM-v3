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
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.RandomEffect)]
    class RandomEffect : Mode
    {
        public override string Name => "랜덤효과";
        public override string Description => "영구적인 랜덤한 효과를 얻습니다.";
        public override string Detail =>
"""
효과의 종류와 세기는 랜덤입니다.
""";
        public override string Color => "BFFF00";
        public override string Suggester => "idea by 몬키키(@monkiki)";

        public static RandomEffect Instance;

        List<EffectType> ignoredEffect = new List<EffectType>
        {
            EffectType.PocketCorroding,
            EffectType.PitDeath,
            EffectType.CardiacArrest,
            EffectType.Poisoned,
            EffectType.SpawnProtected,
            EffectType.Ensnared,
            EffectType.Flashed,
            EffectType.SeveredHands
        };

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
            if (ev.Player.IsAlive)
                Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            List<EffectType> effects = Tools.EnumToList<EffectType>().Where(x => !ignoredEffect.Contains(x)).ToList();

            EffectType Effect = Tools.GetRandomValue(effects);
            byte Intensity = (byte)UnityEngine.Random.Range(1, UnityEngine.Random.Range(12, UnityEngine.Random.Range(48, UnityEngine.Random.Range(64, UnityEngine.Random.Range(100, 255)))));

            player.EnableEffect(Effect, Intensity);
            player.AddHint("랜덤효과 안내", $"<color=#D0FA58>{Effect}</color> 효과가 {Intensity}만큼 적용되는 중입니다.", 99999);
        }
    }
}
