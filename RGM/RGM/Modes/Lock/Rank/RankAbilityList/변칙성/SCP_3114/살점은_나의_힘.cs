using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using Exiled.Events.EventArgs.Scp173;
using Exiled.Events.Handlers;
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
    [RankAbility("살점은 나의 힘", "인간을 처치할 때마다 체력이 50 회복됩니다.", RankAbilityType.살점은_나의_힘, RankCategory.SCP_3114, RankAbilityCategory.변칙성, "🌊")]
    public class 살점은_나의_힘 : RankAbility
    {
        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Died += OnDied;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Died -= OnDied;
        }

        void OnDied(DiedEventArgs ev)
        {
            if (ev.Attacker == Owner && ev.Player.IsHuman)
            {
                ev.Attacker.Heal(50);
            }
        }
    }
}
