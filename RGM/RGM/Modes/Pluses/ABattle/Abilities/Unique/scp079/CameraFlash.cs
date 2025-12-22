using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Sources;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using Exiled.Events.EventArgs.Scp106;
using InventorySystem.Items.Usables.Scp330;
using ProjectMER.Features;
using MEC;
using RGM.API.DataBases;
using RGM.API.Features;
using UnityEngine;
using ProjectMER.Features.Serializable;
using ProjectMER.Features.Objects;
using LabApi.Features.Wrappers;
using ProjectMER.Commands.Modifying.Position;

namespace RGM.Modes.Abilities.Unique.Scp079;

[Ability("카메라 플래시", "핑이 찍힌 장소에 점화된 섬광탄이 생성됩니다. (쿨타임 20초)", AbilityCategory.Scp079, AbilityType.SCP079_CAMERAFLASH)]
public class CameraFlash : Ability
{
    bool isScp079Cooldown = false;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp079.Pinging += OnPinging;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp079.Pinging -= OnPinging;
    }

    public void OnPinging(PingingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        if (!isScp079Cooldown)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                var g = (FlashGrenade)Exiled.API.Features.Items.Item.Create(ItemType.GrenadeFlash, ev.Player);
                g.FuseTime = 4f;
                g.SpawnActive(ev.Position, ev.Player);

                LightSourceToy light = LightSourceToy.Create(ev.Position);
                light.Position = ev.Position;
                light.Range = 5;
                light.Color = new Color(1, 1, 0, 1);
                light.Rotation = Quaternion.Euler(0, 0, 0);


                Timing.CallDelayed(5, () =>
                {
                    light.Destroy();
                });

                isScp079Cooldown = true;

                Timing.CallDelayed(20, () =>
                {
                    isScp079Cooldown = false;
                });
            });
        }
    }
}
