using Exiled.API.Features;
using MEC;
using RGM.Modes;
using System.Collections.Generic;
using System.Linq;
using RGM.API.Features;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("연결 확인", "반란이 몇 명 살아있는지 확인합니다.", RankAbilityType.연결_확인, RankCategory.반란, "🚦", 50)]
    public class 연결_확인 : RankGadgetAbility
    {
        protected override void OnGadgetUsed()
        {
            int count = Player.List.Count(x => x.IsCHI);

            IEnumerator<float> enumerator()
            {
                for (int i = 0; i < count; i++)
                {
                    Tools.PlaySound(Owner.Transform, "RankCheckAlive", 2f);

                    yield return Timing.WaitForSeconds(1);
                }    
            }

            Timing.RunCoroutine(enumerator());
        }
    }
}
