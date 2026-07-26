using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerStatsSystem;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Legend;

//[Ability("솔져: 76", "적군 주변 8m 이내에 발사된 총알은 모두 맞은 판정으로 처리됩니다. 단, 최종 데미지가 30% 감소합니다.", AbilityCategory.Legend, AbilityType.LEGEND_SOLDIER76)]
public class Soldier76 : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Shooting += OnShooting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Shooting -= OnShooting;
    }

    public void OnShooting(ShootingEventArgs ev)
    {
        if (ev.Player != Owner || ev.Firearm == null)
            return;

        Player target = PlayerManager.List
            .Where(x => x.IsAlive && !x.IsNPC && HitboxIdentity.IsEnemy(Owner.ReferenceHub, x.ReferenceHub))
            .Select(x => (Player: x, Distance: Vector3.Distance(Owner.Position, x.Position)))
            .Where(x => x.Distance < 8f)
            .OrderBy(x => x.Distance)
            .Select(x => x.Player)
            .FirstOrDefault();

        if (target == null)
            return;

        ev.IsAllowed = false;

        if (ev.Firearm.MagazineAmmo > 0)
            ev.Firearm.MagazineAmmo--;

        target.Hurt(new ScpDamageHandler(ev.Player.ReferenceHub, ev.Firearm.Damage * 0.7f, DeathTranslations.BulletWounds));
        ev.Player.ShowHitMarker();
    }
}
