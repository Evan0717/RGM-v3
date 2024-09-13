using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using CommandSystem.Commands.RemoteAdmin;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Loader.Models;
using InventorySystem;
using InventorySystem.Items.Coin;
using InventorySystem.Items.Usables.Scp330;
using MapEditorReborn.API.Features.Objects;
using MEC;
using MultiBroadcast;
using MultiBroadcast.API;
using PluginAPI.Roles;
using UnityEngine;

namespace RGM.Modes
{
    class ABattle
    {
        public static ABattle Instance;

        public Dictionary<string, List<Vector3>> PlayerWorkstation = new Dictionary<string, List<Vector3>>();
        public Dictionary<string, List<string>> PlayerAbilities = new Dictionary<string, List<string>>();

        public List<string> BlackOutCooldown = new List<string>();
        public List<Player> MeleeCooldown = new List<Player>();
        public List<ushort> PickCoinSerials = new List<ushort>();
        public List<ushort> FollowCoinSerials = new List<ushort>();
        public List<ushort> GrapCoinSerials = new List<ushort>();
        public List<ushort> ClockCoinSerials = new List<ushort>();
        public List<ushort> FlashLightSerials = new List<ushort>();

        public List<Player> insurers = new List<Player>();
        public List<Player> fighters = new List<Player>();
        public List<Player> repairs = new List<Player>();
        public List<Player> ability941s = new List<Player>();
        public List<Player> magicians = new List<Player>();
        public List<Player> posions = new List<Player>();
        public List<Player> spirits = new List<Player>();
        public List<Player> twinkles = new List<Player>();

