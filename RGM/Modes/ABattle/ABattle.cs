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
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Loader.Models;
using InventorySystem;
using InventorySystem.Items.Coin;
using InventorySystem.Items.Usables.Scp330;
using MapEditorReborn.API.Features.Objects;
using MEC;
using MultiBroadcast;
using MultiBroadcast.API;
using PlayerRoles;
using PluginAPI.Roles;
using UnityEngine;

namespace RGM.Modes
{
    class ABattle
    {
        public static ABattle Instance;

        public bool IsFeverModeEnabled = false;
        public Dictionary<Player, List<Vector3>> PlayerWorkstation = new Dictionary<Player, List<Vector3>>();
        public Dictionary<Player, List<string>> PlayerAbilities = new Dictionary<Player, List<string>>();

        public List<Player> BlackOutCooldown = new List<Player>();
        public List<Player> MeleeCooldown = new List<Player>();
        public List<Player> PickPocketCooldown = new List<Player>();
        public List<ushort> PickCoinSerials = new List<ushort>();
        public List<ushort> FollowCoinSerials = new List<ushort>();
        public List<ushort> GrapCoinSerials = new List<ushort>();
        public List<ushort> ClockCoinSerials = new List<ushort>();
        public List<ushort> InstallModeSerials = new List<ushort>();
        public List<ushort> CallSnakeHandsSerials = new List<ushort>();
        public List<ushort> FlashLightSerials = new List<ushort>();
        public List<ushort> ChaosCoinSerials = new List<ushort>();

