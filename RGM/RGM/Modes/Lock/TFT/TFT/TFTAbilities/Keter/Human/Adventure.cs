using CustomPlayerEffects;
using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Player;
using HintServiceMeow.Core.Extension;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using Mirror;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DAONTFT.Core.TFT.Keter.Human;

[TFTAbility("고귀한 모험", "500 데미지를 가하는 데 성공하면 큰 보상을 획득합니다.", TFTAbilityLevel.Keter, TFTAbilityCategory.Human, TFTAbilityPoint.Continuous, TFTAbilityType.Adventure, "🚩")]
public class Adventure : TFTAbility
{
    float damage = 0;
    bool isGiven = false;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
    }

    void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Attacker != null && ev.Attacker == Owner)
        {
            damage += ev.Amount;

            Data.Description = $"500 데미지를 가하는 데 성공하면 큰 보상을 획득합니다. (현재: {damage} / 500)";

            if (damage > 500 && !isGiven)
            {
                isGiven = true;

                List<ItemType> weapons = new()
                {
                    ItemType.Jailbird,
                    ItemType.MicroHID,
                    ItemType.ParticleDisruptor
                };

                List<ItemType> items = new()
                {
                    ItemType.ArmorHeavy,
                    ItemType.Adrenaline,
                    ItemType.Adrenaline,
                    ItemType.GrenadeFlash,
                    ItemType.GrenadeHE,
                    ItemType.KeycardO5,
                    ItemType.GunLogicer,
                    ItemType.GunFRMG0,
                    ItemType.AntiSCP207,
                    ItemType.SCP207,
                    ItemType.SCP1853,
                    ItemType.SCP1344,
                    ItemType.SCP018,
                    ItemType.SCP268
                };

                Pickup.CreateAndSpawn(weapons.GetRandomValue(), Owner.Position);

                for (int i = 0; i < 10; i++)
                {
                    Pickup.CreateAndSpawn(items.GetRandomValue(), Owner.Position);
                }

                foreach (var item in Item.List.Where(x => x.IsAmmo))
                {
                    Owner.AddItem(item.Type, 3);
                }
            }
        }
    }
}