        public Dictionary<string, string> CommonAbilities = new Dictionary<string, string>()
        {
            {"[일반] 운동", "25%만큼 최대 체력을 추가합니다."},
            {"[일반] 경공", "이동 속도가 25% 증가합니다."},
            {"[일반] 진화", "몸의 크기가 12% 작아집니다."},
            {"[일반] 단련", "공격력이 20% 추가됩니다."},
            {"[일반] 행운", "5% 확률로 잠긴 문을 열 수 있습니다."},
            {"[일반] 체력 보충", "파란 사탕을 받습니다."},
            {"[일반] 랜덤박스", "랜덤한 아이템을 지급받습니다."},
            {"[일반] 위치 추적", "10초 간 랜덤한 1인의 위치를 확인합니다."},
            // {"[일반] 광고", "현재 진영 정보를 출력합니다."},
            {"[일반] 뽑기", "지급된 동전을 튕기면 10% 확률로 새로운 능력을 3개 더 얻습니다."},
            {"[일반] 보험", "사망 판정을 받을 경우 1번 버텨냅니다." },
            {"[일반] 회축", "[ALT]를 눌러 발차기 공격을 가할 수 있습니다. (쿨타임 1초)"},
            {"[일반] 보급", "탄약이 랜덤하게 지급됩니다."}
        };
        public Dictionary<string, string> RareAbilities = new Dictionary<string, string>()
        {
            {"[희귀] 육체 강화", "1초당 1HP를 받습니다."},
            {"[희귀] 강철 껍질", "데미지 경감 효과를 1 받습니다."},
            {"[희귀] 투명 망토", "25초 간 투명 효과를 받습니다."},
            {"[희귀] 흡혈귀", "상대에게 입힌 피해량의 20%만큼 AHP를 받습니다."},
            {"[희귀] 순간이동", "지급된 동전을 튕기면 랜덤한 유저의 위치로 순간이동합니다."},
            {"[희귀] 봄버맨", "랜덤한 유저의 위치에 고폭 수류탄을 투척합니다."},
            {"[희귀] 갈고리", "지급된 동전을 튕기면 랜덤한 1인을 끌어옵니다."},
            {"[희귀] 잠수", "스태미나가 줄어들지 않습니다."},
            {"[희귀] 고스트룰", "문을 통과할 수 있습니다."},
            {"[희귀] 회중시계", "지급된 동전을 튕기면 3초간 움직일 수 없는 대신에 무적 상태가 됩니다."}
        };
        public Dictionary<string, string> EpicAbilities = new Dictionary<string, string>()
        {
            {"[영웅] 테러리스트의 유품", "핑크 사탕을 지급받습니다."},
            {"[영웅] 도박꾼", "아이템을 버리면 새로운 아이템을 받지만, 5% 확률로 손이 잘립니다!"},
            {"[영웅] 랜덤상자", "랜덤하지만 좋은 아이템을 지급받습니다."},
            {"[영웅] 핵 리모컨", "핵 프로세스를 시작합니다."},
            {"[영웅] 수리 기사", "모든 엘레베이터를 15초 간 고장내고, 모든 잠겨진 문에 액세스할 수 있으며, 테슬라를 작동시키지 않습니다."},
            {"[영웅] 슈퍼 스타", "자신의 마이크가 모두에게 공유됩니다."},
            {"[영웅] 럭키비키", "이전에 방문했던 워크스테이션에서 다시 한번 더 능력을 획득할 수 있습니다."},
            {"[영웅] 극독", "누군가에게 죽으면 죽인 자에게 심장 마비 효과를 겁니다."},
            {"[영웅] 구사일생", "사망 판정을 받을 경우 1번 버텨내며 3초간 무적이 됩니다."}
        };
        public Dictionary<string, string> LegendAbilities = new Dictionary<string, string>()
        {
            {"[전설] 스피드왜건", "속도가 크게 증가합니다."},
            {"[전설] 뱀의 손 무전기", "즉시 뱀의 손 지원을 부르며, 자신도 뱀의 손 소속이 됩니다."},
            {"[전설] 모드 설치", "현재 라운드에 모드를 하나 더 추가합니다."},
            {"[전설] 랜덤택배", "서버 인원 수 만큼 랜덤한 아이템을 드롭합니다."},
            {"[전설] 마술사", "누군가에게 죽으면 죽인 자와 교체됩니다."},
            {"[전설] 플래시라이트", "지급된 손전등을 들고 상대를 쳐다보면 눈뽕 공격을 가할 수 있습니다."}
        };
        public Dictionary<string, string> MythicAbilities = new Dictionary<string, string>()
        {
            // {"[신화] 해킹", "시설 핵을 즉시 터트립니다."},
            {"[신화] 로켓 런처", "상대방을 하늘로 승천시킬 수 있습니다!"},
            {"[신화] 스피릿", "2초마다 영혼 상태가 됩니다!"},
            {"[신화] 눈빛맨", "쳐다보는 것만으로도 당신을 두려워할 것입니다!"}
        };

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
            Timing.RunCoroutine(UpgradeBody());
            Timing.RunCoroutine(Spirit());
            Timing.RunCoroutine(Twinkle());
            Timing.RunCoroutine(FlashLight());

            Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;
            Exiled.Events.Handlers.Player.Jumping += OnJumping;
            Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
            Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.InteractingDoor += InteractingDoor;
            Exiled.Events.Handlers.Player.InteractingLocker += InteractingLocker;
            Exiled.Events.Handlers.Player.DroppedItem += DroppedItem;
            Exiled.Events.Handlers.Player.Hurting += Hurting;
            Exiled.Events.Handlers.Player.ChangingSpectatedPlayer += ChangingSpectatedPlayer;
        }

