using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using RGM.API.Features;
using InventorySystem.Items.Usables.Scp330;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Enums;

namespace RGM.Modes
{
    [Mode(ModeCategory.Private, ModeInfo.Plus, ModeType.TripleAxel)]
    public class TripleAxel : Mode
    {
        public override string Name => "트리플악셀";
        public override string Description => "획득한 총기는 COM-45로 변환됩니다. 대신 데미지가 70%로 하향됩니다.";
        public override string Detail =>
"""
총기를 습득하는 그 순간부터 COM-45로 변환됩니다.
탄약을 습득하는 그 순간부터 9x19 탄약으로 변경됩니다.
COM-45로 인한 데미지가 70%로 하향됩니다.
""";
        public override string Color => "DF7401";

        public static TripleAxel Instance;

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.ItemAdded += OnItemAdded;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.ItemAdded -= OnItemAdded;
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;

            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            foreach (var player in PlayerManager.List)
            {
                foreach (var item in player.Items)
                {
                    if (item.IsFirearm && item.Type != ItemType.GunCom45)
                    {
                        player.RemoveItem(item);
                        player.AddItem(ItemType.GunCom45);
                    }
                }

                Spawned(player);
            }

            yield break;
        }

        public void OnItemAdded(ItemAddedEventArgs ev)
        {
            if (ev.Item.IsFirearm && ev.Item.Type != ItemType.GunCom45)
            {
                ev.Player.RemoveItem(ev.Item);
                ev.Player.AddItem(ItemType.GunCom45);
            }
        }

        public void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker != null && ev.Attacker.CurrentItem.Type == ItemType.GunCom45)
                ev.DamageHandler.Damage *= 0.7f;
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            if (player.IsAlive)
            {
                ushort totalAmmo = 0;

                foreach (var ammo in player.Ammo.Values)
                    totalAmmo += (ushort)(ammo * 3);

                player.ClearAmmo();
                player.AddAmmo(AmmoType.Nato9, totalAmmo);
            }
        }
    }
};
