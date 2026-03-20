using System.Collections.Generic;
using System.Linq;
using DAONTFT.Core.Classes;
using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace DAONTFT.Core.TFT.Euclid.Human;

[TFTAbility("평화주의자", "무기를 소지할 수 없게 됩니다. 대신 초당 최대 체력이 1씩 증가합니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Human, TFTAbilityPoint.Continuous, TFTAbilityType.Peace, "💞")]
public class Peace : TFTAbility
{
    CoroutineHandle _peaceLoop;

    public override void OnEnabled()
    {
        foreach (var item in Owner.Items.ToList())
        {
            if (item.IsFirearm || new List<ItemType> 
            { 
                ItemType.MicroHID,
                ItemType.Jailbird
            }.Contains(item.Type))
            {
                Owner.DropItem(item);
            }
        }

        Exiled.Events.Handlers.Player.SearchingPickup += OnSearchingPickup;
        
        _peaceLoop = Timing.RunCoroutine(peaceLoop());
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.SearchingPickup -= OnSearchingPickup;

        Timing.KillCoroutines(_peaceLoop);
    }

    IEnumerator<float> peaceLoop()
    {
        while (true)
        {
            Owner.MaxHealth += 1;
            Owner.Health += 1;

            yield return Timing.WaitForSeconds(1f);
        }
    }

    void OnSearchingPickup(SearchingPickupEventArgs ev)
    {
        if (ev.Player == Owner) 
        {
            if (ev.Pickup.Type.IsWeapon())
            {
                ev.IsAllowed = false;
            }
        }
    }
}
