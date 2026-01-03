using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using RGM.API.DataBases;
using RGM.API.Features;
using static RGM.Variables.Variable;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.RocketLauncher)]
    class RocketLauncher : Mode
    {
        public override string Name => "로켓 런처";
        public override string Description => "무슨 이유로든 피격당하면 일정 확률로 승천합니다.";
        public override string Detail =>
"""
공격자가 <b>인간</b>인 경우 - 5%
공격자가 <b>SCP</b>인 경우 - 50%
공격자가 <b>???</b>인 경우 - 2000%!!!!!
""";
        public override string Color => "FA8258";

        public static RocketLauncher Instance;

        List<Player> queue = new();

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Hurt += OnHurt;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Hurt -= OnHurt;
        }

        public void OnHurt(Exiled.Events.EventArgs.Player.HurtEventArgs ev)
        {
            if (ev.Attacker != null && HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub) && ev.Player != ev.Attacker)
            {
                if (!queue.Contains(ev.Player))
                {
                    queue.Add(ev.Player);

                    int GetPercent()
                    {
                        if (ev.Attacker.IsScpRole())
                            return UnityEngine.Random.Range(1, 3);

                        else if (ev.Attacker.Role.Type == RoleTypeId.Tutorial)
                            return 1;

                        else
                            return UnityEngine.Random.Range(1, 21);
                    }

                    if (GetPercent() == 1)
                    {
                        Exiled.API.Features.Cassie.MessageTranslated("", $"{ev.Player.DisplayNickname}(<color={ev.Player.Role.Color.ToHex()}>{( Trans.Role[ev.Player.Role.Type])}</color>)(이)가 하늘로 승천했습니다.");
                        Timing.RunCoroutine(Tools.DoRocket(ev.Attacker, ev.Player, 1f));
                    }

                    Timing.CallDelayed(2, () =>
                    {
                        queue.Remove(ev.Player);
                    });
                }
            }
        }
    }
}
