using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.Features;
using RGM.Commands.RemoteAdminCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static RGM.Variables.Variable;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.TrickorTreat)]
    public class TrickorTreat : Mode
    {
        public override string Name => "트릭 오어 트릿";
        public override string Description => "재단에 사탕 파티가 열렸습니다!";
        public override string Detail =>
"""
<b><color=#E65000>무</color><color=#E5560F>작</color><color=#E55D1F>위</color> <color=#E46A3E>사</color><color=#E4714D>탕</color> <color=#E37E6C>4</color><color=#E3857C>개</color></b>를 획득합니다.

가끔씩 바닥에 꽁짜 사탕이 떨어질 수도 있겠죠.
아니면 누군가를 죽이거나..
""";
        public override string Color => "5F04B4";

        public static TrickorTreat Instance;

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.Dying += OnDying;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            Exiled.Events.Handlers.Player.Dying -= OnDying;

            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            foreach (var player in PlayerManager.List)
            {
                Spawned(player);
            }

            while (true)
            {
                foreach (var door in Door.List)
                {
                    if (UnityEngine.Random.Range(0, 100) < 1)
                    {
                        for (int i = 0; i < UnityEngine.Random.Range(1, 10); i++)
                            CandyParty.Create(Tools.PickRandomCandy(), UnityEngine.Random.Range(0.1f, 50), door.Position + new Vector3(0, 2, 0));
                    }
                }

                foreach (var player in PlayerManager.List)
                {
                    if (UnityEngine.Random.Range(0, 100) < 2)
                        CandyParty.Create(Tools.PickRandomCandy(), UnityEngine.Random.Range(0.1f, 50), player.Position);
                }

                GlobalPlayer.TryPlay("treat or treat", 1.5f);

                yield return Timing.WaitForSeconds(UnityEngine.Random.Range(1, 300));
            }
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            if (player.IsAlive && !player.IsNonePlayer())
            {
                Timing.CallDelayed(1f, () =>
                {
                    var Scp330 = player.AddItem(ItemType.SCP330);

                    for (int i = 1; i < 4; i++)
                    {
                        player.AddRandomCandy();
                    }
                });
            }
        }

        public void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            if (ev.Attacker == null)
                return;

            ev.Attacker.AddRandomCandy();
        }
    }
};
