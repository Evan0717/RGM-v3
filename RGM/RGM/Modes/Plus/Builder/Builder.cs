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

using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using Exiled.Events.EventArgs.Item;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using Mirror;
using PlayerRoles;

namespace RGM.Modes
{
    [Mode(ModeCategory.Private, ModeInfo.Plus, ModeType.Builder)]
    public class Builder : Mode
    {
        public override string Name => "건축가";
        public override string Description => "건축하세요. 재료는 오직 건축가의 체력 뿐입니다.";
        public override string Detail =>
"""
엄폐물 짓기에 실패하면 스텍이 쌓입니다. (<color=red>SCP-079</color>의 경우 [레벨 업])
스텍은 엄폐물을 짓거나, 가장 큰 엄폐물 스텍을 초과하면 초기화됩니다.

* 발차기(ALT)로 엄폐물을 부술 수 있습니다.
인간 -> (데미지: 40, 쿨타임: 0.5초)
<color=red>SCP</color> -> (데미지: 100, 쿨타임: 0.5초)

<b>[참고]</b>
• 인간은 동전을 버릴 수 없지만, <color=red>SCP</color>는 동전을 버릴 수 있습니다.
• <color=red>SCP-079</color>의 경우에는 설치 쿨타임이 10초입니다.
• 생성된 모든 엄폐물은 3분 뒤 자동으로 제거됩니다. (렉 방지)
• 고레벨의 엄폐물일수록 노동의 대가(체력)도 높아집니다.
• <color=red>벽을 뚫거나, 아군의 경로를 막거나, 탑 쌓고 올라가는 행위는 제재 대상입니다.</color>
""";
        public override string Color => "2EFEC8";
        public override string Suggester => "idea by 몬키키(@monkiki)";

        public static Builder Instance;

        List<Item> _tools = new List<Item>();
        List<Player> _cooldownPlayers = new List<Player>();
        List<Player> _scp079Cooldown = new List<Player>();

        Dictionary<string, List<object>> _objects = new Dictionary<string, List<object>>()
        {
            { "da_d2", new List<object>() { "<color=#ffffff>더미</color>", 15 } },
            { "da_b2", new List<object>() { "<color=#FAAC58>판자 Ⅰ</color>", 25 } },
            { "da_b1", new List < object >() { "<color=#FE9A2E>판자 Ⅱ</color>", 35 } },
            { "da_d1", new List < object >() { "<color=#BDBDBD>합금</color>", 50 } },
            { "da_l1", new List < object >() { "<color=#F7D358>모래주머니</color>", 70 } },
            { "da_o1", new List < object >() { "<color=#81F7F3>마공학 코어</color>", 120 } },
        };
        Dictionary<Player, int> _stacks = new Dictionary<Player, int>();

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
            Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;

            Exiled.Events.Handlers.Scp079.GainingLevel += OnGainingLevel;
            Exiled.Events.Handlers.Scp079.Pinging += OnPinging;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            Exiled.Events.Handlers.Player.Dying -= OnDying;
            Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;
            Exiled.Events.Handlers.Player.FlippingCoin -= OnFlippingCoin;
            Exiled.Events.Handlers.Player.TogglingNoClip -= OnTogglingNoClip;

            Exiled.Events.Handlers.Scp079.GainingLevel -= OnGainingLevel;
            Exiled.Events.Handlers.Scp079.Pinging -= OnPinging;

            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            foreach (var player in PlayerManager.List)
            {
                Verified(player);
                Spawned(player);
            }

