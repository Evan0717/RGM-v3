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
    [RankAbility("암살 조장", "발걸음 소리가 줄어들고 흐리게 보입니다.", RankAbilityType.암살_조장, RankCategory.SCP_939, RankAbilityCategory.변칙성, "📛")]
    public class 암살_조장 : RankAbility
    {
        public override void OnEnabled()
        {
            Owner.AddEffect(EffectType.SilentWalk, 9);
            Owner.AddEffect(EffectType.Fade, 125);
        }

        public override void OnDisabled()
        {
        }
    }
}
