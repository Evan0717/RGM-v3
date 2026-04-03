using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using Exiled.Events.EventArgs.Scp173;
using Exiled.Events.EventArgs.Scp3114;
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
    [RankAbility("단백질", "변장 해제 후 이동 속도가 10초간 10% 증가합니다.", RankAbilityType.단백질, RankCategory.SCP_3114, RankAbilityCategory.변칙성, "🍢")]
    public class 단백질 : RankAbility
    {
        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Scp3114.Revealed += OnRevealed;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Scp3114.Revealed -= OnRevealed;
        }

        void OnRevealed(RevealedEventArgs ev)
        {
            if (ev.Player == Owner)
                Owner.AddEffect(EffectType.MovementBoost, 10, 10);
        }
    }
}
