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
using Exiled.Events.EventArgs.Item;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using Mirror;
using PlayerRoles;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.Builder)]
    public class Builder : Mode
    {
        public override string Name => "건축가";
        public override string Description => "건축하세요. 재료는 오직 건축가의 체력 뿐입니다.";
        public override string Detail =>
"""
건축!

엄폐물 짓기에 실패하면 스텍이 쌓입니다.
스텍은 엄폐물을 짓거나, 가장 큰 엄폐물 스텍을 초과하면 초기화됩니다.

* 발차기(ALT)로 엄폐물을 부술 수 있습니다.
인간 -> (데미지: 40, 쿨타임: 0.5초)
SCP -> (데미지: 200, 쿨타임: 0.5초)
""";
        public override string Color => "2EFEC8";
        public override string Suggester => "몬키키";

        public static Builder Instance;

        List<Item> _tools = new List<Item>();
        List<Player> _cooldownPlayers = new List<Player>();

        Dictionary<string, float> _objects = new Dictionary<string, float>()
        {
            { "da_b2", 25 },
            { "da_b1", 50 },
            { "da_d1", 75 },
            { "da_l1", 90 }
        };
        Dictionary<Player, int> _stacks = new Dictionary<Player, int>();

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
            Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;


            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            foreach (var player in Player.List)
            {
                Verified(player);
                Spawned(player);
            }

            while (true)
            {
                foreach (var p in Player.List.Where(x => x.IsAlive || x.Role.Type == RoleTypeId.Scp079))
                {
                    if (_tools.Contains(p.CurrentItem))
                        p.ShowHint("<size=25><b>건축 도구</b>, 엄폐물을 생성하세요.</size>", 1.2f);
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public void OnVerified(VerifiedEventArgs ev)
        {
            Verified(ev.Player);
        }

        public void Verified(Player player)
        {
            if (!_stacks.ContainsKey(player))
                _stacks.Add(player, 0);
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            Timing.CallDelayed(1f, () =>
            {
                if (!player.Items.Select(x => x.Type).Contains(ItemType.Coin))
                {
                    Item _tool = player.AddItem(ItemType.Coin);

                    _tools.Add(_tool);
                }
            });
        }

        public void OnDying(DyingEventArgs ev)
        {
            foreach (var _item in ev.Player.Items)
            {
                if (_tools.Contains(_item))
                {
                    _tools.Remove(_item);
                    ev.Player.RemoveItem(_item);
                }
            }
        }

        public void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (ev.Player.CurrentItem == null)
                return;

            if (_tools.Contains(ev.Player.CurrentItem))
            {
                ev.IsAllowed = false;
            }
        }
        
        public void OnFlippingCoin(FlippingCoinEventArgs ev)
        {
            if (Tools.TryGetRaycastPoint(ev.Player, 3, out Vector3 pos))
            {
                int stackValue = _stacks.ContainsKey(ev.Player) ? _stacks[ev.Player] : 0;
                int objectIndex = stackValue >= _objects.Count ? _objects.Count - 1 : stackValue;
                string selectedObject = _objects.ElementAt(objectIndex).Key;

                SchematicObject _object = ObjectSpawner.SpawnSchematic(selectedObject, pos, ev.Player.Rotation, isStatic: true);

                Timing.CallDelayed(180, () =>
                {
                    _object.Destroy();
                    NetworkServer.Destroy(_object.gameObject);
                });

                ev.Player.Hurt(_objects[selectedObject], "고된 노동이 목숨을 앗아갔습니다.");

                _stacks[ev.Player] = 0;
            }
            else
            {
                _stacks[ev.Player]++;

                if (_stacks[ev.Player] >= _objects.Count)
                    _stacks[ev.Player] = 0;
            }
        }

        public void OnTogglingNoClip(TogglingNoClipEventArgs ev)
        {
            if (_cooldownPlayers.Contains(ev.Player))
                return;

            Vector3 _forward = ev.Player.CameraTransform.forward;

            if (Physics.Raycast(ev.Player.ReferenceHub.PlayerCameraReference.position + ev.Player.ReferenceHub.PlayerCameraReference.forward * 0.2f, _forward, out RaycastHit hit, 3, (LayerMask)1))
                GGUtils.HealthObject.DamageObject(ev.Player, ev.Player.IsScp ? 200 : 40, hit);

            _cooldownPlayers.Add(ev.Player);

            Timing.CallDelayed(0.5f, () =>
            {
                _cooldownPlayers.Remove(ev.Player);
            });
        }
    }
}
