using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.Events.EventArgs.Player;
using HintServiceMeow.Core.Extension;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using static DAONTFT.Core.Variables.Base;

namespace DAONTFT.Core.TFT.Euclid.Human;

[TFTAbility("폭탄병", "고폭 수류탄이 지면에 닿으면 즉시 폭발합니다. 고폭 수류탄을 획득합니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Human, TFTAbilityPoint.Continuous, TFTAbilityType.IllegalWeapon, "💢")]
public class IllegalWeapon : TFTAbility
{
    public override void OnEnabled()
    {
        Owner.AddItem(ItemType.GrenadeHE);

        Exiled.Events.Handlers.Player.ThrownProjectile += OnThrownProjectile;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.ThrownProjectile -= OnThrownProjectile;
    }

    public IEnumerator<float> OnThrownProjectile(ThrownProjectileEventArgs ev)
    {
        if (ev.Player == Owner)
        {
            yield return Timing.WaitForSeconds(0.3f);

            if (ev.Projectile is ExplosionGrenadeProjectile grenade && ev.Player.Role.Type != PlayerRoles.RoleTypeId.Scp079)
            {
                while (!grenade.IsAlreadyDetonated)
                {
                    if (Physics.OverlapSphere(grenade.Position, 0.3f).Count() > 4)
                    {
                        grenade.Base.Network_syncTargetTime = 0.1f;
                    }

                    yield return Timing.WaitForOneFrame;
                }
            }
        }
    }
}