        public IEnumerator<float> OnModeStarted()
        {
            // Server.ExecuteCommand($"/mp load ABattle");

            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (!PlayerWorkstation.ContainsKey(player.UserId))
                    {
                        PlayerWorkstation.Add(player.UserId, new List<Vector3>());
                        PlayerAbilities.Add(player.UserId, new List<string>());
                    }
                    else
                    {
                        if (!player.IsDead)
                        {
                            if (PlayerAbilities[player.UserId].Count <= 0)
                                player.ShowHint($"<align=left><b><size=22>워크스테이션 위에서 점프하면 능력을 획득할 수 있습니다.</size></b></align>", 1.2f);
                            else
                            {
                                string abilitiesText = string.Join(", ", PlayerAbilities[player.UserId]);
                                abilitiesText = abilitiesText.Replace("[신화]", "<color=#DF0101>[신화]</color>").Replace("[전설]", "<color=#ffd700>[전설]</color>").Replace("[영웅]", "<color=#FF00FF>[영웅]</color>").Replace("[희귀]", "<color=#2ECCFA>[희귀]</color>").Replace("[일반]", "<color=#A4A4A4>[일반]</color>");

                                player.ShowHint($"<align=left><b><size=25>보유 업그레이드</size></b>\n<size=20>{abilitiesText}</size></align>", 1.2f);
                            }
                        }
                    }

                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public IEnumerator<float> UpgradeBody()
        {
            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (PlayerAbilities[player.UserId].Contains("[희귀] 육체 강화"))
                        if (player.MaxHealth > player.Health)
                            player.Health += 1;
                }
                yield return Timing.WaitForSeconds(1f);
            }
        }

        public IEnumerator<float> Spirit()
        {
            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (spirits.Contains(player))
                        player.EnableEffect(EffectType.Invisible);
                }

                yield return Timing.WaitForSeconds(2f);
            }
        }

        public IEnumerator<float> Twinkle()
        {
            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (twinkles.Contains(player))
                    {
                        if (Physics.Raycast(player.ReferenceHub.PlayerCameraReference.position + player.ReferenceHub.PlayerCameraReference.forward * 0.2f, player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 2f, InventorySystem.Items.Firearms.Modules.StandardHitregBase.HitregMask) &&
                            hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
                        {
                            var target = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>());

                            if (player != target)
                            {
                                Hitmarker.SendHitmarkerDirectly(player.ReferenceHub, 0.5f);
                            }
                        }
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public IEnumerator<float> FlashLight()
        {
            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (player.CurrentItem != null && FlashLightSerials.Contains(player.CurrentItem.Serial))
                    {
                        if (Physics.Raycast(player.ReferenceHub.PlayerCameraReference.position + player.ReferenceHub.PlayerCameraReference.forward * 0.2f, player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 10f, InventorySystem.Items.Firearms.Modules.StandardHitregBase.HitregMask) &&
                            hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
                        {
                            var target = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>());

                            if (player != target)
                            {
                                Hitmarker.SendHitmarkerDirectly(player.ReferenceHub, 0.8f);
                                target.EnableEffect(EffectType.Flashed, 1, 1);
                            }
                        }
                    }
                }

                yield return Timing.WaitForSeconds(2f);
            }
        }

        // -------------------------------------------------------------------------------------------------------------------------- //

        public async void AddAbility(Player player)
        {

            int grade = UnityEngine.Random.Range(1, 1001);
            string abilityGrade;

            if (grade <= 600)
                abilityGrade = "[일반]";
            else if (grade <= 900)
                abilityGrade = "[희귀]";
            else if (grade <= 990)
                abilityGrade = "[영웅]";
            else if (grade <= 999)
                abilityGrade = "[전설]";
            else
                abilityGrade = "[신화]";

            Dictionary<string, string> AbilityList()
            {
                if (abilityGrade == "[일반]")
                    return CommonAbilities;
                else if (abilityGrade == "[희귀]")
                    return RareAbilities;
                else if (abilityGrade == "[영웅]")
                {
                    Cassie.Clear();
                    Server.ExecuteCommand($"/cassie_sl {player.DisplayNickname}(이)가 <color=#FF00FF>[영웅]</color> 업그레이드를 입수하였습니다.");
                    return EpicAbilities;
                }
                else if (abilityGrade == "[전설]")
                {
                    Cassie.Clear();
                    Server.ExecuteCommand($"/cassie_sl {player.DisplayNickname}(이)가 <color=#ffd700>[전설]</color> 업그레이드를 입수하였습니다.");
                    return LegendAbilities;
                }
                else
                {
                    Cassie.Clear();
                    Server.ExecuteCommand($"/cassie_sl {player.DisplayNickname}(이)가 <color=#DF0101>[신화]</color> 업그레이드를 입수하였습니다.");
                    return MythicAbilities;
                }
            }

            void ApplyGiveAbility(string abilityName)
            {
                PlayerAbilities[player.UserId].Add(abilityName);
                string styleName = abilityName.Replace("[신화]", "<color=#DF0101>[신화]</color>").Replace("[전설]", "<color=#ffd700>[전설]</color>").Replace("[영웅]", "<color=#FF00FF>[영웅]</color>").Replace("[희귀]", "<color=#2ECCFA>[희귀]</color>").Replace("[일반]", "<color=#A4A4A4>[일반]</color>");
                player.ClearBroadcasts();
                player.AddBroadcast(8, $"<size=20><b>다음 능력이 추가되었습니다.</b></size>\n<size=30>{styleName}</size>\n<size=25>{AbilityList()[abilityName]}</size>");
            }

            string abilityName = RGM.GetRandomValue(AbilityList().Keys.ToList());

            ApplyGiveAbility(abilityName);

            string aT = abilityName.Replace("[일반] ", "").Replace("[희귀] ", "").Replace("[영웅] ", "").Replace("[전설] ", "").Replace("[신화] ", "");

            switch (aT)
            {
                case "운동":
                    float maxHealth = player.MaxHealth;
                    float healthIncrease = maxHealth * 0.25f;
                    player.MaxHealth += healthIncrease;
                    player.Health += healthIncrease;
                    break;
                case "경공": player.GetEffect(EffectType.MovementBoost).Intensity += 10; break;
                case "진화": player.Scale = new Vector3(player.Scale.x - 0.12f, player.Scale.y - 0.12f, player.Scale.z - 0.12f); break;
                case "체력 보충": player.TryAddCandy(CandyKindID.Blue); break;
                case "랜덤박스":
                    int rn = UnityEngine.Random.Range(0, 55);

                    if (player.IsInventoryFull)
                        Server.ExecuteCommand($"/drop {player.Id} {rn} 1");

                    else
                        Server.ExecuteCommand($"/give {player.Id} {rn}");

                    if (player.IsScp)
                        Server.ExecuteCommand($"/forceeq {player.Id} {rn}");
                    break;
                case "위치 추적":
                    Player target1 = RGM.GetRandomValue(Player.List.Where(x => x.IsAlive).ToList());

                    for (int i = 1; i < 11; i++)
                    {
                        player.ShowHint($"소속이 <color={target1.Role.Color.ToHex()}>{target1.Role.Name}</color>인 ???은(는) <b>{target1.CurrentRoom.Name}</b>에 있습니다.", 1.2f);
                        await Task.Delay(1000);
                    }
                    break;
                case "광고": Server.ExecuteCommand($"/cassie_sl <b>[<color={player.Role.Color.ToHex()}>{player.DisplayNickname}</color>이(가) 출력한 진영 정보]</b>\n <color=red>SCP</color> : {Player.List.Where(x => x.IsScp).Count()} / <color=#088A29>혼돈의 반란</color> : {Player.List.Where(x => x.IsCHI).Count()} / <color=#0080FF>NTF</color> : {Player.List.Where(x => x.IsNTF).Count()}"); break;
                case "뽑기":
                    Item pc = player.AddItem(ItemType.Coin);
                    PickCoinSerials.Add(pc.Serial);

                    if (player.IsScp)
                        player.CurrentItem = pc;
                    break;
                case "보험": insurers.Add(player); break;
                case "주먹다짐": fighters.Add(player); break;
                case "보급":
                    List<AmmoType> Ammos = new List<AmmoType>() 
                    { 
                        AmmoType.Ammo44Cal,
                        AmmoType.Nato556,
                        AmmoType.Nato9,
                        AmmoType.Nato762,
                        AmmoType.Ammo12Gauge
                    };

                    player.AddAmmo(RGM.GetRandomValue(Ammos), 3);

                    break;
                case "강철 껍질": player.GetEffect(EffectType.DamageReduction).Intensity += 1; break;
                case "투명 망토": player.EnableEffect(EffectType.Invisible, 1, 25); break;
                case "순간이동":
                    Item fc = player.AddItem(ItemType.Coin);
                    FollowCoinSerials.Add(fc.Serial);

                    if (player.IsScp)
                        player.CurrentItem = fc;
                    break;
                case "봄버맨":
                    var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, player);
                    g.FuseTime = 3f;
                    g.SpawnActive(RGM.GetRandomValue(Player.List.ToList().Where(x => x.IsAlive && x.Role.Team != player.Role.Team && player != x).ToList()).Position, player);
                    break;
                case "갈고리":
                    Item gc = player.AddItem(ItemType.Coin);
                    GrapCoinSerials.Add(gc.Serial);

                    if (player.IsScp)
                        player.CurrentItem = gc;
                    break;
                case "잠수": player.IsUsingStamina = false; break;
                case "고스트룰": player.GetEffect(Exiled.API.Enums.EffectType.Ghostly).Intensity += 1; break;
                case "회중시계":
                    Item cc = player.AddItem(ItemType.Coin);
                    ClockCoinSerials.Add(cc.Serial);

                    if (player.IsScp)
                        player.CurrentItem = cc;
                    break;
                case "테러리스트의 유품": player.TryAddCandy(CandyKindID.Pink); break;
                case "랜덤상자":
                    int rn1 = RGM.GetRandomValue(new List<int> { 11, 16, 18, 24, 31, 32, 44, 45, 47, 48, 49, 50, 51, 52, 53 });

                    if (player.IsInventoryFull)
                        Server.ExecuteCommand($"/drop {player.Id} {rn1} 1");

                    else
                        Server.ExecuteCommand($"/give {player.Id} {rn1}");

                    if (player.IsScp)
                        Server.ExecuteCommand($"/forceeq {player.Id} {rn1}");
                    break;
                case "럭키비키": PlayerWorkstation[player.UserId].Clear(); break;
                case "핵 리모컨": Warhead.Start(); Server.ExecuteCommand($"/cassie_sl {player.DisplayNickname}(이)가 핵을 <b>원격으로 활성화했습니다.</b>"); break;
                case "수리 기사": Server.ExecuteCommand("/el l all"); player.IsBypassModeEnabled = true; await Task.Delay(15000); Server.ExecuteCommand("/el u all"); break;
                case "슈퍼 스타": Server.ExecuteCommand($"/speak {player.Id} enable"); break;
                case "극독": posions.Add(player); break;
                case "구사일생": ability941s.Add(player); break;
                case "스피드왜건": player.GetEffect(EffectType.MovementBoost).Intensity += 100; break;
                case "모드 설치":
                    string Mode1 = RGM.GetRandomValue(RGM.Instance.ModeList.Keys.Where(x => RGM.Instance.ModeList[x][3] != "private").ToList());
                    string mod1 = Mode1.ToString();

                    var modeType = Type.GetType($"RGM.Modes.{RGM.Instance.ModeList[Mode1][2]}");
                    if (modeType != null)
                    {
                        var modeInstance = Activator.CreateInstance(modeType);
                        var onEnabledMethod = modeType.GetMethod("OnEnabled");
                        onEnabledMethod?.Invoke(modeInstance, null);
                    }

                    Server.ExecuteCommand($"/cassie_sl {player.DisplayNickname}(이)가 [{mod1}] 모드를 설치했습니다.");
                    break;
                case "뱀의 손 무전기":
                    player.Role.Set(PlayerRoles.RoleTypeId.Tutorial, SpawnReason.ForceClass, PlayerRoles.RoleSpawnFlags.None);

                    List<Player> SnakeHands = Player.List.Where(x => x.IsDead).ToList();

                    foreach (var p in SnakeHands)
                    {
                        p.Role.Set(PlayerRoles.RoleTypeId.Tutorial);
                        p.Position = new Vector3(-0.08203125f, 1000.96f, 6.828125f);

                        foreach (ItemType Item in new List<ItemType> { ItemType.KeycardFacilityManager, ItemType.GunFSP9, ItemType.GunRevolver, ItemType.Adrenaline, ItemType.AntiSCP207 })
                            p.AddItem(Item);

                        for (int i = 1; i < 3; i++)
                            Player.List.ToList().ForEach(x => Server.ExecuteCommand($"/give {x.Id} 27.29."));
                    }

                    player.ShowHint($"<i>{SnakeHands.Count()}명의 <color=#FE2EF7>동료</color>들이 당신과 함께합니다..</i>", 5);
                    break;
                case "랜덤택배":
                    for (int i = 1; i < Player.List.Count() + 1; i++)
                        Server.ExecuteCommand($"/drop {player.Id} {UnityEngine.Random.Range(1, 55)} 1");
                    break;
                case "마술사": magicians.Add(player); break;
                case "플래시라이트":
                    Item fl = player.AddItem(ItemType.Flashlight);
                    FlashLightSerials.Add(fl.Serial);

                    if (player.IsScp)
                        player.CurrentItem = fl;
                    break;
                case "해킹": Warhead.Start(); Warhead.Detonate(); Server.ExecuteCommand($"/cassie_sl {player.DisplayNickname}(이)가 핵을 <b>원격으로 터트렸습니다.</b>"); break;
                case "스피릿": spirits.Add(player); break;
                case "눈빛맨": twinkles.Add(player); break;
            }
        }

        public async void OnTogglingNoClip(Exiled.Events.EventArgs.Player.TogglingNoClipEventArgs ev)
        {
            if (!ev.Player.IsCuffed)
            {
                if (Physics.Raycast(ev.Player.ReferenceHub.PlayerCameraReference.position + ev.Player.ReferenceHub.PlayerCameraReference.forward * 0.2f, ev.Player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 4f, InventorySystem.Items.Firearms.Modules.StandardHitregBase.HitregMask) &&
                    hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
                {
                    var player = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>());

                    if (ev.Player != player && !MeleeCooldown.Contains(ev.Player) && ev.Player.LeadingTeam != player.LeadingTeam)
                    {
                        Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 0.7f);
                        player.Hurt(12.05f * 3, "무지성으로 구타당해 죽었습니다.");

                        MeleeCooldown.Add(ev.Player);
                        await Task.Delay(1000);
                        MeleeCooldown.Remove(ev.Player);
                    }
                }
            }
        }

        public async void OnJumping(Exiled.Events.EventArgs.Player.JumpingEventArgs ev)
        {
            if (Physics.Raycast(ev.Player.Position, Vector3.down, out RaycastHit hit, 5, (LayerMask)1))
            {
                Transform WorkStation = hit.transform.parent.parent;

                if (WorkStation.name.Contains("Work Station") && !PlayerWorkstation[ev.Player.UserId].Contains(WorkStation.position))
                {
                    PlayerWorkstation[ev.Player.UserId].Add(WorkStation.position);

                    AddAbility(ev.Player);
                }
            }

            if (PlayerAbilities[ev.Player.UserId].Contains("[희귀] 블랙아웃"))
            {
                if (!BlackOutCooldown.Contains(ev.Player.UserId))
                {
                    BlackOutCooldown.Add(ev.Player.UserId);
                    ev.Player.CurrentRoom.TurnOffLights(3);
                    await Task.Delay(20000);
                    BlackOutCooldown.Remove(ev.Player.UserId);
                }
            }
        }

        public async void OnChangedItem(Exiled.Events.EventArgs.Player.ChangedItemEventArgs ev)
        {
            if (PickCoinSerials.Contains(ev.Item.Serial))
            {
                while (true)
                {
                    if (ev.Player.CurrentItem == null || !PickCoinSerials.Contains(ev.Player.CurrentItem.Serial))
                        break;

                    ev.Player.ShowHint("이 동전을 튕기면 <b>뽑기</b> 능력을 사용할 수 있습니다.", 1.2f);

                    await Task.Delay(1000);
                }
            }
            else if (FollowCoinSerials.Contains(ev.Item.Serial))
            {
                while (true)
                {
                    if (ev.Player.CurrentItem == null || !FollowCoinSerials.Contains(ev.Player.CurrentItem.Serial))
                        break;

                    ev.Player.ShowHint("이 동전을 튕기면 <b>순간이동</b> 능력을 사용할 수 있습니다.", 1.2f);

                    await Task.Delay(1000);
                }
            }
            else if (GrapCoinSerials.Contains(ev.Item.Serial))
            {
                while (true)
                {
                    if (ev.Player.CurrentItem == null || !GrapCoinSerials.Contains(ev.Player.CurrentItem.Serial))
                        break;

                    ev.Player.ShowHint("이 동전을 튕기면 <b>갈고리</b> 능력을 사용할 수 있습니다.", 1.2f);

                    await Task.Delay(1000);
                }
            }
            else if (ClockCoinSerials.Contains(ev.Item.Serial))
            {
                while (true)
                {
                    if (ev.Player.CurrentItem == null || !ClockCoinSerials.Contains(ev.Player.CurrentItem.Serial))
                        break;

                    ev.Player.ShowHint("이 동전을 튕기면 <b>회중시계</b> 능력을 사용할 수 있습니다.", 1.2f);

                    await Task.Delay(1000);
                }
            }
        }
        public async void OnFlippingCoin(Exiled.Events.EventArgs.Player.FlippingCoinEventArgs ev)
        {
            if (PickCoinSerials.Contains(ev.Item.Serial))
            {
                ev.Item.Destroy();

                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    for (int i = 1; i < 4; i++)
                        AddAbility(ev.Player);
                }
            }
            else if (FollowCoinSerials.Contains(ev.Item.Serial))
            {
                ev.Item.Destroy();

                Player target = RGM.GetRandomValue(Player.List.Where(x => x != ev.Player && x.IsAlive).ToList());
                ev.Player.Position = target.Position;
            }
            else if (GrapCoinSerials.Contains(ev.Item.Serial))
            {
                ev.Item.Destroy();

                Player target1 = RGM.GetRandomValue(Player.List.Where(x => x.IsAlive && x != ev.Player).ToList());
                target1.Position = ev.Player.Position;
            }
            if (ClockCoinSerials.Contains(ev.Item.Serial))
            {
                ev.Item.Destroy();

                ev.Player.EnableEffect(Exiled.API.Enums.EffectType.Ensnared, 1, 3);
                ev.Player.IsGodModeEnabled = true;

                await Task.Delay(3000);
                ev.Player.IsGodModeEnabled = false;
            }
        }

        public async void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            if (PlayerWorkstation.ContainsKey(ev.Player.UserId))
            {
                if (insurers.Contains(ev.Player))
                {
                    insurers.Remove(ev.Player);
                    ev.IsAllowed = false;
                    return;
                }
                if (ability941s.Contains(ev.Player))
                {
                    ability941s.Remove(ev.Player);
                    ev.IsAllowed = false;

                    ev.Player.EnableEffect(Exiled.API.Enums.EffectType.Blinded, 1, 3);
                    ev.Player.EnableEffect(Exiled.API.Enums.EffectType.Invisible, 1, 3);
                    ev.Player.GetEffect(Exiled.API.Enums.EffectType.MovementBoost).Intensity += 20;
                    ev.Player.IsGodModeEnabled = true;
                    await Task.Delay(3000);
                    ev.Player.GetEffect(Exiled.API.Enums.EffectType.MovementBoost).Intensity -= 20;
                    ev.Player.IsGodModeEnabled = false;
                    return;
                }

                PlayerAbilities[ev.Player.UserId].Clear();
                PlayerWorkstation[ev.Player.UserId].Clear();
                ev.Player.Scale = new Vector3(1, 1, 1);
                Server.ExecuteCommand($"/speak {ev.Player.Id} disable");
                ev.Player.IsUsingStamina = true;
                ev.Player.IsBypassModeEnabled = false;

                if (fighters.Contains(ev.Player))
                    fighters.Remove(ev.Player);

                if (BlackOutCooldown.Contains(ev.Player.UserId))
                    BlackOutCooldown.Remove(ev.Player.UserId);

                if (spirits.Contains(ev.Player))
                    spirits.Remove(ev.Player);

                if (repairs.Contains(ev.Player))
                    repairs.Remove(ev.Player);

                if (magicians.Contains(ev.Player))
                {
                    magicians.Remove(ev.Player);

                    ev.IsAllowed = false;

                    ev.Player.Role.Set(ev.Attacker.Role, Exiled.API.Enums.SpawnReason.ForceClass, PlayerRoles.RoleSpawnFlags.None);
                    ev.Player.Health = ev.Attacker.Health;
                    foreach (Item Item in ev.Attacker.Items)
                        ev.Player.AddItem(Item);

                    ev.Attacker.Kill($"몸이 교체되는 마술에 당했네요!");
                }

                if (posions.Contains(ev.Player))
                {
                    posions.Remove(ev.Player);

                    ev.Attacker.EnableEffect(Exiled.API.Enums.EffectType.CardiacArrest);
                }
            }
        }

        public void InteractingDoor(Exiled.Events.EventArgs.Player.InteractingDoorEventArgs ev)
        {
            if (ev.Player != null && PlayerAbilities[ev.Player.UserId].Contains("[일반] 행운") && UnityEngine.Random.Range(0, 100) <= 5)
            {
                if (ev.Door.IsOpen)
                    ev.Door.IsOpen = false;

                else
                    ev.Door.IsOpen = true;
            }
        }

        public void InteractingLocker(Exiled.Events.EventArgs.Player.InteractingLockerEventArgs ev)
        {
            if (ev.Player != null && PlayerAbilities[ev.Player.UserId].Contains("[일반] 행운") && UnityEngine.Random.Range(0, 100) <= 5)
            {
                if (ev.Chamber.IsOpen)
                    ev.Chamber.IsOpen = false;

                else
                    ev.Chamber.IsOpen = true;
            }
        }

        public void OnTriggeringTesla(Exiled.Events.EventArgs.Player.TriggeringTeslaEventArgs ev)
        {
            if (repairs.Contains(ev.Player))
                ev.IsAllowed = false;
        }

        public void DroppedItem(Exiled.Events.EventArgs.Player.DroppedItemEventArgs ev)
        {
            if (PlayerAbilities[ev.Player.UserId].Contains("[영웅] 도박꾼"))
            {
                if (UnityEngine.Random.Range(0, 100) <= 5)
                    ev.Player.EnableEffect(Exiled.API.Enums.EffectType.SeveredHands);

                else
                {
                    ev.Pickup.Destroy();
                    Server.ExecuteCommand($"/drop {ev.Player.Id} {UnityEngine.Random.Range(0, 55)} 1");
                }
            }
        }

        public void Hurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            try
            {
                if (PlayerAbilities[ev.Player.UserId].Contains("[일반] 단련"))
                {
                    int count = PlayerAbilities.Values.Count(list => list.Contains("[일반] 단련"));

                    ev.DamageHandler.Damage = (int)(ev.DamageHandler.Damage * (1 + (0.2 * count)));
                }

                if (PlayerAbilities[ev.Attacker.UserId].Contains("[희귀] 흡혈귀"))
                    ev.Attacker.AddAhp(20 * (ev.DamageHandler.Damage / 100));

                if (PlayerAbilities[ev.Attacker.UserId].Contains("[신화] 로켓 런처") && ev.Attacker != ev.Player)
                    Server.ExecuteCommand($"/rocket {ev.Player.Id} 1");
            }
            catch (Exception ex)
            {

            }
        }

        public void ChangingSpectatedPlayer(Exiled.Events.EventArgs.Player.ChangingSpectatedPlayerEventArgs ev)
        {
            if (ev.NewTarget != null)
            {
                if (PlayerAbilities[ev.NewTarget.UserId].Count <= 0)
                    ev.Player.ShowHint($"<align=left><b><size=22>워크스테이션 위에서 점프하면 능력을 획득할 수 있습니다.</size></b></align>", 250f);
                else
                {
                    string abilitiesText = string.Join(", ", PlayerAbilities[ev.NewTarget.UserId]);
                    abilitiesText = abilitiesText.Replace("[신화]", "<color=#DF0101>[신화]</color>").Replace("[전설]", "<color=#ffd700>[전설]</color>").Replace("[영웅]", "<color=#FF00FF>[영웅]</color>").Replace("[희귀]", "<color=#2ECCFA>[희귀]</color>").Replace("[일반]", "<color=#A4A4A4>[일반]</color>");

                    ev.Player.ShowHint($"<align=left><b><size=25>보유 업그레이드</size></b>\n<size=20>{abilitiesText}</size></align>", 250f);
                }
            }
        }

    }
}
