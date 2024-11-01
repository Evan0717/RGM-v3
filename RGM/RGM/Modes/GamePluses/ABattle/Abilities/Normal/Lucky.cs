using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace RGM.Modes.Abilities.Normal;

[Ability("행운", "5% 확률로 잠긴 문을 열 수 있습니다.", AbilityCategory.Common, AbilityType.NORMAL_LUCKY)]
public class Lucky : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
    }

    public void OnInteractingDoor(Exiled.Events.EventArgs.Player.InteractingDoorEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        if (ev.Door.Type == DoorType.Scp079First)
        {
            ev.Player.ShowHint("이 헤비도어는 능력으로 개폐가 불가능합니다.", 1.2f);
            return;
        }

        if (Warhead.IsInProgress)
        {
            ev.Player.ShowHint("알파 핵탄투가 작동 중일때는 문을 개폐할 수 없습니다.", 1.2f);
            return;
        }

        if (Random.Range(1, 21) == 1)
        {
            ev.IsAllowed = false;

            if (ev.Door.IsOpen)
                ev.Door.IsOpen = false;

            else
                ev.Door.IsOpen = true;
        }
    }
}