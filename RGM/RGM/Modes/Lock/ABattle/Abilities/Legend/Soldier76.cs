using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerStatsSystem;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Legend;

/*[Ability("솔져: 76",
    "적군 주변 10m 이내에 발사된 총알은 모두 맞은 판정으로 처리하는 E11SR를 획득합니다.\n" +
                   " 단, 최종 데미지가 50% 감소하며 30초마다 50발의 5.56x45mm 탄이 장전됩니다. (150개를 넘을 시 지급되지 않습니다.)", 
    AbilityCategory.Legend, 
    AbilityType.LEGEND_SOLDIER76)]*/
public class Soldier76 : Ability
{
    private static CoroutineHandle _ammoCoroutine;
    
    private ushort _serial;
    private Item _item;
    
    public  override void OnEnabled()
    {
        _item = Owner.AddItem(ItemType.GunE11SR);
        _serial = _item.Serial;
        
        Exiled.Events.Handlers.Player.Shooting += OnShooting;
        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;

        if (!Timing.IsRunning(_ammoCoroutine))
            _ammoCoroutine = Timing.RunCoroutine(AmmoGiver());
    }

    private IEnumerator<float> AmmoGiver()
    {
        var firearm = _item.As<Firearm>();
        while (true)
        {
            yield return Timing.WaitForSeconds(30f);
            if (firearm.MagazineAmmo >= 150) continue;
            firearm.MagazineAmmo += 50;
        }
    }

    private void OnChangedItem(ChangedItemEventArgs e)
    {
        if (e.Player == null || e.Player.IsDead) return;
        
        if (e.Player.CurrentItem?.Serial == _serial)
            e.Player.AddEffect(EffectType.Scp1344, 1);
        else 
            e.Player.RemoveEffect(EffectType.Scp1344, 1);
    }
    
    

    private void OnShooting(ShootingEventArgs ev)
    {
        if (ev.Item == null || ev.Firearm.Serial != _serial) return;

        ev.Player.TryGetNearestVisiblePlayer(out var player, out _, 10f, 60f, [.. PlayerManager.List.Where(x => x == ev.Player || x.IsDead)]);

        ev.IsAllowed = false;
        if (player == null) return;
        
        if (ev.Firearm.MagazineAmmo > 0)
            ev.Firearm.MagazineAmmo--;

        player.Hurt(new ScpDamageHandler(ev.Player.ReferenceHub, ev.Firearm.Damage * (player.IsScpRole() ? 0.3f : 0.5f), DeathTranslations.BulletWounds));
        ev.Player.ShowHitMarker();
    }
}