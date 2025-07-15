using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MEC;
using Mirror;
using MultiBroadcast;
using MultiBroadcast.API;
using PlayerRoles;
using ProjectMER.Events.Arguments;
using RGM.API.Features;
using UnityEngine;
using RGM.Modes.Sets.AddScp.Scps;
using Exiled.API.Extensions;
using Exiled.API.Features.Doors;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.AddScp)]
    class AddScp : Mode
    {
        public override string Name => "추가 SCP";
        public override string Description => "새로운 SCP들이 추가됩니다.";
        public override string Detail =>
"""
새로운 SCP..?

<color=red>SCP-008</color> : 좀비 전염병 (10+, 인간, 1/2)
<color=red>SCP-035</color> : 빙의 가면 (10+, 인간, 1/2)
<color=red>SCP-294</color> : 자판기 (2/3)
<color=red>SCP-457</color> : 불타는 사람 (15+, SCP, 1/2)
<color=red>SCP-966</color> : 잠을 죽이는 자 (15+, SCP, 1/2)
<color=red>SCP-999</color> : 간지럼 괴물 (5+, 인간, 1/2)
<color=red>SCP-1162</color> : 벽 속의 구멍 (2/3)
""";
        public override string Color => "fd0101";

        public static AddScp Instance;

        static List<Player> specialScps = new();

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            if (Random.Range(1, 4) > 1)
                Scp294.OnEnabled();

            if (Random.Range(1, 4) > 1)
                Scp1162.OnEnabled();

            if (Server.PlayerCount >= 5)
            {
                if (Random.Range(1, 3) == 1)
                {
                    Player scp999 = Player.List.GetRandomValue(x => !x.IsScp && x.IsAlive && !specialScps.Contains(x));
                    specialScps.Add(scp999);

                    Door door = Door.Get(DoorType.HIDLab);
                    scp999.Position = door.Position + new Vector3(0, 2, 0);

                    Scp999.Create(scp999);
                }
            }

            if (Server.PlayerCount >= 10)
            {
                if (Random.Range(1, 3) == 1)
                {
                    Player scp008 = Player.List.GetRandomValue(x => !x.IsScp && x.IsAlive && !specialScps.Contains(x));
                    specialScps.Add(scp008);

                    Scp008.Create(scp008);
                }

                if (Random.Range(1, 3) == 1)
                {
                    Player scp035 = Player.List.GetRandomValue(x => !x.IsScp && x.IsAlive && !specialScps.Contains(x));
                    specialScps.Add(scp035);

                    Door door = Door.Get(DoorType.ElevatorNuke);
                    scp035.Position = door.Position + new Vector3(0, 2, 0);

                    Scp035.Create(scp035);
                }
            }

            if (Server.PlayerCount >= 15)
            {
                if (Random.Range(1, 3) == 1)
                {
                    Player scp457 = Player.List.GetRandomValue(x => x.IsScp && x.IsAlive && !specialScps.Contains(x));
                    specialScps.Add(scp457);

                    Scp457.Create(scp457);
                }

                if (Random.Range(1, 3) == 1)
                {
                    Player scp966 = Player.List.GetRandomValue(x => x.IsScp && x.IsAlive && !specialScps.Contains(x));
                    specialScps.Add(scp966);

                    Scp966.Create(scp966);
                }
            }

            yield break;
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            if (ev.Player.IsAlive)
            {
                if (Random.Range(1, 6) == 1)
                {
                    ev.Player.AddItem(ItemType.Coin);
                }
            }
        }
    }
}
