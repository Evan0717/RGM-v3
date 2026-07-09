using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Mythic;

[Ability("워 머신", "발사할 때마다 고폭 수류탄을 투하하는, 탄약이 무제한인 리볼버를 얻습니다.\n능력 획득 시 자신이 받는 폭발 데미지가 97% 감소합니다.", AbilityCategory.Mythic, AbilityType.MYTHIC_BOMBGUN)]
public class BombGun : Ability
{
    const float OwnerExplosionDamageMultiplier = 0.03f;
    const float WarMachineGrenadeDamageMultiplier = 0.6f;

    ushort itemSerial = 0;
    readonly List<ExplosionGrenadeProjectile> bombGunGrenades = new();

    public override void OnEnabled()
    {
        Item item = Owner.AddItem(ItemType.GunRevolver);

        itemSerial = item.Serial;

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.Shooting += OnShooting;
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        bombGunGrenades.Clear();
    }

    public void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Player != Owner || ev.Item == null || ev.Player.CurrentItem == null) return;
        if (itemSerial != ev.Player.CurrentItem.Serial) return;
        if (itemSerial == ev.Item.Serial)
            ev.Player.AddHint("워 머신", $"<b><color={ABattle.RatingColor["신화"]}>워 머신</color></b> 능력이 있는 <b>리볼버</b>입니다!");
    }

    public void OnShooting(ShootingEventArgs ev)
    {
        if (ev.Player == Owner && ev.Item.Serial == itemSerial)
        {
            Throwable throwable = ev.Player.ThrowGrenade(ProjectileType.FragGrenade);

            Timing.RunCoroutine(ExplodeOnImpact(throwable));

            Timing.CallDelayed(1, () =>
            {
                ev.Item.As<Firearm>().MagazineAmmo = 6;
            });
        }
    }
    public void OnHurting(HurtingEventArgs ev)
    {
        if (ev.DamageHandler.Type != DamageType.Explosion) return;

        if (ABattle.Instance.HasAbility(ev.Player, AbilityType.MYTHIC_BOMBGUN))
            ev.DamageHandler.Damage *= OwnerExplosionDamageMultiplier;

        if (ev.Attacker == Owner && bombGunGrenades.Any(grenade => grenade != null && Vector3.Distance(grenade.Position, ev.Player.Position) <= 10f))
            ev.DamageHandler.Damage *= WarMachineGrenadeDamageMultiplier;
    }
    public IEnumerator<float> ExplodeOnImpact(Throwable throwable)
    {
        yield return Timing.WaitForSeconds(0.3f);

        if (throwable.Projectile is not ExplosionGrenadeProjectile grenade)
            yield break;

        bombGunGrenades.Add(grenade);

        while (!grenade.IsAlreadyDetonated)
        {
            if (Physics.OverlapSphere(grenade.Position, 0.3f).Count() > 4)
            {
                grenade.Base.Network_syncTargetTime = 0.1f;
            }

            yield return Timing.WaitForOneFrame;
        }

        Timing.CallDelayed(0.5f, () => bombGunGrenades.Remove(grenade));
    }
}
