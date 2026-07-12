using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.ClassD;

[Ability("불법개조무기소지죄", "지면에 닿으면 폭발하는 수류탄을 지급받습니다.", AbilityCategory.ClassD, AbilityType.CLASSD_ILLEGALWEAPON)]
public class IllegalWeapon : Ability
{
    ushort id = 0;

    public override void OnEnabled()
    {
        Item item = Owner.AddItem(ItemType.GrenadeHE);
        id = item.Serial;

        Exiled.Events.Handlers.Player.ThrownProjectile += OnThrownProjectile;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.ThrownProjectile -= OnThrownProjectile;
    }

    public IEnumerator<float> OnThrownProjectile(ThrownProjectileEventArgs ev)
    {
        if (ev.Item.Serial != id)
            yield break;

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
