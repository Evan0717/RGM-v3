using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using Exiled.Events.EventArgs.Scp173;
using MEC;
using MultiBroadcast.Commands.Subcommands;
using PlayerRoles;
using RGM.Modes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("엇박자", "다음 이동 쿨타임을 즉시 1초로 조정합니다.", RankAbilityType.엇박자, RankCategory.SCP_173, "🎶")]
    public class 엇박자 : RankGadgetAbility
    {
        protected override void OnGadgetUsed()
        {
            if (Owner.Role is Scp173Role scp173)
            {
                float originCooldown = scp173.BlinkCooldown;

                scp173.BlinkCooldown = 1;

                Exiled.Events.Handlers.Scp173.Blinking += OnBlinking;

                void OnBlinking(BlinkingEventArgs ev)
                {
                    if (ev.Player == Owner)
                    {
                        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                        {
                            scp173.BlinkCooldown = originCooldown;
                        });
                    }

                    Exiled.Events.Handlers.Scp173.Blinking -= OnBlinking;
                }
            }
        }
    }
}
