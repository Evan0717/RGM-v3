using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using RGM.API;
using UnityEngine;
using Exiled.API.Enums;
using PlayerRoles;
using Exiled.Events.EventArgs.Player;
using RGM.API.Features;
using Exiled.API.Extensions;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.DogFight)]
    public class DogFight : Mode
    {
        public override string Name => "개판 1초전";
        public override string Description => "팀킬이 허용됩니다. 리볼버를 얻습니다. ..?";
        public override string Detail =>
"""
리볼버!!!!!!

소환된 SCP-018은 60초 뒤에 사라집니다. 렉 방지를 위해
팀킬이 허용됩니다.
""";
        public override string Color => "DF0101";

        public static DogFight Instance;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            Tools.TryInstallMode(ModeType.FriendlyFire);

            foreach (var player in Player.List)
            {
                Spawned(player);
            }

            yield break;
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            if (ev.Player.IsAlive)
                Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            player.AddItem(ItemType.GunRevolver);
        }

        public void OnShooting(ShootingEventArgs ev)
        {
            if (ev.Item.Type == ItemType.GunRevolver)
            {
                Throwable scp018 = (Throwable)ev.Player.AddItem(ItemType.SCP018);
                Scp018 scp = (Scp018)scp018;
                scp.ChangeItemOwner(ev.Player, null);
                ev.Player.ThrowItem(scp018);
                ev.Player.RemoveItem(scp018);

                Timing.CallDelayed(1, () =>
                {
                    ev.Player.CurrentItem.As<Firearm>().MagazineAmmo = 6;
                });

                Timing.CallDelayed(60, () =>
                {
                    scp018.Destroy();
                });
            }
        }

        public void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker != null && ev.Attacker.CurrentItem != null && ev.Attacker.CurrentItem.Type == ItemType.GunRevolver)
            {
                ev.IsAllowed = false;
            }
        }
    }
}