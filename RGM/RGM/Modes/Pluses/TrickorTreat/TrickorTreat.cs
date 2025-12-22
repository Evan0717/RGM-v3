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

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.TrickorTreat)]
    public class TrickorTreat : Mode
    {
        public override string Name => "트릭 오어 트릿";
        public override string Description => "사탕 4개를 가지고 시작합니다. 다른 이를 사살하면 사탕 1개를 더 받습니다.";
        public override string Detail =>
"""
<b><i><color=#E65000>무</color><color=#E5560F>작</color><color=#E55D1F>위</color> <color=#E46A3E>사</color><color=#E4714D>탕</color> <color=#E37E6C>4</color><color=#E3857C>개</color></i></b>를 획득합니다.
<color=#FA58F4>핑크 캔디</color>도 다른 사탕과 동일한 확률로 등장합니다.
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

            yield break;
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            if (player.IsAlive)
            {
                Timing.CallDelayed(1f, () =>
                {
                    var Scp330 = player.AddItem(ItemType.SCP330);

                    for (int i = 1; i < 4; i++)
                    {
                        var Candy = Tools.GetRandomValue(Tools.EnumToList<CandyKindID>());
                        player.AddCandy(Candy);
                    }
                });
            }
        }

        public void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            if (ev.Attacker == null)
                return;

            List<CandyKindID> CandyList = Tools.EnumToList<CandyKindID>();
            {
                var toGive = Tools.GetRandomValue(CandyList);
                ev.Attacker.AddCandy(toGive);

                if (ev.Player.IsScpRole())
                    Server.ExecuteCommand($"/forceeq {ev.Player.Id} 42");
            }
        }
    }
};
