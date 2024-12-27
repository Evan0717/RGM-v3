using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Rare;

[Ability("트리플악셀", "여분의 탄약과 함께 최대 탄약이 1/2인 COM-45를 지급받습니다.", AbilityCategory.Rare, AbilityType.RARE_TRIPLEAXEL)]
public class TripleAxel : Ability
{
    public override void OnEnabled()
    {
        Item COM45 = Owner.AddItem(ItemType.GunCom45);
        COM45.As<Firearm>().MaxMagazineAmmo /= 2;
        COM45.As<Firearm>().MagazineAmmo = COM45.As<Firearm>().MaxMagazineAmmo;

        Owner.AddItem(ItemType.Ammo9x19, 3);
    }

    public override void OnDisabled()
    {
    }
}
