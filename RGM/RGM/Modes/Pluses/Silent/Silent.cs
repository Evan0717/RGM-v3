using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using RGM.API;
using UnityEngine;
using Exiled.API.Enums;
using Exiled.API.Features.Roles;
using PlayerRoles;
using Exiled.Events.EventArgs.Player;
using RGM.API.Features;
using static RGM.Variables.ServerManagers;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.Silent)]
    public class Silent : Mode
    {
        public override string Name => "사일런트";
        public override string Description => "쉿!";
        public override string Detail =>
"""
마이크(Q)를 사용할 수 없습니다.
채팅(.ㅊ)도 사용할 수 없습니다.
""";
        public override string Color => "9E82F5";

        public static Silent Instance;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.VoiceChatting += OnVoiceChatting;
        }

        public void OnVoiceChatting(VoiceChattingEventArgs ev)
        {
            if (ev.Player.IsAlive)
            {
                var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, ev.Player);
                g.FuseTime = 0f;
                g.MaxRadius = 0;
                g.SpawnActive(ev.Player.Position, null);

                if (GodModePlayers.Contains(ev.Player))
                    GodModePlayers.Remove(ev.Player);

                ev.Player.Kill("입이 근질거리는 것을 참지 못했습니다.");
            }
        }
    }
}
