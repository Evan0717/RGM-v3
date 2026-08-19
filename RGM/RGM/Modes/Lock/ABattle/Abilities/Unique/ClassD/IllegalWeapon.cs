using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.ClassD;

[Ability("불법개조무기소지죄", "지면에 닿으면 폭발하는 수류탄을 지급받습니다.", AbilityCategory.Normal, AbilityType.NORMAL_CLASSD_ILLEGALWEAPON,  RoleAbility.ClassD)]
public class IllegalWeapon : Ability
{
    private ushort _id;

    public override void OnEnabled()
    {
        Item item = Owner.AddItem(ItemType.GrenadeHE);
        _id = item.Serial;

        Exiled.Events.Handlers.Player.ThrownProjectile += OnThrownProjectile;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.ThrownProjectile -= OnThrownProjectile;
    }

    private IEnumerator<float> OnThrownProjectile(ThrownProjectileEventArgs ev)
    {
        if (ev.Item.Serial != _id)
            yield break;

        yield return Timing.WaitForSeconds(0.3f);

        if (ev.Projectile is not ExplosionGrenadeProjectile grenade ||
            ev.Player.Role.Type == PlayerRoles.RoleTypeId.Scp079) yield break;
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
