using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Epic;

[Ability("훌륭한 대화수단", "탄약이 무제한인 A7을 받습니다. 고폭 수류탄 2개를 추가로 얻습니다.", AbilityCategory.Epic, AbilityType.EPIC_INFINITYGUN)]
public class InfinityGun : Ability
{
    private ushort _infinityGunSerial;

    public override void OnEnabled()
    {
        Owner.AddItem(ItemType.GrenadeHE, 2);
        Item ig = Owner.AddItem(ItemType.GunA7);

        _infinityGunSerial = ig.Serial;

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.Shooting += OnShooting;
    }

    public override void OnDisabled()
    {
    }

    public void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Item?.Serial != _infinityGunSerial)
            return;
        
        ev.Player.AddHint("훌륭한 대화수단", $"<b><color={ABattle.RatingColor["영웅"]}>훌륭한 대화수단</color></b> 능력이 있는 A7입니다.");
    }

    public void OnShooting(ShootingEventArgs ev)
    {
        if (ev.Item.Serial == _infinityGunSerial)
            ev.Player.CurrentItem.As<Firearm>().MagazineAmmo = 31;
    }
}