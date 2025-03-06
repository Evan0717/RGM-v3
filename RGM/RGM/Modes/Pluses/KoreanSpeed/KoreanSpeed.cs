using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Enums;
using MEC;
using Exiled.API.Features.Roles;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.KoreanSpeed)]
    public class KoreanSpeed : Mode
    {
        public override string Name => "한국인이 좋아하는 속도";
        public override string Description => "이런 거 좋아하시죠?";
        public override string Detail =>
"""
<b><i><color=#FB00FF>슈</color><color=#D200D5>우</color><color=#A901AB>우</color><color=#800282>우</color><color=#570358>웅</color><color=#2E042E>화</color></i></b>
""";
        public override string Color => "5882FA";

        public static KoreanSpeed Instance;

        public override void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            while (true)
            {
                foreach (var player in Player.List)
                {
                    player.EnableEffect(EffectType.MovementBoost, 125);
                    player.EnableEffect(EffectType.Scp1853, 5);
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
