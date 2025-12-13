using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using ProjectMER.Features.Objects;
using ProjectMER.Features;
using MEC;
using Mirror;
using RGM.API.Features;
using UnityEngine;
using Microsoft.Win32.SafeHandles;
using LabApi.Features.Wrappers;

namespace RGM.Modes.Abilities.Epic;

[Ability("전방주시태만", "총알을 막아주는 방패를 몸 뒤에 장착합니다. 누군가 근처에 오면 방패가 비활성화됩니다.", AbilityCategory.Epic, AbilityType.EPIC_FORWARDLOOKING)]
public class ForwardLooking : Ability
{
    CoroutineHandle _onStarted;

    public override void OnEnabled()
    {
        _onStarted = Timing.RunCoroutine(OnStarted());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_onStarted);
    }

    public IEnumerator<float> OnStarted()
    {
        SchematicObject schematic = ObjectSpawner.SpawnSchematic("전방주시태만", new Vector3(1205, 1205, 1205));

        while (Owner.IsAlive)
        {
            try
            {
                schematic.Position = Owner.Position - Owner.Rotation * Vector3.forward * 1.5f;
                schematic.Rotation = Owner.Rotation;

                bool isNear = false;

                foreach (var player in PlayerManager.List.Where(x => x != Owner))
                {
                    if (Vector3.Distance(player.Position, Owner.Position) < 3)
                    {
                        isNear = true;
                    }
                }

                if (isNear)
                {
                    schematic.GetComponentsInChildren<AdminToys.PrimitiveObjectToy>().ToList().ForEach(x => x.NetworkPrimitiveFlags = AdminToys.PrimitiveFlags.Visible);
                    //schematic.GetComponentsInChildren<AdminToys.LightSourceToy>().ToList().ForEach(x => x.NetworkLightColor = new Color(8.3f, 0, 16.1f));
                }
            }
            catch (Exception e)
            {
                Log.Error($"전방주시태만 오류: {e}");
            }

            yield return Timing.WaitForSeconds(0.1f);
        }

        NetworkServer.Destroy(schematic.gameObject);
        schematic.Destroy();
    }
}
