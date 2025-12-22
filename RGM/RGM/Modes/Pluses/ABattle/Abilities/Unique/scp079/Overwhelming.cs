using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Sources;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using Exiled.Events.EventArgs.Scp106;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.DataBases;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.Scp079;

[Ability("고대의 존재 압도", "[Q] 또는 [V]ㅣ키를 홀드하는 동안 전력을 사용하여 해당 방에 있는 인간의 속도를 감소시킵니다.", AbilityCategory.Scp079, AbilityType.SCP079_OVERWHELMING)]
public class Overwhelming : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.VoiceChatting += OnVoiceChatting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.VoiceChatting -= OnVoiceChatting;
    }

    public void OnVoiceChatting(VoiceChattingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        foreach (var player in PlayerManager.List.Where(x => !x.IsNPC && !x.IsScpRole() && x.IsAlive))
        {
            if (player.CurrentRoom == ev.Player.CurrentRoom)
                player.EnableEffect(EffectType.Slowness, (byte)(6 * ev.Player.AbilityCount(AbilityType.SCP079_OVERWHELMING)), 0.1f);
        }

        if (ev.Player.Role is Scp079Role scp079)
            scp079.Energy -= 0.05f;
    }
}
