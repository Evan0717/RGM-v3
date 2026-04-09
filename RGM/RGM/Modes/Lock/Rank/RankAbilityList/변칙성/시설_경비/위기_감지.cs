using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using RGM.Modes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RGM.API.Features;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("위기 감지", "주변에 반란이 있다면 시야가 잠시 흐려집니다.", RankAbilityType.위기_감지, RankCategory.시설_경비, RankAbilityCategory.변칙성, "🆘")]
    public class 위기_감지 : RankAbility
    {
        int cooldown = 30;
        CoroutineHandle handle;

        public override void OnEnabled()
        {
            handle = Timing.RunCoroutine(loop());
        }

        public override void OnDisabled()
        {
            Timing.KillCoroutines(handle);
        }

        bool isDetected()
        {
            foreach (var player in Player.List.Where(x => x.IsCHI))
            {
                if (Vector3.Distance(Owner.Position, player.Position) < 15)
                    return true;
            }

            return false;
        }

        IEnumerator<float> loop()
        {
            while (true)
            {
                while (!isDetected())
                    yield return Timing.WaitForSeconds(1);

                Owner.AddEffect(EffectType.Blinded, 1, 0.5f);

                yield return Timing.WaitForSeconds(cooldown);
            }
        }
    }
}
