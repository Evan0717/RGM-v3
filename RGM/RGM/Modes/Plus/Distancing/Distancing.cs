using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;
using UnityEngine;
using Exiled.API.Enums;
using PlayerRoles;
using RGM.API.Features;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.Distancing)]
    public class Distancing : Mode
    {
        public override string Name => "사회적 거리두기";
        public override string Description => "최대한 다른 사람과 멀어지세요! 감염 예방이 최우선입니다!";
        public override string Detail =>
"""
다른 사람과 가까이 붙지 마세요.

5m 초과의 거리를 유지하지 못한다면 체력의 일정 비율만큼 데미지를 입습니다.

* 게임 시작 8분 뒤 <color=red>자동핵</color>이 작동됩니다.
""";
        public override string Color => "38610B";

        public static Distancing Instance;

        CoroutineHandle _onModeStarted;
        CoroutineHandle _autoWarhead;

        public override void OnEnabled()
        {
            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
            _autoWarhead = Timing.RunCoroutine(AutoWarhead());
        }

        public override void OnDisabled()
        {
            Timing.KillCoroutines(_onModeStarted);
            Timing.KillCoroutines(_autoWarhead);
        }

        public IEnumerator<float> OnModeStarted()
        {
            while (true)
            {
                List<Player> DamagePlayers = new List<Player>();

                foreach (var p1 in PlayerManager.List)
                {
                    foreach (var p2 in PlayerManager.List)
                    {
                        if (p1 != p2 && Vector3.Distance(p1.Position, p2.Position) < 7.5f)
                        {
                            if (!DamagePlayers.Contains(p1))
                                DamagePlayers.Add(p1);

                            if (!DamagePlayers.Contains(p2))
                                DamagePlayers.Add(p2);
                        }
                    }
                }

                foreach (var player in DamagePlayers.Where(x => x.Role.Type != RoleTypeId.Scp079))
                {
                    player.Hurt(player.IsScpRole() ? player.MaxHealth * 0.008f : player.MaxHealth * 0.024f, "인싸는 죽었습니다.");

                    if (player.IsAlive)
                    {
                        player.EnableEffect(EffectType.Poisoned, 1, 1.5f);

                        if (player.Health <= 0)
                            player.Kill("사회가 당신과 거리를 두었습니다.");
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
        
        public IEnumerator<float> AutoWarhead()
        {
            yield return Timing.WaitForSeconds(7 * 60);

            if (Warhead.IsDetonated)
                yield break;

            Tools.MessageTranslated("", $"1분 뒤 <color=red>자동핵</color>이 작동됩니다.");

            if (Warhead.IsDetonated)
                yield break;

            yield return Timing.WaitForSeconds(1 * 60);

            DeadmanSwitch.StartWarhead();
        }
    }
}