using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MEC;
using Mirror;
using MultiBroadcast;

using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.Cell)]
    class Cell : Mode
    {
        public override string Name => "고문";
        public override string Description => "고문을 최대한 오래 버티는 플레이어가 승리합니다.";
        public override string Detail =>
"""
좁은 방에서 <color=#FE2E2E>SCP-018</color>이 던져집니다!
60초마다 새로운 SCP-018이 스폰됩니다.

최대한 잘 피해 보세요!
""";
        public override string Color => "D7DF01";
        public override string Map => "cell";

        public static Cell Instance;

        List<Player> pl = new List<Player>();

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            Round.IsLocked = true;
            Server.FriendlyFire = true;

            Exiled.Events.Handlers.Player.Died += OnDied;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Round.IsLocked = false;
            Server.FriendlyFire = false;

            Exiled.Events.Handlers.Player.Died -= OnDied;

            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            PlayerManager.List.ToList().CopyTo(pl);

            foreach (var player in PlayerManager.List)
            {
                player.Role.Set(PlayerRoles.RoleTypeId.Tutorial);
                player.Position = GameObject.Find("[SP] Base").transform.position;
            }

            while (true)
            {
                Player badLucky = Tools.GetRandomValue(PlayerManager.List.Where(x => x.IsAlive).ToList());

                Throwable scp018 = (Throwable)badLucky.AddItem(ItemType.SCP018);
                Scp018 scp = (Scp018)scp018;
                scp.ChangeItemOwner(badLucky, null);
                badLucky.ThrowItem(scp018);
                badLucky.RemoveItem(scp018);

                yield return Timing.WaitForSeconds(60);
            }
        }

        public void OnDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            if (pl.Contains(ev.Player))
            {
                pl.Remove(ev.Player);

                if (pl.Count() < 2)
                {
                    Round.IsLocked = false;

                    PlayerManager.List.ToList().ForEach(x => x.AddBroadcast(20, $"승리자 : {pl[0].DisplayNickname}"));
                    Timing.RunCoroutine(Tools.SetWinner(new List<Player>() { pl[0] }, 5));
                }
            }
        }
    }
}
