using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Exiled.API.Features;
using HarmonyLib;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using UnityEngine;
using VoiceChat;
using VoiceChat.Playbacks;
using static RGM.Modes.FriendlyFire;
using static RGM.Variables.Variable;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.SuperStar)]
    class SuperStar : Mode
    {
        public override string Name => "슈퍼 스타";
        public override string Description => "모두의 마이크가 공유됩니다.";
        public override string Detail =>
"""
말 그대로입니다. 마이크가 공유됩니다.
""";
        public override string Color => "FE2EF7";

        public static SuperStar Instance;

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            while (true)
            {
                Server.ExecuteCommand($"/speak {string.Join(".", PlayerManager.List.Select(x => x.Id))}. 1");
                foreach (var player in PlayerManager.List)
                {
                    if (!IntercomPlayers.Contains(player))
                        IntercomPlayers.Add(player);
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
