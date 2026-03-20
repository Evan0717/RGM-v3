using DAONTFT.Core.Classes;
using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using Mirror;
using RGM.API.Features;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DAONTFT.Core.TFT.Safe.Human;

[TFTAbility("태권도", "능력 키(ALT)를 눌러 발차기를 할 수 있습니다. 부위별로 데미지가 다르게 적용됩니다.", TFTAbilityLevel.Safe, TFTAbilityCategory.Human, TFTAbilityPoint.ALT, TFTAbilityType.Kick, "💫")]
public class Kick : TFTAbility
{
    public static int MeleeCooldown = 0;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.TogglingNoClip -= OnTogglingNoClip;
    }

    public void OnTogglingNoClip(TogglingNoClipEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        if (Function.TryGetLookPlayer(ev.Player, 3f, out Player player, out RaycastHit? hit))
        {
            if (ev.Player != player && MeleeCooldown <= 0 && HitboxIdentity.IsEnemy(ev.Player.ReferenceHub, player.ReferenceHub))
            {
                float damageCalcu(string pos)
                {
                    switch (pos)
                    {
                        case "Head":
                            return 24.1f;

                        case "Chest":
                            return 14f;

                        default:
                            return 12.05f;
                    }
                }

                float damage = damageCalcu(hit.Value.transform.name);

                if (player.IsScp)
                {
                    damage *= 2;
                }

                Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, damage / 14);
                player.Hit(ev.Player, damage);

                MeleeCooldown = 1;

                Timing.CallDelayed(1f, () => MeleeCooldown = 0);
            }
        }
    }
}
