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
    [RankAbility("23분 카레", "체력이 850이 되는 대신 총기 관련 데미지에 99% 저항을 가지며, 흄 쉴드가 삭제됩니다.", RankAbilityType.이십삼분_카레, RankCategory.SCP_106, RankAbilityCategory.변칙성, "🍛")]
    public class 이십삼분_카레 : RankAbility
    {
        public override void OnEnabled()
        {
            Owner.MaxHealth = 850;
            Owner.Health = Owner.MaxHealth;
            Owner.MaxHumeShield = 0;
            Owner.HumeShield = Owner.MaxHumeShield;

            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        }

        void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Player == Owner && ev.DamageHandler.Type.IsWeapon(false))
                ev.DamageHandler.Damage *= 0.01f;
        }
    }
}
