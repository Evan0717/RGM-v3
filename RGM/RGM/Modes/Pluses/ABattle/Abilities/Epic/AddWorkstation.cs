using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Serializable;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Epic;

[Ability("업무 증가", "새로운 워크스테이션을 발 아래에 생성합니다. 모두가 사용할 수 있습니다.", AbilityCategory.Epic, AbilityType.EPIC_ADDWORKSTATION)]
public class AddWorkstation : Ability
{
    public override void OnEnabled()
    {
        ObjectSpawner.SpawnWorkstation(new WorkstationSerializable
        {
            IsInteractable = true,
            Position = new Vector3(Owner.Position.x, Owner.Position.y - 1.75f, Owner.Position.z),
            Rotation = new Vector3(Owner.Rotation.x, Owner.Rotation.y, Owner.Rotation.z),
            Scale = new Vector3(1, 1, 1),
            RoomType = RoomType.Surface
        });
    }

    public override void OnDisabled()
    {
    }
}
