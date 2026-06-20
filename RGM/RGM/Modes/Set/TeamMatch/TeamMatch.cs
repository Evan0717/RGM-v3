using System.Collections.Generic;
using System.Linq;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;

using PlayerRoles;
using RGM.API.Features;
using static RGM.Variables.Variable;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.TeamMatch)]
    class TeamMatch : Mode
    {
        public override string Name => "팀 매치";
        public override string Description => "팀원과 함께 상대 팀을 무찌르고, 승리하세요.";
        public override string Detail =>
"""
랜덤한 맵에서, 팀 승리를 위해 상대 팀을 무찌르세요.

제한 시간은 2분이며, 2분이 지나면 버스터콜이 발생합니다.

<b>[Map Credit]</b>
@vasileii, @sleeplessbutter
""";
        public override string Color => "CEECF5";

        public static TeamMatch Instance;

        List<ItemType> StartupItems = new List<ItemType>();
        List<Player> TeamA = new List<Player>();
        List<Player> TeamB = new List<Player>();

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            Round.IsLocked = true;
            Respawn.PauseWaves();

            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.DroppingAmmo += OnDroppingAmmo;
            Exiled.Events.Handlers.Player.Shot += OnShot;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Died -= OnDied;
            Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;
            Exiled.Events.Handlers.Player.DroppingAmmo -= OnDroppingAmmo;
            Exiled.Events.Handlers.Player.Shot -= OnShot;

            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            Tools.LoadMap($"{Maps.GetRandomValue()}");

            StartupItems = Items();

            yield return Timing.WaitForSeconds(1f);

            var players = PlayerManager.List.ToList();
            players.ShuffleList();

            int halfCount = players.Count / 2;

            TeamA = players.Take(halfCount).ToList();
            TeamB = players.Skip(halfCount).ToList();

            foreach (var player in TeamA)
            {
                player.Role.Set(RoleTypeId.ClassD);
                player.Position = Tools.GetObjectList("Spot A").GetRandomValue().position;
                foreach (var item in StartupItems)
                    player.AddItem(item);
            }

            foreach (var player in TeamB)
            {
                player.Role.Set(RoleTypeId.Scientist);
                player.ClearInventory();
                player.Position = Tools.GetObjectList("Spot B").GetRandomValue().position;
                foreach (var item in StartupItems)
                    player.AddItem(item);
            }

            yield return Timing.WaitForSeconds(120f);

            Player BusterCall = Tools.GetRandomValue(PlayerManager.List.Where(x => x.IsAlive).ToList());

            foreach (var player in PlayerManager.List)
            {
                player.Position = BusterCall.Position;
                player.AddBroadcast(20, "<b><size=30>[<color=yellow>버스터콜</color>]</size></b>\n<size=20>모두가 한자리에 모입니다.</size>");
            }
        }

        public List<ItemType> Items()
        {
            List<ItemType> Guns = new List<ItemType>() { ItemType.GunA7, ItemType.GunE11SR, ItemType.GunShotgun, ItemType.GunCom45, ItemType.GunFSP9, ItemType.GunRevolver,
                ItemType.GunCOM18, ItemType.GunCrossvec, ItemType.GunLogicer, ItemType.GunFRMG0, ItemType.GunAK };
            List<ItemType> CDItems = new List<ItemType>() { ItemType.Medkit, ItemType.Painkillers, ItemType.Radio, ItemType.GrenadeFlash };
            List<ItemType> Items = new List<ItemType>();

            Items.Add(Tools.GetRandomValue(Guns));

            foreach (var item in CDItems)
            {
                if (UnityEngine.Random.Range(1, 3) == 1)
                    Items.Add(item);
            }

            return Items;
        }

        public void OnDied(DiedEventArgs ev)
        {
            if (PlayerManager.List.Where(x => x.IsAlive && TeamA.Contains(x)).Count() < 1)
            {
                Round.IsLocked = false;

                Timing.RunCoroutine(Tools.SetWinner(TeamB, 1));
            }
            if (PlayerManager.List.Where(x => x.IsAlive && TeamB.Contains(x)).Count() < 1)
            {
                Round.IsLocked = false;

                Timing.RunCoroutine(Tools.SetWinner(TeamA, 1));
            }
        }

        public void OnDroppingItem(DroppingItemEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        public void OnDroppingAmmo(DroppingAmmoEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        public void OnShot(ShotEventArgs ev)
        {
            ev.Player.AddAmmo(ev.Firearm.AmmoType, 1);
        }
    }
}
