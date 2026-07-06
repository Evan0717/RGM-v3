using RGM.API.Features;
using RGM.Modes;
using System.Linq;
using Exiled.Events.EventArgs.Scp096;
using UnityEngine;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("본능", "분노 시에 30m 내의 인간들을 목격자에 포함시킵니다. (최대 4명)", RankAbilityType.본능, RankCategory.SCP_096, RankAbilityCategory.변칙성, "❓")]
    public class 본능 : RankAbility
    {
        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Scp096.Enraging += OnEnraging;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Scp096.Enraging += OnEnraging;
        }

        public void OnEnraging(EnragingEventArgs ev)
        {
            if (ev.Player != Owner)
                return;

            int Stack = 0;

            foreach (var player in PlayerManager.List.Where(x => x.IsHuman))
            {
                if (Stack == 4)
                    break;

                if (Vector3.Distance(player.Position, ev.Player.Position) < 31)
                {
                    Stack += 1;

                    ev.Scp096.AddTarget(player);

                    player.AddHint("본능",
                        $"<color={ABattle.RatingColor["전용"]}>본능</color>에 의해 강제로 목격자에 포함되었습니다. 도망가세요!");
                }
            }

            ev.Player.AddHint("본능", $"<color={ABattle.RatingColor["전용"]}>본능</color> 능력으로 {Stack}명의 인간들을 추가로 탐색했습니다.");
        }
    }
}
