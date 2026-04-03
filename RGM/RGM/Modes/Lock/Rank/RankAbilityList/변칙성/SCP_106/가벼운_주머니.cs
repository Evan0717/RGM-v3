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
    [RankAbility("가벼운 주머니", "한방에 차원 주머니로 보내는 대신, 차원 주머니 탈출 확률이 50%로 조정됩니다.", RankAbilityType.가벼운_주머니, RankCategory.SCP_106, RankAbilityCategory.변칙성, "🎒")]
    public class 가벼운_주머니 : RankAbility
    {
        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.FailingEscapePocketDimension += OnFailingEscapePocketDimension;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Player.FailingEscapePocketDimension -= OnFailingEscapePocketDimension;
        }

        void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == Owner)
                ev.Player.AddEffect(EffectType.PocketCorroding, 1);
        }

        void OnFailingEscapePocketDimension(FailingEscapePocketDimensionEventArgs ev)
        {
            if (Random.Range(1, 3) == 1)
            {
                ev.IsAllowed = false;

                ev.Player.RemoveEffect(EffectType.PocketCorroding, 1);
                ev.Player.Teleport(Room.List.GetRandomValue());
            }
        }
    }
}
