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
    class RandomEffect
    {
        public static RandomEffect Instance;

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            while (true)
            {
                List<EffectType> Effects = Tools.EnumToList<EffectType>();

                foreach (var player in Player.List)
                {
                    EffectType Effect = Tools.GetRandomValue(Effects);
                    byte Intensity = (byte)UnityEngine.Random.Range(1, UnityEngine.Random.Range(12, UnityEngine.Random.Range(24, 61)));
                    float Duration = UnityEngine.Random.Range(1, UnityEngine.Random.Range(12, UnityEngine.Random.Range(24, 61)));

                    player.EnableEffect(Effect, Intensity, Duration);
                    player.ShowHint($"<color=#D0FA58>{Effect}</color> 효과가 {Intensity}만큼 {Duration}초 동안 적용되는 중입니다..", Duration);
                }

                yield return Timing.WaitForSeconds(60f);
            }
        }
    }
}
