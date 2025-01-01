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

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.RandomEffect)]
    class RandomEffect : Mode
    {
        public override string Name => "랜덤효과";
        public override string Description => "60초마다 랜덤한 효과를 얻을 수 있습니다!";
        public override string Detail =>
"""
1분마다 랜덤한 효과를 지급받을 수 있습니다.
최대 60만큼, 60초 동안 받습니다.
""";
        public override string Color => "BFFF00";
        public override string Suggester => "몬키키";

        public static RandomEffect Instance;

        public override void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            while (true)
            {
                List<EffectType> Effects = Tools.EnumToList<EffectType>();

                foreach (var player in Player.List.Where(x => x.IsAlive))
                {
                    EffectType Effect = Tools.GetRandomValue(Effects.Where(x => x != EffectType.PocketCorroding).ToList());
                    byte Intensity = (byte)UnityEngine.Random.Range(1, UnityEngine.Random.Range(12, UnityEngine.Random.Range(48, UnityEngine.Random.Range(64, UnityEngine.Random.Range(100, 255)))));
                    float Duration = UnityEngine.Random.Range(1, UnityEngine.Random.Range(12, UnityEngine.Random.Range(24, UnityEngine.Random.Range(48, 61))));

                    player.EnableEffect(Effect, Intensity, Duration);
                    player.ShowHint($"<color=#D0FA58>{Effect}</color> 효과가 {Intensity}만큼 {Duration}초 동안 적용되는 중입니다..", Duration);
                }

                yield return Timing.WaitForSeconds(60f);
            }
        }
    }
}
