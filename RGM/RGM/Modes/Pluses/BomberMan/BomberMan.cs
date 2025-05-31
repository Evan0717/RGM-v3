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
using ProjectMER.Features.Serializable;
using ProjectMER.Features.Objects;
using ProjectMER.Features;

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

7초마다 수류탄이 없다면 지급됩니다.
<color=red>SCP-079</color>의 경우에는 핑을 통해 폭탄을 투하할 수 있습니다. (쿨타임 5초)
""";
        public override string Color => "FF8000";
        public override string Suggester => "monkiki";

        public static BomberMan Instance;

        public bool _isScp079Cooldown = false;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.ItemAdded += OnItemAdded;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;

            Exiled.Events.Handlers.Scp079.Pinging += OnPinging;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            foreach (var player in Player.List)
            {
                try
                {
                    foreach (var item in player.Items)
                    {
                        if (item.IsFirearm)
                            player.RemoveItem(item);
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"Error on removing items: {e}");
                }
            }

            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (!player.HasItem(ItemType.GrenadeHE))
                        player.AddItem(ItemType.GrenadeHE);
                }

                yield return Timing.WaitForSeconds(7f);
            }
        }

        public void OnItemAdded(ItemAddedEventArgs ev)
        {
            if (ev.Item is Firearm firearm)
            {
                ev.Player.RemoveItem(ev.Item);

                firearm.Destroy();
            }
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
                    g.FuseTime = 5.5f;
                    g.SpawnActive(ev.Position, ev.Player);

                    LabApi.Features.Wrappers.LightSourceToy light = LabApi.Features.Wrappers.LightSourceToy.Create(ev.Position);
                    light.Position = ev.Position;
                    light.Range = 5;
                    light.Color = new Color(1, 0, 0, 1);
                    light.Rotation = Quaternion.Euler(0, 0, 0);


                    Timing.CallDelayed(5, () =>
                    {
                        light.Destroy();
                    });

                    _isScp079Cooldown = true;

                    Timing.CallDelayed(20, () =>
                    {
                        _isScp079Cooldown = false;
                    });
                });
            }
        }
    }
}