        public List<Player> insurers = new List<Player>();
        public List<Player> fighters = new List<Player>();
        public List<Player> martyrs = new List<Player>();
        public List<Player> repairs = new List<Player>();
        public List<Player> ability941s = new List<Player>();
        public List<Player> culprits = new List<Player>();
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
            {"[일반] 보급", "탄약이 랜덤하게 지급됩니다."},
            {"[일반] 정화", "초록 사탕을 받습니다."},
            {"[일반] 무기 전문가", "SCP-1853을 받습니다."},
            {"[일반] 신내림", "당신을 지켜보는 관전자가 5초 이내로 나타나면 능력 2~3개를 추가로 얻습니다."},
            {"[일반] 횃불", "랜턴과 노란 사탕을 받습니다."}
        };
        public Dictionary<string, string> RareAbilities = new Dictionary<string, string>()
        {
            {"[희귀] 육체 강화", "1초당 1HP를 받습니다."},
            {"[희귀] 강철 껍질", "데미지 경감 효과가 5% 추가됩니다."},
            {"[희귀] 투명 망토", "25초 간 투명 효과를 받습니다."},
            {"[희귀] 흡혈귀", "상대에게 입힌 피해량의 20%만큼 AHP를 받습니다."},
            {"[희귀] 순간이동", "지급된 동전을 튕기면 랜덤한 유저의 위치로 순간이동합니다."},
            {"[희귀] 봄버맨", "랜덤한 유저의 위치에 고폭 수류탄을 투척합니다."},
            {"[희귀] 갈고리", "지급된 동전을 튕기면 랜덤한 1인을 끌어옵니다."},
            {"[희귀] 회중시계", "지급된 동전을 튕기면 3초간 움직일 수 없는 대신에 무적 상태가 됩니다."},
            {"[희귀] 스테로이드", "25초 간 이동 속도가 많이 증가합니다."},
            {"[희귀] 순교", "사망할 시 해당 지역에 점화된 수류탄을 떨굽니다."},
            {"[희귀] 하이패스", "25초 간 무적이 됩니다."}
        };
        public Dictionary<string, string> EpicAbilities = new Dictionary<string, string>()
        {
            {"[영웅] 테러리스트의 유품", "핑크 사탕을 지급받습니다."},
            {"[영웅] 도박꾼", "아이템을 버리면 새로운 아이템을 받지만, 5% 확률로 손이 잘립니다."},
            {"[영웅] 랜덤상자", "랜덤하지만 좋은 아이템을 지급받습니다."},
            // {"[영웅] 핵 리모컨", "핵 프로세스를 시작합니다."},
            {"[영웅] 수리 기사", "모든 잠겨진 문에 액세스할 수 있으며, 테슬라를 작동시키지 않습니다."},
            {"[영웅] 슈퍼 스타", "자신의 마이크가 모두에게 공유됩니다."},
            {"[영웅] 럭키비키", "이전에 방문했던 워크스테이션에서 다시 한번 더 능력을 획득할 수 있습니다."},
            {"[영웅] 극독", "누군가에게 죽으면 죽인 자에게 심장 마비 효과를 겁니다."},
            {"[영웅] 구사일생", "사망 판정을 받을 경우 1번 버텨내며 3초간 무적이 됩니다."},
            {"[영웅] 최후의 발악", "5초 뒤 반드시 죽지만, 그동안 무적이 되며 속도가 매우 빨라집니다."},
            {"[영웅] 초재생", "핑크 콜라를 지급받습니다."},
            {"[영웅] 고스트룰", "문을 통과할 수 있습니다."},
            {"[영웅] 잠수부", "스태미나가 줄어들지 않습니다."}
        };
        public Dictionary<string, string> LegendAbilities = new Dictionary<string, string>()
        {
            {"[전설] 스피드왜건", "속도가 크게 증가합니다."},
            {"[전설] 뱀의 손 무전기", "지급된 무전기를 조작하면 뱀의 손 지원을 부르며, 자신도 뱀의 손 소속이 됩니다."},
            // {"[전설] 모드 설치", "지급된 동전을 튕기면 모드를 하나 더 추가할 수 있습니다."},
            {"[전설] 랜덤택배", "서버 인원 수 만큼 랜덤한 아이템을 드롭합니다."},
            {"[전설] 마술사", "누군가에게 죽으면 죽인 자와 교체됩니다."},
            {"[전설] 플래시라이트", "지급된 손전등을 들고 상대를 쳐다보면 눈뽕 공격을 가할 수 있습니다."},
            {"[전설] 킬스트릭", "누군가를 죽일 때마다 새로운 능력을 얻습니다."}
        };
        public Dictionary<string, string> MythicAbilities = new Dictionary<string, string>()
        {
            // {"[신화] 해킹", "시설 핵을 즉시 터트립니다."},
            {"[신화] 로켓 런처", "5% 확률로 상대방을 하늘로 승천시킬 수 있습니다!"},
            {"[신화] 스피릿", "2초마다 영혼 상태가 됩니다!"},
            {"[신화] 눈빛맨", "쳐다보는 것만으로도 당신을 두려워할 것입니다!"}
        };
        public Dictionary<string, string> ClassDAbilities = new Dictionary<string, string>()
        {
            {"[전용] 소매치기", "[ALT]를 눌러 상대의 아이템 중 하나를 빼앗을 수 있습니다. (쿨타임 1분)"}
        };
        public Dictionary<string, string> ScientistAbilities = new Dictionary<string, string>()
        {
            {"[전용] 05 평의회", "05등급 키카드를 지급받습니다."}
        };
        public Dictionary<string, string> GuardAbilities = new Dictionary<string, string>()
        {
            {"[전용] 관리 의무자", "손전등과 Crossvec를 지급받습니다."}
        };
        public Dictionary<string, string> NtfAbilities = new Dictionary<string, string>()
        {
            {"[전용] 격리 의무자", "고폭 수류탄과 섬광탄을 지급받습니다."}
        };
        public Dictionary<string, string> ChaosAbilities = new Dictionary<string, string>()
        {
            {"[전용] 카오스 볼", "SCP-018을 획득합니다."},
            {"[전용] 혼돈의 손길", "지급된 동전을 튕기면 보유한 능력을 전부 삭제합니다."}
        };
        public Dictionary<string, string> SnakeAbilities = new Dictionary<string, string>()
        {
            {"[전용] 세치 혀", "SCP-1576을 획득합니다."}
        };
        public Dictionary<string, string> Scp173Abilities = new Dictionary<string, string>()
        {
            {"[전용] 공포", "인간을 죽이면 그 방에 있는 인간들이 0.5초 동안 움직일 수 없게 됩니다."}
        };
        public Dictionary<string, string> Scp049Abilities = new Dictionary<string, string>()
        {
            {"[전용] 사자", "공격 쿨타임이 1/2배가 됩니다."}
        };
        public Dictionary<string, string> Scp0492Abilities = new Dictionary<string, string>()
        {
            {"[전용] 허기", "인간을 섭취할 때 얻는 회복량이 2배가 됩니다."}
        };
        public Dictionary<string, string> Scp096Abilities = new Dictionary<string, string>()
        {
            {"[전용] 분노", "분노 때에는 어떠한 데미지도 입지 않습니다."}
        };
        public Dictionary<string, string> Scp106Abilities = new Dictionary<string, string>()
        {
            {"[전용] 회춘", "데미지를 20% 경감시키는 효과를 받습니다."}
        };
        public Dictionary<string, string> Scp939Abilities = new Dictionary<string, string>()
        {
            {"[전용] 흉내쟁이", "흉내 쿨타임이 1/2배가 됩니다."}
        };
        public Dictionary<string, string> Scp3114Abilities = new Dictionary<string, string>()
        {
            {"[전용] 숙련된 암살자", "교살 데미지가 10배 증가합니다."}
        };
        public Dictionary<string, string> Scp079Abilities = new Dictionary<string, string>() 
        {
            {"[전용] 핑 리모컨", "핑한 장소에 0.5초 간 정전이 됩니다."},
            {"[전용] 간이 충전기", "즉시 20 경험치를 받습니다."},
            {"[전용] 과전류", "20초 간 전력이 무제한이 됩니다."},
            {"[전용] 랜덤 함수", "정전하면 랜덤한 방 5개를 더 정전합니다."},
            {"[전용] RTX4090", "격리당하면 튜토리얼(능력 5~10개)로 부활합니다."},
            {"[전용] 고대의 존재 압도", "V키를 연타하면 해당 방에 있는 인간의 속도가 감소합니다."}
        };

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;
            Exiled.Events.Handlers.Player.Jumping += OnJumping;
            Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
            Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
            Exiled.Events.Handlers.Player.TogglingRadio += OnTogglingRadio;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Player.DroppedItem += OnDroppedItem;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.Hurt += OnHurt;
            Exiled.Events.Handlers.Player.TriggeringTesla += OnTriggeringTesla;

            Exiled.Events.Handlers.Scp0492.ConsumedCorpse += OnConsumedCorpse;

            Exiled.Events.Handlers.Scp079.Pinging += OnPinging;
            Exiled.Events.Handlers.Scp079.ZoneBlackout += OnZoneBlackout;
            Exiled.Events.Handlers.Scp079.ChangingSpeakerStatus += OnChangingSpeakerStatus;

