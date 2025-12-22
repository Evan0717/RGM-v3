using Exiled.API.Features.Items;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using UnityEngine;
using Exiled.API.Features.Roles;
using Exiled.API.Enums;
using PlayerRoles;
using MultiBroadcast.API;
using RGM.API.Features;
using Mirror;
using Respawning;

using static RGM.Variables.Variable;
using Exiled.API.Features.Doors;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Extensions;
using Exiled.API.Features.Pickups;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.PickMeUp)]
    public class PickMeUp : Mode
    {
        public override string Name => "픽미업!";
        public override string Description => "누구보다 빠르게 아이템을 사수하세요.";
        public override string Detail =>
    """
    매 라운드마다 유저는 한정된 아이템을 획득해야 살아남을 수 있습니다.

    최후의 1인이 남을 때까지 게임은 계속됩니다.
    """;
        public override string Color => "AEEFC5";
        public override string Map => "PickMeUp";

        public static PickMeUp Instance;

        static List<ItemType> ItemTypes = new List<ItemType>
        {
            ItemType.Painkillers,
            ItemType.Coin,
            ItemType.Lantern,
            ItemType.Radio,
            ItemType.KeycardMTFPrivate,
            ItemType.Adrenaline,
            ItemType.Flashlight,
        };
        static List<Player> PassedPlayers = new List<Player>();

        static Vector3 SpawnBase;
        static Vector3 SpawnX1;
        static Vector3 SpawnX2;
        static Vector3 SpawnY1;
        static Vector3 SpawnY2;
        static Vector3 SpawnZ1;
        static Vector3 SpawnZ2;

        static Vector3 SpawnX()
        {
            return new Vector3(Random.Range(SpawnX1.x, SpawnX2.x), SpawnX1.y, Random.Range(SpawnX1.z, SpawnX2.z));
        }

        static Vector3 SpawnY()
        {
            return new Vector3(Random.Range(SpawnY1.x, SpawnY2.x), SpawnY1.y, Random.Range(SpawnY1.z, SpawnY2.z));
        }

        static Vector3 SpawnZ()
        {
            return new Vector3(Random.Range(SpawnZ1.x, SpawnZ2.x), SpawnZ1.y, Random.Range(SpawnZ1.z, SpawnZ2.z));
        }

        CoroutineHandle _onModeStarted;

        AudioClipPlayback audio;

        public override void OnEnabled()
        {
            Server.FriendlyFire = true;
            Round.IsLocked = true;
            Respawn.PauseWaves();
            Door.Get(DoorType.Scp079Second).IsOpen = true;

            Timing.CallDelayed(1, () =>
            {
                SpawnBase = GameObject.Find("SpawnBasePickMeUp").transform.position;
                SpawnX1 = GameObject.Find("SpawnX1").transform.position;
                SpawnX2 = GameObject.Find("SpawnX2").transform.position;
                SpawnY1 = GameObject.Find("SpawnY1").transform.position;
                SpawnY2 = GameObject.Find("SpawnY2").transform.position;
                SpawnZ1 = GameObject.Find("SpawnZ1").transform.position;
                SpawnZ2 = GameObject.Find("SpawnZ2").transform.position;

                Exiled.Events.Handlers.Player.ItemAdded += OnItemAdded;
                Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;

                _onModeStarted = Timing.RunCoroutine(OnModeStarted());
            });
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.ItemAdded -= OnItemAdded;
            Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;

            Timing.KillCoroutines(_onModeStarted);

            if (audio != null)
                audio.IsPaused = true;
        }

        public IEnumerator<float> OnModeStarted()
        {
            foreach (var player in PlayerManager.List)
            {
                player.Role.Set(RoleTypeId.Tutorial);
            }

            while (!Round.IsEnded)
            {
                Exiled.API.Features.Map.CleanAllItems();
                Exiled.API.Features.Map.CleanAllRagdolls();

                PassedPlayers.Clear();

                foreach (var player in PlayerManager.List)
                {
                    player.Position = SpawnBase;
                    player.ClearInventory();
                }

                ItemType item = ItemTypes.GetRandomValue();

                Vector3 pos()
                {
                    return new List<Vector3> { SpawnX(), SpawnY(), SpawnZ() }.GetRandomValue();
                }

                for (int i = 0; i < PlayerManager.List.Where(x => x.IsAlive).Count() / 2; i++)
                {
                    Pickup.CreateAndSpawn(item, pos());
                }

                audio = GlobalPlayer.TryPlay($"PickMeUp", 2f);

                for (int i = 1; i < 11; i++)
                {
                    foreach (var player in PlayerManager.List)
                    {
                        player.AddBroadcast(1, $"<b><size=30>{11 - i}</size></b>");
                    }

                    yield return Timing.WaitForSeconds(1f);
                }

                Door.Get(DoorType.Scp079Armory).IsOpen = true;

                for (int i = 1; i < 7; i++)
                {
                    foreach (var player in PlayerManager.List)
                    {
                        player.AddBroadcast(1, $"<b><size=30>{7 - i}</size></b>");
                    }

                    yield return Timing.WaitForSeconds(1f);
                }

                Door.Get(DoorType.Scp079Armory).IsOpen = false;

                yield return Timing.WaitForSeconds(1);

                foreach (var player in PlayerManager.List.Where(x => x.IsAlive && !PassedPlayers.Contains(x)))
                {
                    var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, Server.Host);
                    g.FuseTime = 0.1f;
                    g.MaxRadius = 0;
                    g.SpawnActive(player.Position, Server.Host);

                    player.Kill($"굼떠서 사망했습니다.");
                }

                if (PlayerManager.List.Where(x => x.IsAlive).Count() <= 1)
                {
                    Round.IsLocked = false;
                    Timing.RunCoroutine(Tools.SetWinner(new List<Player>() { PlayerManager.List.Where(x => x.IsAlive).ToList()[0] }, 5));

                    yield break;
                }
            }
        }

        public void OnItemAdded(ItemAddedEventArgs ev)
        {
            if (!PassedPlayers.Contains(ev.Player))
               PassedPlayers.Add(ev.Player);
        }

        public void OnDroppingItem(DroppingItemEventArgs ev)
        {
            ev.IsAllowed = false;
        }
    }
}
