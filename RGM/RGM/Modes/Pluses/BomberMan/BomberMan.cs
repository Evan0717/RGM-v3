using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using UnityEngine;
using Exiled.API.Features.Roles;
using Exiled.API.Enums;
using RGM.API.Features;
using MultiBroadcast.API;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.BomberMan)]
    public class BomberMan : Mode
    {
        public override string Name => "봄버맨";
        public override string Description => "무기로는 오직 고폭 수류탄만 사용할 수 있습니다. 무한으로 지급됩니다.";
        public override string Detail =>
"""
<b>수류탄 지급 규칙</b>

1초마다 수류탄이 없다면 지급됩니다.
<color=red>SCP-079</color>의 경우에는 2초마다 핑을 통해 폭탄을 투하할 수 있습니다.
""";
        public override string Color => "FF8000";

        public static BomberMan Instance;

        public bool _isScp079Cooldown = false;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;

            Exiled.Events.Handlers.Scp079.Pinging += OnPinging;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (!player.HasItem(ItemType.GrenadeHE))
                        player.AddItem(ItemType.GrenadeHE);
                }

                yield return Timing.WaitForSeconds(5f);
            }
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                foreach (var _item in ev.Player.Items)
                {
                    if (_item.IsWeapon)
                        ev.Player.RemoveItem(_item);
                }
            });
        }

        public void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (ev.Pickup.Category == ItemCategory.Firearm)
                ev.IsAllowed = false;
        }

        public void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (ev.Item.Type == ItemType.GrenadeHE)
                ev.IsAllowed = false;
        }

        public void OnPinging(PingingEventArgs ev)
        {
            if (!_isScp079Cooldown)
            {
                Timing.CallDelayed(0.1f, () =>
                {
                    var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, ev.Player);
                    g.FuseTime = 3f;
                    g.SpawnActive(ev.Position, ev.Player);

                    _isScp079Cooldown = true;

                    Timing.CallDelayed(2, () =>
                    {
                        _isScp079Cooldown = false;
                    });
                });
            }
        }
    }
}