            Timing.RunCoroutine(OnModeStarted());
            Timing.RunCoroutine(UpgradeBody());
            Timing.RunCoroutine(Spirit());
            Timing.RunCoroutine(Twinkle());
            Timing.RunCoroutine(FlashLight());
        }

        public void ShowStatus(Player player, Player target)
        {
            if (!PlayerWorkstation.ContainsKey(target))
            {
                PlayerWorkstation.Add(target, new List<Vector3>());
                PlayerAbilities.Add(target, new List<string>());
            }
            else
            {
                if (PlayerAbilities[target].Count <= 0)
                    player.ShowHint($"<align=left><b><size=22>워크스테이션 위에서 점프하면 능력을 획득할 수 있습니다.</size></b></align>", 0.6f);

                else
                {
                    string abilitiesText = string.Join(", ", PlayerAbilities[target]);
                    abilitiesText = abilitiesText.Replace("[전용]", "<color=#F7819F>[전용]</color>").Replace("[신화]", "<color=#DF0101>[신화]</color>").Replace("[전설]", "<color=#ffd700>[전설]</color>").Replace("[영웅]", "<color=#FF00FF>[영웅]</color>").Replace("[희귀]", "<color=#2ECCFA>[희귀]</color>").Replace("[일반]", "<color=#A4A4A4>[일반]</color>");

                    player.ShowHint($"<align=left><b><size=25>보유 업그레이드</size></b>\n<size=20>{abilitiesText}</size></align>", 0.6f);
                }
            }
        }

        public IEnumerator<float> OnModeStarted()
        {
            if (UnityEngine.Random.Range(1, 6) == 1)
                IsFeverModeEnabled = true;

            if (IsFeverModeEnabled)
            {
                Server.ExecuteCommand($"/mp load ABattle");
                Player.List.ToList().ForEach(x => x.AddBroadcast(10, "<size=25><b><i><color=#FF00EA>피</color><color=#EF00EB>버</color> <color=#CF00ED>모</color><color=#BF00EF>드</color><color=#AF00F0>가</color> <color=#8F00F3>활</color><color=#7F00F4>성</color><color=#6F00F5>화</color><color=#5F00F7>되</color><color=#4F00F8>었</color><color=#3F00F9>습</color><color=#2F00FB>니</color><color=#1F00FC>다</color><color=#0F00FD>!</color></i></b></size>"));
            }

            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (player.IsAlive)
                        ShowStatus(player, player);

                    else
                    {
                        if (player.Role is SpectatorRole spectator)
                        {
                            if (spectator.SpectatedPlayer != null)
                                ShowStatus(player, spectator.SpectatedPlayer);
                        }
                    }
                }

                yield return Timing.WaitForSeconds(0.5f);
            }
        }

        public IEnumerator<float> UpgradeBody()
        {
            while (true)
            {
                foreach (var player in Player.List.Where(PlayerAbilities.ContainsKey))
                {
                    if (PlayerAbilities[player].Contains("[희귀] 육체 강화"))
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
                foreach (var player in Player.List.Where(PlayerAbilities.ContainsKey))
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
                foreach (var player in Player.List.Where(PlayerAbilities.ContainsKey))
                {
                    if (twinkles.Contains(player))
                    {
                        if (Physics.Raycast(player.ReferenceHub.PlayerCameraReference.position + player.ReferenceHub.PlayerCameraReference.forward * 0.2f, player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 100f, InventorySystem.Items.Firearms.Modules.StandardHitregBase.HitregMask) &&
                            hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
                        {
                            var target = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>());

                            if (player != target && player.LeadingTeam != target.LeadingTeam)
                            {
                                target.EnableEffect(EffectType.SinkHole, 1, 0.2f);
                                target.EnableEffect(EffectType.Blinded, 1, 0.2f);
                                Hitmarker.SendHitmarkerDirectly(player.ReferenceHub, 0.5f);
                            }
                        }
                    }
                }

                yield return Timing.WaitForSeconds(0.1f);
            }
        }

        public IEnumerator<float> FlashLight()
        {
            while (true)
            {
                foreach (var player in Player.List.Where(PlayerAbilities.ContainsKey))
                {
                    if (player.CurrentItem != null && FlashLightSerials.Contains(player.CurrentItem.Serial))
                    {
                        if (Physics.Raycast(player.ReferenceHub.PlayerCameraReference.position + player.ReferenceHub.PlayerCameraReference.forward * 0.2f, player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 45f, InventorySystem.Items.Firearms.Modules.StandardHitregBase.HitregMask) &&
                            hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
                        {
                            var target = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>());

                            if (player != target && player.LeadingTeam != target.LeadingTeam)
                            {
                                Hitmarker.SendHitmarkerDirectly(player.ReferenceHub, 0.8f);
                                target.EnableEffect(EffectType.Flashed, 1, 0.2f);
                            }
                        }
                    }
                }

                yield return Timing.WaitForSeconds(0.1f);
            }
        }

        // -------------------------------------------------------------------------------------------------------------------------- //

        public async void AddAbility(Player player, bool force = false)
        {
            if (player.IsScp && player.Role.Type != RoleTypeId.Scp0492)
            {
                foreach (var scp079 in Player.List.Where(x => x.IsAlive))
                {
                    if (scp079.Role.Type == RoleTypeId.Scp079)
                    {
                        if (UnityEngine.Random.Range(1, 21) == 1)
                        {
                            if (scp079.Role.Type == RoleTypeId.Scp079)
                            {
                                foreach (var scp in Player.List.Where(x => x.IsScp))
                                    scp.AddBroadcast(10, $"<size=25><color=red>{player.Nickname}</color>({player.Role.Name}) 덕분에 <color=red>{scp079.Nickname}</color>({scp079.Role.Name})(이)가 능력을 획득하였습니다.</size>");

                                AddAbility(scp079, true);
                                break;
                            }
                        }
                    }
                }
            }

            if (force || (UnityEngine.Random.Range(1, 2) == 1))
            {

                int grade = UnityEngine.Random.Range(1, 1001);
                string abilityGrade;

                if (player.Role.Type == RoleTypeId.Scp079)
                    abilityGrade = "[전용]";

                else
                {
                    if (grade < 2)
                        abilityGrade = "[신화]";

                    else if (grade < 7)
                        abilityGrade = "[전설]";

                    else if (grade < 57)
                        abilityGrade = "[영웅]";

                    else if (grade < 257)
                        abilityGrade = "[희귀]";

                    else if (grade < 317)
                        abilityGrade = "[전용]";

                    else
                        abilityGrade = "[일반]";
                }

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
                    else if (abilityGrade == "[신화]")
                    {
                        Cassie.Clear();
                        Server.ExecuteCommand($"/cassie_sl {player.DisplayNickname}(이)가 <color=#DF0101>[신화]</color> 업그레이드를 입수하였습니다.");
                        return MythicAbilities;
                    }
                    else
                    {
                        Dictionary<string, string> Format()
                        {
                            RoleTypeId role = player.Role.Type;

                            if (role == RoleTypeId.ClassD)
                                return ClassDAbilities;
                            else if (role == RoleTypeId.Scientist)
                                return ScientistAbilities;
                            else if (role == RoleTypeId.FacilityGuard)
                                return GuardAbilities;
                            else if (player.IsNTF)
                                return NtfAbilities;
                            else if (player.IsCHI)
                                return ChaosAbilities;
                            else if (role == RoleTypeId.Tutorial)
                                return SnakeAbilities;
                            else if (role == RoleTypeId.Scp173)
                                return Scp173Abilities;
                            else if (role == RoleTypeId.Scp049)
                                return Scp049Abilities;
                            else if (role == RoleTypeId.Scp0492)
                                return Scp0492Abilities;
                            else if (role == RoleTypeId.Scp096)
                                return Scp096Abilities;
                            else if (role == RoleTypeId.Scp106)
                                return Scp106Abilities;
                            else if (role == RoleTypeId.Scp939)
                                return Scp939Abilities;
                            else if (role == RoleTypeId.Scp3114)
                                return Scp3114Abilities;
                            else
                                return Scp079Abilities;
                        }

                        Cassie.Clear();
                        Server.ExecuteCommand($"/cassie_sl {player.DisplayNickname}(이)가 <color=#F7819F>[전용]</color> 업그레이드를 입수하였습니다.");
                        return Format();
                    }
                }

                void ApplyGiveAbility(string abilityName)
                {
                    PlayerAbilities[player].Add(abilityName);
                    string styleName = abilityName.Replace("[전용]", "<color=#F7819F>[전용]</color>").Replace("[신화]", "<color=#DF0101>[신화]</color>").Replace("[전설]", "<color=#ffd700>[전설]</color>").Replace("[영웅]", "<color=#FF00FF>[영웅]</color>").Replace("[희귀]", "<color=#2ECCFA>[희귀]</color>").Replace("[일반]", "<color=#A4A4A4>[일반]</color>");

                    string Message = $"<size=20><b>다음 능력이 추가되었습니다.</b></size>\n<size=30>{styleName}</size>\n<size=25>{AbilityList()[abilityName]}</size>";
                    player.AddBroadcast(8, Message);
                    player.SendConsoleMessage($"\n{Message}", "white");
                }

                string abilityName = RGM.GetRandomValue(AbilityList().Keys.ToList());

                ApplyGiveAbility(abilityName);

                string aT = abilityName.Replace("[전용]", "").Replace("[일반] ", "").Replace("[희귀] ", "").Replace("[영웅] ", "").Replace("[전설] ", "").Replace("[신화] ", "");

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
                    case "체력 보충":
                        player.TryAddCandy(CandyKindID.Blue);

                        if (player.IsScp)
                            Server.ExecuteCommand($"/forceeq {player.Id} 42");
                        break;
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
                    case "회축": fighters.Add(player); break;
                    case "보급":
                        List<string> Ammos = new List<string> { "19", "22", "27", "28", "29" };

                        for (int i = 1; i < 4; i++)
                            Server.ExecuteCommand($"/give {player.Id} {RGM.GetRandomValue(Ammos)}");
                        break;
                    case "정화":
                        player.TryAddCandy(CandyKindID.Green);

                        if (player.IsScp)
                            Server.ExecuteCommand($"/forceeq {player.Id} 42");
                        break;
                    case "무기 전문가":
                        Item scp1853 = player.AddItem(ItemType.SCP1853);

                        if (player.IsScp)
                            player.CurrentItem = scp1853;
                        break;
                    case "신내림":
                        bool Pass = false;

                        for (int i = 1; i < 61; i++)
                        {
                            if (player.CurrentSpectatingPlayers.Count() > 0)
                            {
                                player.AddBroadcast(5, $"<size=25>당신은 {player.CurrentSpectatingPlayers.ToList()[0].Nickname}로부터 신내림을 받았습니다.</size>");
                                player.CurrentSpectatingPlayers.ToList()[0].AddBroadcast(5, $"<size=25>당신은 {player.Nickname}(에)게 신내림을 내려주었습니다.</size>");
                                Pass = true;
                                break;
                            }

                            await Task.Delay(100);
                        }

                        if (Pass)
                        {
                            for (int i = 1; i < UnityEngine.Random.Range(3, 5); i++)
                                AddAbility(player);
                        }
                        break;
                    case "횃불":
                        player.AddItem(ItemType.Lantern);
                        player.TryAddCandy(CandyKindID.Yellow);

                        if (player.IsScp)
                            Server.ExecuteCommand($"/forceeq {player.Id} 42");
                        break;
                    case "강철 껍질": player.GetEffect(EffectType.DamageReduction).Intensity += 10; break;
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
                    case "회중시계":
                        Item cc = player.AddItem(ItemType.Coin);
                        ClockCoinSerials.Add(cc.Serial);

                        if (player.IsScp)
                            player.CurrentItem = cc;
                        break;
                    case "스테로이드":
                        player.GetEffect(EffectType.MovementBoost).Intensity += 50;
                        Timing.CallDelayed(25, () => { player.GetEffect(EffectType.MovementBoost).Intensity -= 50; });
                        break;
                    case "순교":
                        martyrs.Add(player);
                        break;
                    case "하이패스":
                        RGM.Instance.GodModePlayers.Add(player);
                        Timing.CallDelayed(25, () =>
                        {
                            if (RGM.Instance.GodModePlayers.Contains(player))
                                RGM.Instance.GodModePlayers.Remove(player);
                        });
                        break;
                    case "테러리스트의 유품":
                        player.TryAddCandy(CandyKindID.Pink);

                        if (player.IsScp)
                            Server.ExecuteCommand($"/forceeq {player.Id} 42");
                        break;
                    case "랜덤상자":
                        int rn1 = RGM.GetRandomValue(new List<int> { 11, 16, 18, 24, 31, 32, 47, 48, 49, 50, 51, 52, 53 });

                        if (player.IsInventoryFull)
                            Server.ExecuteCommand($"/drop {player.Id} {rn1} 1");

                        else
                            Server.ExecuteCommand($"/give {player.Id} {rn1}");

                        if (player.IsScp)
                            Server.ExecuteCommand($"/forceeq {player.Id} {rn1}");
                        break;
                    case "럭키비키": PlayerWorkstation[player].Clear(); break;
                    case "핵 리모컨": Warhead.Start(); Server.ExecuteCommand($"/cassie_sl {player.DisplayNickname}(이)가 핵을 <b>원격으로 활성화했습니다.</b>"); break;
                    case "슈퍼 스타": Server.ExecuteCommand($"/speak {player.Id} enable"); break;
                    case "극독": posions.Add(player); break;
                    case "구사일생": ability941s.Add(player); break;
                    case "최후의 발악": culprits.Add(player); break;
                    case "초재생":
                        Item AntiScp207 = player.AddItem(ItemType.AntiSCP207);

                        if (player.IsScp)
                            player.CurrentItem = AntiScp207;
                        break;
                    case "고스트룰": player.GetEffect(EffectType.Ghostly).Intensity += 1; break;
                    case "잠수부": player.IsUsingStamina = false; break;
                    case "스피드왜건": player.GetEffect(EffectType.MovementBoost).Intensity += 100; break;
                    case "모드 설치":
                        Item IM = player.AddItem(ItemType.Coin);
                        InstallModeSerials.Add(IM.Serial);

                        if (player.IsScp)
                            player.CurrentItem = IM;
                        break;
                    case "뱀의 손 무전기":
                        Item SH = player.AddItem(ItemType.Radio);
                        CallSnakeHandsSerials.Add(SH.Serial);

                        if (player.IsScp)
                            player.CurrentItem = SH;
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
                    case "05 평의회": player.AddItem(ItemType.KeycardO5); break;
                    case "관리 의무자":
                        List<ItemType> ManageDuty = new List<ItemType>() 
                        { 
                            ItemType.GunCrossvec, 
                            ItemType.Flashlight, 
                            ItemType.Ammo9x19, 
                            ItemType.Ammo9x19
                        };

                        foreach (var item in ManageDuty)
                            player.AddItem(item);
                        break;
                    case "격리 의무자":
                        List<ItemType> ContainDuty = new List<ItemType>()
                        {
                            ItemType.GrenadeFlash,
                            ItemType.GrenadeHE
                        };

                        foreach (var item in ContainDuty)
                            player.AddItem(item);
                        break;
                    case "카오스 볼":
                        Item c = player.AddItem(ItemType.SCP018);

                        if (player.IsScp)
                            player.CurrentItem = c;
                        break;
                    case "혼돈의 손길":
                        Item Ch = player.AddItem(ItemType.Coin);
                        ChaosCoinSerials.Add(Ch.Serial);

                        if (player.IsScp)
                            player.CurrentItem = Ch;
                        break;
                    case "세치 혀":
                        Item Scp1576 = player.AddItem(ItemType.SCP1576);

                        if (player.IsScp)
                            player.CurrentItem = Scp1576;
                        break;
                    case "공포": 
                        break;
                    case "사자":
                        if (player.Role is Scp049Role scp049)
                            scp049.RemainingAttackCooldown /= 2;
                        break;
                    case "허기":
                        break;
                    case "격노":
                        break;
                    case "회춘":
                        player.GetEffect(EffectType.DamageReduction).Intensity += 40;
                        break;
                    case "흉내쟁이":
                        if (player.Role is Scp939Role scp939)
                            scp939.MimicryCooldown /= 2;
                        break;
                    case "숙련된 암살자":
                        break;
                    case "핑 리모컨":
                        break;
                    case "간이 충전기":
                        if (player.Role is Scp079Role scp079)
                            scp079.Experience += 20;
                        break;
                    case "과전류":
                        for (int i = 1; i < 21; i++)
                        {
                            if (player.Role is Scp079Role scp0791)
                                scp0791.Energy = scp0791.MaxEnergy;

                            await Task.Delay(1000);
                        }
                        break;
                    case "랜덤 함수":
                        break;
                    case "RTX4090":
                        break;
                    case "고대의 존재 압도":
                        break;
                }
            }
            else
            {
                string Message = $"<size=20><b>꽝! 다음 기회에..</b></size>";

                player.AddBroadcast(8, Message);
                player.SendConsoleMessage($"\n{Message}", "white");
            }
        }

        public async void OnTogglingNoClip(Exiled.Events.EventArgs.Player.TogglingNoClipEventArgs ev)
        {
            if (!ev.Player.IsCuffed)
            {
                if (fighters.Contains(ev.Player))
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

                if (PlayerAbilities[ev.Player].Contains("[전용] 소매치기"))
                {
                    if (!PickPocketCooldown.Contains(ev.Player))
                    {
                        if (Physics.Raycast(ev.Player.ReferenceHub.PlayerCameraReference.position + ev.Player.ReferenceHub.PlayerCameraReference.forward * 0.2f, ev.Player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 2f, InventorySystem.Items.Firearms.Modules.StandardHitregBase.HitregMask) &&
                            hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
                        {
                            var player = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>());

                            if (ev.Player != player && !MeleeCooldown.Contains(ev.Player))
                            {
                                if (!player.IsInventoryEmpty)
                                {
                                    Item Item = RGM.GetRandomValue(player.Items.ToList());

                                    player.RemoveItem(Item);
                                    player.ShowHint("주머니가 허전합니다..", 1.2f);
                                    Item I = ev.Player.AddItem(Item.Type);
                                    ev.Player.ShowHint("소매치기에 성공했습니다.", 1.2f);

                                    if (ev.Player.IsScp)
                                        ev.Player.CurrentItem = I;

                                    Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 0.7f);

                                    MeleeCooldown.Add(ev.Player);
                                    await Task.Delay(60 * 1000);
                                    MeleeCooldown.Remove(ev.Player);
                                }
                                else
                                {
                                    player.ShowHint("누군가가 소매치기를 하려고 시도했습니다.", 1.2f);
                                    ev.Player.ShowHint("소매치기에 실패했습니다.\n대상은 아이템을 가지고 있지 않습니다.", 1.2f);
                                }
                            }
                        }
                    }
                }
            }
        }

        public async void OnJumping(Exiled.Events.EventArgs.Player.JumpingEventArgs ev)
        {
            if (Physics.Raycast(ev.Player.Position, Vector3.down, out RaycastHit hit, 5, (LayerMask)1))
            {
                Transform WorkStation = hit.transform.parent.parent;

                if (WorkStation.name.Contains("Work Station") && !PlayerWorkstation[ev.Player].Contains(WorkStation.position))
                {
                    PlayerWorkstation[ev.Player].Add(WorkStation.position);

                    AddAbility(ev.Player);
                }
            }

            if (PlayerAbilities[ev.Player].Contains("[희귀] 블랙아웃"))
            {
                if (!BlackOutCooldown.Contains(ev.Player))
                {
                    BlackOutCooldown.Add(ev.Player);
                    ev.Player.CurrentRoom.TurnOffLights(3);
                    await Task.Delay(20000);
                    BlackOutCooldown.Remove(ev.Player);
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

                    ev.Player.ShowHint("이 동전을 튕기면 <b><color=#A4A4A4>뽑기</color></b> 능력을 사용할 수 있습니다.", 1.2f);

                    await Task.Delay(1000);
                }
            }
            else if (FollowCoinSerials.Contains(ev.Item.Serial))
            {
                while (true)
                {
                    if (ev.Player.CurrentItem == null || !FollowCoinSerials.Contains(ev.Player.CurrentItem.Serial))
                        break;

                    ev.Player.ShowHint("이 동전을 튕기면 <b><color=#58D3F7>순간이동</color></b> 능력을 사용할 수 있습니다.", 1.2f);

                    await Task.Delay(1000);
                }
            }
            else if (GrapCoinSerials.Contains(ev.Item.Serial))
            {
                while (true)
                {
                    if (ev.Player.CurrentItem == null || !GrapCoinSerials.Contains(ev.Player.CurrentItem.Serial))
                        break;

                    ev.Player.ShowHint("이 동전을 튕기면 <b><color=#58D3F7>갈고리</color></b> 능력을 사용할 수 있습니다.", 1.2f);

                    await Task.Delay(1000);
                }
            }
            else if (ClockCoinSerials.Contains(ev.Item.Serial))
            {
                while (true)
                {
                    if (ev.Player.CurrentItem == null || !ClockCoinSerials.Contains(ev.Player.CurrentItem.Serial))
                        break;

                    ev.Player.ShowHint("이 동전을 튕기면 <b><color=#58D3F7>회중시계</color></color></b> 능력을 사용할 수 있습니다.", 1.2f);

                    await Task.Delay(1000);
                }
            }
            else if (InstallModeSerials.Contains(ev.Item.Serial))
            {
                while (true)
                {
                    if (ev.Player.CurrentItem == null || !InstallModeSerials.Contains(ev.Player.CurrentItem.Serial))
                        break;

                    ev.Player.ShowHint("이 동전을 튕기면 <b><color=yellow>모드 설치</color></b> 능력을 사용할 수 있습니다.", 1.2f);

                    await Task.Delay(1000);
                }
            }
            else if (FlashLightSerials.Contains(ev.Item.Serial))
            {
                while (true)
                {
                    if (ev.Player.CurrentItem == null || !FlashLightSerials.Contains(ev.Player.CurrentItem.Serial))
                        break;

                    ev.Player.ShowHint("손전등을 상대에게 비추면 <b><color=yellow>플래시라이트</color></b> 능력을 사용할 수 있습니다.", 1.2f);

                    await Task.Delay(1000);
                }
            }
            else if (ChaosCoinSerials.Contains(ev.Item.Serial))
            {
                while (true)
                {
                    if (ev.Player.CurrentItem == null || !ChaosCoinSerials.Contains(ev.Player.CurrentItem.Serial))
                        break;

                    ev.Player.ShowHint("이 동전을 튕기면 <b><color=#F7819F>혼돈의 손길</color></color></b> 능력을 사용할 수 있습니다.", 1.2f);

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

                Player target = RGM.GetRandomValue(Player.List.Where(x => x != ev.Player && x.IsAlive && x.Role.Type != RoleTypeId.Scp079).ToList());
                ev.Player.Position = target.Position;
            }
            else if (GrapCoinSerials.Contains(ev.Item.Serial))
            {
                ev.Item.Destroy();

                Player target1 = RGM.GetRandomValue(Player.List.Where(x => x.IsAlive && x != ev.Player && x.Role.Type != RoleTypeId.Scp079).ToList());
                target1.Position = ev.Player.Position;
            }
            else if (ClockCoinSerials.Contains(ev.Item.Serial))
            {
                ev.Item.Destroy();

                ev.Player.EnableEffect(EffectType.Ensnared, 1, 3);

                RGM.Instance.GodModePlayers.Add(ev.Player);

                await Task.Delay(3000);

                if (RGM.Instance.GodModePlayers.Contains(ev.Player))
                    RGM.Instance.GodModePlayers.Remove(ev.Player);
            }
            else if (InstallModeSerials.Contains(ev.Item.Serial))
            {
                ev.Item.Destroy();

                string Mode1 = RGM.GetRandomValue(RGM.Instance.ModeList.Keys.Where(x => RGM.Instance.ModeList[x][3] != "private").ToList());
                string mod1 = Mode1.ToString();

                var modeType = Type.GetType($"RGM.Modes.{RGM.Instance.ModeList[Mode1][2]}");
                if (modeType != null)
                {
                    var modeInstance = Activator.CreateInstance(modeType);
                    var onEnabledMethod = modeType.GetMethod("OnEnabled");
                    onEnabledMethod?.Invoke(modeInstance, null);
                }

                Server.ExecuteCommand($"/cassie_sl {ev.Player.DisplayNickname}(이)가 [{mod1}] 모드를 설치했습니다.");
            }
            else if (ChaosCoinSerials.Contains(ev.Item.Serial))
            {
                ev.Item.Destroy();

                if (PlayerAbilities.ContainsKey(ev.Player))
                {
                    PlayerAbilities[ev.Player].Clear();
                    PlayerWorkstation[ev.Player].Clear();
                }
            }
        }

        public async void OnTogglingRadio(Exiled.Events.EventArgs.Player.TogglingRadioEventArgs ev)
        {
            if (CallSnakeHandsSerials.Contains(ev.Item.Serial))
            {
                ev.Item.Destroy();

                ev.Player.Role.Set(RoleTypeId.Tutorial, SpawnReason.ForceClass, RoleSpawnFlags.None);

                List<Player> SnakeHands = Player.List.Where(x => x.IsDead).ToList();

                List<ItemType> Items = new List<ItemType>
                {
                    ItemType.KeycardFacilityManager,
                    ItemType.GunFSP9, ItemType.GunRevolver,
                    ItemType.Adrenaline,
                    ItemType.AntiSCP207
                };

                List<ItemType> Ammos = new List<ItemType> 
                {
                    ItemType.Ammo44cal,
                    ItemType.Ammo762x39
                };

                foreach (var p in SnakeHands)
                {
                    p.Role.Set(RoleTypeId.Tutorial);
                    p.Position = new Vector3(-0.08203125f, 1000.96f, 6.828125f);

                    foreach (ItemType Item in Items)
                        p.AddItem(Item);

                    for (int i = 1; i < 3; i++)
                    {
                        foreach (var Ammo in Ammos)
                            p.AddItem(Ammo);
                    }
                }

                for (int i = 1; i < 6; i++)
                {
                    ev.Player.ShowHint($"<i>{SnakeHands.Count()}명의 <color=#FE2EF7>동료</color>들이 당신과 함께합니다..</i>", 1.2f);

                    await Task.Delay(1000);
                }
            }
        }

        public async void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            if (PlayerWorkstation.ContainsKey(ev.Player))
            {
                if (ev.Player.Role.Type == RoleTypeId.Scp079)
                {
                    if (PlayerAbilities[ev.Player].Contains("[전용] RTX4090"))
                    {
                        PlayerAbilities[ev.Player].Clear();

                        ev.Player.Role.Set(RoleTypeId.Tutorial);

                        Vector3 Pos = Door.Get(DoorType.Scp079First).Position;
                        ev.Player.Position = new Vector3(Pos.x, Pos.y + 2, Pos.z);

                        for (int i = 1; i < UnityEngine.Random.Range(7, 12); i++)
                            AddAbility(ev.Player);
                    }
                }
                else
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

                        ev.Player.EnableEffect(EffectType.Blinded, 1, 3);
                        ev.Player.EnableEffect(EffectType.Invisible, 1, 3);
                        ev.Player.GetEffect(EffectType.MovementBoost).Intensity += 20;
                        RGM.Instance.GodModePlayers.Add(ev.Player);

                        await Task.Delay(3000);

                        ev.Player.GetEffect(EffectType.MovementBoost).Intensity -= 20;
                        if (RGM.Instance.GodModePlayers.Contains(ev.Player))
                            RGM.Instance.GodModePlayers.Remove(ev.Player);
                        return;
                    }

                    if (culprits.Contains(ev.Player))
                    {
                        culprits.Remove(ev.Player);
                        ev.IsAllowed = false;

                        ev.Player.GetEffect(EffectType.MovementBoost).Intensity += 30;
                        RGM.Instance.GodModePlayers.Add(ev.Player);

                        Timing.CallDelayed(5f, () =>
                        {
                            if (RGM.Instance.GodModePlayers.Contains(ev.Player))
                                RGM.Instance.GodModePlayers.Remove(ev.Player);
                            ev.Player.Kill("최후의 발악의 효과로 사망하였습니다.");
                        });
                        return;
                    }

                    PlayerWorkstation[ev.Player].Clear();
                    PlayerAbilities[ev.Player].Clear();
                    ev.Player.Scale = new Vector3(1, 1, 1);
                    Server.ExecuteCommand($"/speak {ev.Player.Id} disable");
                    ev.Player.IsUsingStamina = true;
                    if (RGM.Instance.GodModePlayers.Contains(ev.Player))
                        RGM.Instance.GodModePlayers.Remove(ev.Player);

                    if (fighters.Contains(ev.Player))
                        fighters.Remove(ev.Player);

                    if (martyrs.Contains(ev.Player))
                    {
                        martyrs.Remove(ev.Player);

                        var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, ev.Player);
                        g.FuseTime = 3f;
                        g.SpawnActive(ev.Player.Position, ev.Player);
                    }

                    if (BlackOutCooldown.Contains(ev.Player))
                        BlackOutCooldown.Remove(ev.Player);

                    if (spirits.Contains(ev.Player))
                        spirits.Remove(ev.Player);

                    if (repairs.Contains(ev.Player))
                        repairs.Remove(ev.Player);

                    if (magicians.Contains(ev.Player))
                    {
                        magicians.Remove(ev.Player);

                        ev.Player.Role.Set(ev.Attacker.Role, SpawnReason.ForceClass, RoleSpawnFlags.None);
                        ev.Player.Health = ev.Attacker.Health;
                        foreach (Item Item in ev.Attacker.Items)
                            ev.Player.AddItem(Item.Type);

                        ev.Attacker.Kill($"몸이 교체되는 마술에 당했네요!");
                    }

                    if (posions.Contains(ev.Player))
                    {
                        posions.Remove(ev.Player);

                        ev.Attacker.EnableEffect(EffectType.CardiacArrest, 1, 15);
                    }
                }
            }
        }

        public void OnDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            if (PlayerAbilities.ContainsKey(ev.Attacker))
            {
                List<string> aa = PlayerAbilities[ev.Attacker];

                if (aa.Contains("[전설] 킬스트릭"))
                    AddAbility(ev.Attacker);

                if (aa.Contains("[전용] 공포"))
                {
                    foreach (var player in ev.Attacker.CurrentRoom.Players.Where(x => !x.IsScp))
                        player.EnableEffect(EffectType.Ensnared, 1, 0.5f);
                }
            }
        }

        public void OnInteractingDoor(Exiled.Events.EventArgs.Player.InteractingDoorEventArgs ev)
        {
            if (ev.Door.Type == DoorType.Scp079First)
            {
                ev.Player.ShowHint("이 헤비도어는 능력으로 개폐가 불가능합니다.", 1.2f);
                return;
            }
            else if (ev.IsAllowed)
                return;

            else if (ev.Player != null && ((PlayerAbilities[ev.Player].Contains("[일반] 행운") && UnityEngine.Random.Range(1, 101) <= 5) || PlayerAbilities[ev.Player].Contains("[영웅] 수리 기사")))
            {
                if (ev.Door.IsOpen)
                    ev.Door.IsOpen = false;

                else
                    ev.Door.IsOpen = true;
            }
        }

        public void OnDroppedItem(Exiled.Events.EventArgs.Player.DroppedItemEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[영웅] 도박꾼"))
            {
                if (UnityEngine.Random.Range(0, 100) <= 5)
                    ev.Player.EnableEffect(EffectType.SeveredHands);

                else
                {
                    ev.Pickup.Destroy();
                    Server.ExecuteCommand($"/drop {ev.Player.Id} {UnityEngine.Random.Range(0, 55)} 1");
                }
            }
        }

        public void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            if (ev.Attacker != null)
            {
                if (PlayerAbilities[ev.Player].Contains("[일반] 단련"))
                {
                    int count = PlayerAbilities.Values.Count(list => list.Contains("[일반] 단련"));

                    ev.DamageHandler.Damage = (int)(ev.DamageHandler.Damage * (1 + (0.2 * count)));
                }

                if (PlayerAbilities[ev.Attacker].Contains("[희귀] 흡혈귀"))
                    ev.Attacker.AddAhp(20 * (ev.DamageHandler.Damage / 100));

                if (PlayerAbilities[ev.Attacker].Contains("[신화] 로켓 런처") && ev.Attacker.LeadingTeam != ev.Player.LeadingTeam)
                {
                    if (UnityEngine.Random.Range(1, 21) == 1)
                        Server.ExecuteCommand($"/rocket {ev.Player.Id} 1");
                }

                if (PlayerAbilities[ev.Player].Contains("[전용] 격노"))
                {
                    if (ev.Player.Role is Scp096Role Scp096)
                    {
                        if (Scp096.RageManager.IsEnraged && ev.Attacker != null && ev.Attacker != ev.Player)
                            ev.IsAllowed = false;
                    }
                }

                if (PlayerAbilities[ev.Player].Contains("[전용] 숙련된 암살자"))
                {
                    if (ev.DamageHandler.Type == DamageType.Strangled)
                        ev.DamageHandler.Damage *= 10;
                }
            }
        }

        public void OnHurt(Exiled.Events.EventArgs.Player.HurtEventArgs ev)
        {
            if (ev.Attacker != null && PlayerAbilities[ev.Attacker].Contains("[신화] 스피릿"))
                ev.Attacker.DisableEffect(EffectType.Invisible);

            if (PlayerAbilities[ev.Player].Contains("[신화] 스피릿"))
                ev.Player.DisableEffect(EffectType.Invisible);
        }

        public void OnTriggeringTesla(Exiled.Events.EventArgs.Player.TriggeringTeslaEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[영웅] 수리 기사"))
                ev.DisableTesla = true;
        }

        public void OnConsumedCorpse(Exiled.Events.EventArgs.Scp0492.ConsumedCorpseEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[전용] 허기"))
                ev.ConsumeHeal *= 2;
        }

        public void OnPinging(Exiled.Events.EventArgs.Scp079.PingingEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[전용] 핑 리모컨"))
                ev.Room.TurnOffLights(0.5f);
        }

        public void OnZoneBlackout(Exiled.Events.EventArgs.Scp079.ZoneBlackoutEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[전용] 랜덤 함수"))
            {
                for (int i = 1; i < 6; i++)
                {
                    Room SelectedRoom = RGM.GetRandomValue(Room.List.ToList());

                    SelectedRoom.TurnOffLights(10);
                }
            }
        }

        public void OnChangingSpeakerStatus(Exiled.Events.EventArgs.Scp079.ChangingSpeakerStatusEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[전용] 고대의 존재 압도"))
            {
                foreach (var player in ev.Scp079.Camera.Room.Players)
                    player.EnableEffect(EffectType.SinkHole, 1, 1);
            }
        }
    }
}
