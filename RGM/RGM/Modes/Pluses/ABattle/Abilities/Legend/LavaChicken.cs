using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features;
using MEC;
using RGM.API.Features;
using UnityEngine;
using Mirror;
using PlayerRoles;
using static UnityEngine.GraphicsBuffer;

namespace RGM.Modes.Abilities.Legend;

[Ability("La-La-La Lava Ch-Ch-Ch Chicken", "8m 반경의 적들을 태웁니다.", AbilityCategory.Legend, AbilityType.LEGEND_LAVACHICKEN)]
public class LavaChicken : Ability
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
        SchematicObject lava = ObjectSpawner.SpawnSchematic("LavaChicken", new Vector3(1205, 1205, 1205), null, null, null);

        while (Owner.IsAlive)
        {
            try
            {
                if (Physics.Raycast(Owner.Position, Vector3.down, out RaycastHit hit, 100, (LayerMask)1))
                {
                    lava.Position = hit.point;
                }

                foreach (var player in Player.List.Where(x => HitboxIdentity.IsEnemy(x.ReferenceHub, Owner.ReferenceHub)))
                {
                    if (Vector3.Distance(player.Position, Owner.Position) <= 8)
                    {
                        Hitmarker.SendHitmarkerDirectly(Owner.ReferenceHub, 0.5f);
                        player.Hurt(Owner, player.IsScp ? player.MaxHealth / 120 : player.MaxHealth / 30, DamageType.Unknown, null, "바삭하게 구워졌습니다.");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"LavaChicken 오류: {e}");
            }
            yield return Timing.WaitForSeconds(0.1f);
        }

        NetworkServer.Destroy(lava.gameObject);
        lava.Destroy();
    }
}
