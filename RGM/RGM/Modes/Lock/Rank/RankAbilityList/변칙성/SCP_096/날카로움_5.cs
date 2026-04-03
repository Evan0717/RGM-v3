using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
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
    [RankAbility("날카로움Ⅴ", "공격력이 39 증가합니다.", RankAbilityType.날카로움_5, RankCategory.SCP_096, RankAbilityCategory.변칙성, "🔪")]
    public class 날카로움_5 : RankAbility
    {
        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        }

        void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker != null && ev.Attacker == Owner)
                ev.DamageHandler.Damage += 39;
        }
    }
}
