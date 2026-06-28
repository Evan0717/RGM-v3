using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Epic;

[Ability("람보", "탄약이 무제한인 로지카를 받습니다.", AbilityCategory.Epic, AbilityType.EPIC_RAMBO)]
public class Rambo : Ability
{
    ushort InfinityGunSerial = 0;

    public override void OnEnabled()
    {
        Item ig = Owner.AddItem(ItemType.GunLogicer);

        InfinityGunSerial = ig.Serial;

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.Shooting += OnShooting;
    }

    public override void OnDisabled()
    {
    }

    public void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (InfinityGunSerial == ev.Player.CurrentItem.Serial && ev.Item != null)
        {
            if (InfinityGunSerial == ev.Item.Serial)
                ev.Player.AddHint("람보", $"<b><color={ABattle.RatingColor["영웅"]}>람보</color></b> 능력이 있는 Logicer입니다");
        }
    }

    public void OnShooting(ShootingEventArgs ev)
    {
        if (ev.Item.Serial == InfinityGunSerial)
        {
            ev.Player.CurrentItem.As<Firearm>().MagazineAmmo = 250;
            ev.Player.CurrentItem.As<Firearm>().Damage *= 0.86f;
        }
    }
}