            while (true)
            {
                foreach (var p in PlayerManager.List.Where(x => x.IsAlive))
                {
                    if (_tools.Contains(p.CurrentItem))
                    {
                        int stackValue = _stacks.ContainsKey(p) ? _stacks[p] : 0;
                        int objectIndex = stackValue >= _objects.Count ? _objects.Count - 1 : stackValue;
                        string selectedObject = _objects.ElementAt(objectIndex).Key;

                        p.AddHint("건축가", $"<size=25><b>건축 도구</b>ㅣLvl {objectIndex + 1}. {(string)_objects[selectedObject][0]} (❤️{(int)_objects[selectedObject][1]})</size>", 1.2f);
                    }
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
            if (player.IsDead)
                return;

            Timing.CallDelayed(1f, () =>
            {
                if (!player.Items.Select(x => x.Type).Contains(ItemType.Coin))
                {
                    Item _tool = player.AddItem(ItemType.Coin);

                    _tools.Add(_tool);

                    player.AddHint("건축가 경고", $"<b>⚠️ 주의하세요</b>, <color=red>벽을 뚫거나, 아군의 경로를 막거나, 탑 쌓고 올라가는 행위는 제재 대상입니다.</color>", 10);
                }
            });
        }

        public IEnumerator<float> OnDying(DyingEventArgs ev)
        {
            yield return Timing.WaitForOneFrame;

            if (ev.Player.IsDead)
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
        }

        public void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (ev.Player.CurrentItem == null)
                return;

            if (_tools.Contains(ev.Player.CurrentItem) && !ev.Player.IsScpRole())
            {
                ev.IsAllowed = false;
            }
        }
        
        public void OnFlippingCoin(FlippingCoinEventArgs ev)
        {
            if (_tools.Contains(ev.Player.CurrentItem) && Tools.TryGetRaycastPoint(ev.Player, 3, out Vector3 pos))
            {
                int stackValue = _stacks.ContainsKey(ev.Player) ? _stacks[ev.Player] : 0;
                int objectIndex = stackValue >= _objects.Count ? _objects.Count - 1 : stackValue;
                string selectedObject = _objects.ElementAt(objectIndex).Key;
                bool _isinElevator = Physics.RaycastAll(ev.Player.Position, Vector3.down, 5, (LayerMask)1)
                    .Any(hit => hit.transform.parent != null && hit.transform.parent.name == "ElevatorChamber Gates(Clone)")
                    || Physics.RaycastAll(new Vector3(pos.x, pos.y + 0.1f, pos.z), Vector3.down, 5, (LayerMask)1)
                        .Any(hit => hit.transform.parent != null && hit.transform.parent.name == "ElevatorChamber Gates(Clone)");
                
                if (!_isinElevator)
                    ev.Player.Hurt((int)_objects[selectedObject][1], "고된 노동이 목숨을 앗아갔습니다.");

                Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                {
                    if (!ev.Player.IsDead)
                    {
                        if (_isinElevator)
                            ev.Player.AddHint("엘레베이터", $"엘레베이터에는 엄폐물을 설치할 수 없습니다.", 1);

                        else
                        {
                            SchematicObject _object = ObjectSpawner.SpawnSchematic(selectedObject, pos, ev.Player.Rotation);

                            Timing.CallDelayed(180, () =>
                            {
                                _object.Destroy();
                                UnityEngine.Object.Destroy(_object.gameObject);
                            });
                        }
                    }
                });

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
                GGUtils.HealthObject.DamageObject(ev.Player, ev.Player.IsScpRole() ? 100 : 40, hit);

            _cooldownPlayers.Add(ev.Player);

            Timing.CallDelayed(0.5f, () =>
            {
                _cooldownPlayers.Remove(ev.Player);
            });
        }

        public void OnGainingLevel(GainingLevelEventArgs ev)
        {
            _stacks[ev.Player]++;
        }

        public void OnPinging(PingingEventArgs ev)
        {
            int stackValue = _stacks.ContainsKey(ev.Player) ? _stacks[ev.Player] : 0;
            int objectIndex = stackValue >= _objects.Count ? _objects.Count - 1 : stackValue;
            string selectedObject = _objects.ElementAt(objectIndex).Key;

            if (_scp079Cooldown.Contains(ev.Player) || ev.Scp079.Energy < (int)_objects[selectedObject][1])
            {

            }
            else
            {
                _scp079Cooldown.Add(ev.Player);

                ev.Scp079.Energy -= (int)_objects[selectedObject][1];

                SchematicObject _object = ObjectSpawner.SpawnSchematic(selectedObject, ev.Position, ev.Player.Rotation);

                Timing.CallDelayed(180, () =>
                {
                    _object.Destroy();
                    UnityEngine.Object.Destroy(_object.gameObject);
                });

                Timing.CallDelayed(10, () =>
                {
                    _scp079Cooldown.Remove(ev.Player);
                });
            }
        }
    }
}
