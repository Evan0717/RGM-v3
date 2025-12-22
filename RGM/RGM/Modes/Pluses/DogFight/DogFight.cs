using Exiled.API.Features.Items;
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
using Exiled.Events.EventArgs.Scp079;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Server;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.DogFight)]
    public class DogFight : Mode
    {
        public override string Name => "개판 1초전";
        public override string Description => "그냥 개판입니다. 관리자에게 제약이 적용되지 않습니다.";
        public override string Detail =>
"""
리볼버!!!!!!

소환된 SCP-018은 60초 뒤에 사라집니다. 렉 방지를 위해
팀킬이 허용됩니다.
""";
        public override string Color => "DF0101";

        public static DogFight Instance;

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;

            Exiled.Events.Handlers.Scp079.Pinging += OnPinging;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());

            Tools.TryInstallMode(ModeType.FriendlyFire);
            Tools.TryInstallMode(ModeType.Radio);
            Tools.TryInstallMode(ModeType.Ghost);
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            Exiled.Events.Handlers.Player.Shooting -= OnShooting;
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;

            Exiled.Events.Handlers.Scp079.Pinging -= OnPinging;

            Timing.KillCoroutines(_onModeStarted);

            Tools.UnInstallMode(ModeType.FriendlyFire);
            Tools.UnInstallMode(ModeType.Radio);
            Tools.UnInstallMode(ModeType.Ghost);
        }

        public IEnumerator<float> OnModeStarted()
        {
            foreach (var player in PlayerManager.List)
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

        public void OnPinging(PingingEventArgs ev)
        {
            Pickup.CreateAndSpawn(ItemType.SCP018, ev.Position, new Quaternion(Random.Range(1, 180), Random.Range(1, 180), Random.Range(1, 180), Random.Range(1, 180)), ev.Player);
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            foreach (var player in PlayerManager.List.Where(x => x.IsAlive))
            {
                Server.ExecuteCommand($"/drop {player.Id} 31 25");
            }
        }
    }
}