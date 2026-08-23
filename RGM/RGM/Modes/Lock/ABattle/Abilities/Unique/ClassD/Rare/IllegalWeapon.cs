using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.ClassD.Rare;

[Ability("불법개조무기소지죄", """
                      충격 수류탄을 지급받습니다.
                      또한, 자신이 투척한 수류탄은 모두 충격 수류탄이 됩니다.
                      """,
    AbilityCategory.Rare, AbilityType.RARE_CLASSD_ILLEGALWEAPON,  RoleAbility.ClassD)]
public class IllegalWeapon : Ability
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

    private IEnumerator<float> OnThrownProjectile(ThrownProjectileEventArgs ev)
    {
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
