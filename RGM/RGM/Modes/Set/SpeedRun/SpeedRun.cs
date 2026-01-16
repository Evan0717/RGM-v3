using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using MapGeneration.Distributors;
using MEC;
using Mirror;

using PlayerRoles;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.SpeedRun)]
    class SpeedRun : Mode
    {
        public override string Name => "스피드런";
        public override string Description => "누구보다 빠르게 시설에서 탈출하세요. 아, 그리고 핵을 중지하려 시도하지 마세요!";
        public override string Detail =>
"""
아이템이 <color=#C8FE2E>랜덤</color>하게 지급됩니다.

모든 방법을 총동원하여 1등으로 시설에서 탈출하세요.
그것 뿐입니다.
""";
        public override string Color => "58FAAC";

        public static SpeedRun Instance;

        List<Player> pl = new List<Player>();
        List<ItemType> _standards = new List<ItemType>()
        {
            ItemType.Flashlight
        };
        List<ItemType> _extensions = new List<ItemType>()
        {
            ItemType.KeycardO5,
            ItemType.Adrenaline,
            ItemType.Coin,
            ItemType.Lantern,
            ItemType.Medkit,
            ItemType.Painkillers
        };
        List<ItemType> _itemsList = new List<ItemType>();

        bool IsEnd = false;

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            Round.IsLocked = true;

            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.Escaping += OnEscaping;
            Exiled.Events.Handlers.Warhead.Stopping += OnStopping;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            Exiled.Events.Handlers.Player.Dying -= OnDying;
            Exiled.Events.Handlers.Player.Escaping -= OnEscaping;

            Exiled.Events.Handlers.Warhead.Stopping -= OnStopping;

            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            PlayerManager.List.ToList().CopyTo(pl);

            Warhead.Start();

            foreach (var Item in _standards)
                _itemsList.Add(Item);

            foreach (var Item in _extensions)
            {
                if (UnityEngine.Random.Range(1, 3) == 1)
                    _itemsList.Add(Item);
            }

            Tools.TryInstallMode(ModeType.FriendlyFire);

            foreach (var player in PlayerManager.List)
                Spawned(player);

            yield break;
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            if (pl.Contains(player))
            {
                if (player.Role.Type == RoleTypeId.ChaosConscript)
                    return;

                if (player.Role.Type != RoleTypeId.ClassD)
                    player.Role.Set(RoleTypeId.ClassD);

                Timing.CallDelayed(0.1f, () =>
                {
                    foreach (var _item in _itemsList)
                        player.AddItem(_item);
                });
            }
        }

        public void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            if (pl.Contains(ev.Player))
            {
                pl.Remove(ev.Player);

                if (pl.Count() < 2)
                {
                    if (!IsEnd)
                    {
                        Round.IsLocked = false;

                        PlayerManager.List.ToList().ForEach(x => x.AddBroadcast(20, $"<b><size=30><생존자 : {pl[0].DisplayNickname}></size></b>"));
                        Timing.RunCoroutine(Tools.SetWinner(new List<Player>() { pl[0] }, 5));
                        IsEnd = true;
                    }
                }
            }
        }

        public void OnEscaping(Exiled.Events.EventArgs.Player.EscapingEventArgs ev)
        {
            if (ev.Player.Role.Type == RoleTypeId.ClassD)
            {
                if (!IsEnd)
                {
                    Round.IsLocked = false;

                    PlayerManager.List.ToList().ForEach(x => x.AddBroadcast(20, $"<b><size=30><최초 탈출자 : {ev.Player.DisplayNickname}></size></b>"));
                    Timing.RunCoroutine(Tools.SetWinner(new List<Player>() { ev.Player }, 5));
                    IsEnd = true;
                }
            }
        }

        public void OnStopping(Exiled.Events.EventArgs.Warhead.StoppingEventArgs ev)
        {
            PlayerManager.List.ToList().ForEach(x => x.AddBroadcast(20, $"<size=30>{ev.Player.DisplayNickname}(으)로 인해 20초 뒤 <color=red>핵</color>이 강제로 폭파됩니다.</size>"));

            Timing.CallDelayed(20, Warhead.Detonate);
        }
    }
}
