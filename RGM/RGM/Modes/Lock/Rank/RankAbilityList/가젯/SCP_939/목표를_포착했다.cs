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
using RGM.API.Features;
using RGM.Modes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("목표를 포착했다", "8초 동안 시야가 개선되고 스테미나가 무제한이 됩니다.", RankAbilityType.목표를_포착했다, RankCategory.SCP_939, "🔭")]
    public class 목표를_포착했다 : RankGadgetAbility
    {
        protected override void OnGadgetUsed()
        {
            Owner.AddEffect(EffectType.Invigorated, 1, 8);
        }
    }
}
