using CustomPlayerEffects;
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
    [RankGadget("유독성 가스", "안개에 있는 적들에게 2초간 부식 효과를 적용하고 20의 데미지를 입힙니다.", RankAbilityType.유독성_가스, RankCategory.SCP_939, "🚫")]
    public class 유독성_가스 : RankGadgetAbility
    {
        protected override void OnGadgetUsed()
        {
            foreach (var human in Player.List.Where(x => x.IsHuman && x.IsEffectActive<AmnesiaVision>()).ToList())
            {
                human.Hit(Owner, 20);
                human.AddEffect(EffectType.Corroding, 1, 2);
            }
        }
    }
}
