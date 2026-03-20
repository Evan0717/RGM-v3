using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DAONTFT.Core.TFT.Keter.Scp079;

[TFTAbility("랜덤 함수", "정전ㅣ다른 10개의 방을 추가로 정전합니다.", TFTAbilityLevel.Keter, TFTAbilityCategory.Scp079, TFTAbilityPoint.Continuous, TFTAbilityType.RandomFunction, "🔗")]
public class RandomFunction : TFTAbility
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp079.RoomBlackout += OnRoomBlackout;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp079.RoomBlackout -= OnRoomBlackout;
    }

    public void OnRoomBlackout(RoomBlackoutEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        for (int i = 1; i < 11; i++)
        {
            Room SelectedRoom = Room.List.GetRandomValue();

            SelectedRoom.TurnOffLights(10);
        }
    }
}
