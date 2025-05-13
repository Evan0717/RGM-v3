using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Normal;

[Ability("치유 사제", "체력을 15만큼 회복시켜주는 COM-18을 받습니다. 9x19mm탄을 2세트 얻습니다.", AbilityCategory.Common, AbilityType.NORMAL_HEALGUN)]
public class HealGun : Ability
{
    ushort HealGunSerial = 0;

    public override void OnEnabled()
    {
        Owner.AddItem(ItemType.Ammo9x19, 2);
        Item hg = Owner.AddItem(ItemType.GunCOM18);

        HealGunSerial = hg.Serial;

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
    }

    public void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (HealGunSerial == ev.Player.CurrentItem.Serial && ev.Item != null)
        {
            if (HealGunSerial == ev.Item.Serial)
                ev.Player.AddHint($"<b><color={ABattle.RatingColor["일반"]}>치유 사제</color></b> 능력이 있는 COM-18입니다.");
        }
    }

    public void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Attacker == null)
            return;

        if (ev.Attacker.CurrentItem != null && ev.Attacker.CurrentItem.Serial == HealGunSerial)
        {
            ev.IsAllowed = false;

            ev.Player.Heal(15);
        }
    }
}