using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;

using RGM.API.DataBases;
using RGM.API.Features;
using UnityEngine;

using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Epic;

[Ability("슈퍼 스타", "자신의 마이크가 모두에게 공유되고, 사망 시 사망한 사실이 모두에게 공개됩니다.", AbilityCategory.Epic, AbilityType.EPIC_SUPERSTAR)]
public class SuperStar : Ability
{
    public override void OnEnabled()
    {
        Server.ExecuteCommand($"/speak {Owner.Id} 1");
        IntercomPlayers.Add(Owner);

        Exiled.Events.Handlers.Player.Died += OnDied;
    }

    public override void OnDisabled()
    {
        Server.ExecuteCommand($"/speak {Owner.Id} 0");
        IntercomPlayers.Remove(Owner);

        Exiled.Events.Handlers.Player.Died -= OnDied;
    }

    public void OnDied(DiedEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        foreach (var player in PlayerManager.List)
        {
            player.AddBroadcast(10, $"<size=25><color=#FE2EF7>슈퍼 스타</color>였던 <color=#F4FA58>{Owner.DisplayNickname}</color>(<color={ev.TargetOldRole.GetColor().ToHex()}>{( Trans.Role[ev.TargetOldRole])}</color>)(은)는 별세하셨습니다..</size>");
        }
    }
}
