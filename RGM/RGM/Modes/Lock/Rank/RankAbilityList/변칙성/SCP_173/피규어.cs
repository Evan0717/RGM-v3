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
    [RankAbility("피규어", "체력이 500 증가하고, 몸 크기가 0.8로 조정됩니다.", RankAbilityType.피규어, RankCategory.SCP_173, RankAbilityCategory.변칙성, "🀄")]
    public class 피규어 : RankAbility
    {
        public override void OnEnabled()
        {
            Owner.MaxHealth += 500;
            Owner.Health = Owner.MaxHealth;
            Owner.Scale = new Vector3(0.8f, 0.8f, 0.8f);
        }

        public override void OnDisabled()
        {
        }
    }
}
