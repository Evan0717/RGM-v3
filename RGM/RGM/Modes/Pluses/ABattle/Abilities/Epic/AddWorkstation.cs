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

//[Ability("업무 증가", "새로운 워크스테이션을 발 아래에 생성합니다. 모두가 사용할 수 있습니다. 길을 막으면 제재 대상입니다.", AbilityCategory.Epic, AbilityType.EPIC_ADDWORKSTATION)]
public class AddWorkstation : Ability
{
    public override void OnEnabled()
    {
        if (Physics.Raycast(Owner.Position, Vector3.down, out RaycastHit hit, 100, (LayerMask)1))
        {
            ObjectSpawner.SpawnWorkstation(new WorkstationSerializable
            {
                IsInteractable = true,
                Position = hit.point,
                Rotation = new Vector3(90, Owner.Rotation.y, 0),
                Scale = new Vector3(1, 1, 1),
                RoomType = RoomType.Surface
            });
        }
    }

    public override void OnDisabled()
    {
    }
}
