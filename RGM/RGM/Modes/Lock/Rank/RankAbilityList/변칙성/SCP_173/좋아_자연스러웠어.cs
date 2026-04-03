using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using Exiled.Events.EventArgs.Scp173;
using Exiled.Events.Patches.Events.Scp049;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using RGM.Modes;
using SLPlayerRotation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("좋아, 자연스러웠어!", "순간이동 시, 0.7초 동안 은신 상태가 됩니다.", RankAbilityType.좋아_자연스러웠어, RankCategory.SCP_173, RankAbilityCategory.변칙성, "🎬")]
    public class 좋아_자연스러웠어 : RankAbility
    {
        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Scp173.Blinking += OnBlinking;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Scp173.Blinking -= OnBlinking;
        }

        void OnBlinking(BlinkingEventArgs ev)
        {
            if (ev.Player == Owner)
                ev.Player.AddEffect(EffectType.Invisible, 1, 0.7f);
        }
    }
}
