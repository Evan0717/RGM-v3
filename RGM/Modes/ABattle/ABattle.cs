using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using InventorySystem.Items.Usables.Scp330;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features.Serializable;
using MapEditorReborn.API.Features;
using MEC;
using MultiBroadcast.API;
using PlayerRoles;
using RGM.API;
using UnityEngine;
using HarmonyLib;
using Utils.NonAllocLINQ;
using System.Threading;
using Exiled.API.Extensions;
using InventorySystem.Items.MicroHID;
using PluginAPI.Events;
using System.Diagnostics.Tracing;

namespace RGM.Modes
{
    class ABattle
    {
        public static ABattle Instance;

        public bool IsFeverModeEnabled = false;
        public Dictionary<Player, List<Vector3>> PlayerWorkstation = new Dictionary<Player, List<Vector3>>();
        public Dictionary<Player, List<string>> PlayerAbilities = new Dictionary<Player, List<string>>();
        public Dictionary<Player, string> PlayerVotes = new Dictionary<Player, string>();

        public List<Player> MeleeCooldown = new List<Player>();
        public List<Player> RoaringSoundCooldown = new List<Player>();
        public List<Player> TeleportCooldown = new List<Player>();
        public List<Player> PickPocketCooldown = new List<Player>();

        public List<ushort> PickCoinSerials = new List<ushort>();
        public List<ushort> EscapeCoinSerials = new List<ushort>();
        public List<ushort> EarthquakeCoinSerials = new List<ushort>();
        public List<ushort> FollowCoinSerials = new List<ushort>();
        public List<ushort> GrapCoinSerials = new List<ushort>();
        public List<ushort> ClockCoinSerials = new List<ushort>();
        public List<ushort> ContractCoinSerials = new List<ushort>();
        public List<ushort> CallSnakeHandsSerials = new List<ushort>();
        public List<ushort> FlashLightSerials = new List<ushort>();
        public List<ushort> FlamethrowerSerials = new List<ushort>();
        public List<ushort> RadarSerials = new List<ushort>();
        public List<ushort> ChaosCoinSerials = new List<ushort>();

        ReferenceHub dj;

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
            {"[일반] 뽑기", "지급된 동전을 튕기면 10% 확률로 새로운 능력을 3개 더 얻습니다."},
            {"[일반] 보험", "사망 판정을 받을 경우 1번 버텨냅니다." },
            {"[일반] 회축", "[ALT]를 눌러 발차기 공격을 가할 수 있습니다. (쿨타임 1초)"},
            {"[일반] 보급", "탄약이 랜덤하게 지급됩니다."},
            {"[일반] 정화", "초록 사탕을 받습니다."},
            {"[일반] 무기 전문가", "SCP-1853을 받습니다."},
            {"[일반] 신내림", "당신을 지켜보는 관전자가 5초 이내로 나타나면 능력 2~3개를 추가로 얻습니다."},
            {"[일반] 횃불", "랜턴과 노란 사탕을 받습니다."},
            {"[일반] 잠행", "발걸음 소리가 줄어듭니다."},
            {"[일반] 위기 탈출", "넘버원! 지급된 동전을 튕기면 대상을 잠시 동안 멈추게 만듭니다."},
            {"[일반] 저체중", "점프 속도가 20% 증가합니다."},
            {"[일반] 지진", "지급된 동전을 튕기면 시설 내 모두에게 화면 흔들림 효과를 일시적으로 부여합니다."},
            {"[일반] 우애", "자신이 가진 아이템 중 하나를 복사하여 근처에 있는 플레이어에게 지급합니다."},
            {"[일반] 무지개", "무지개 사탕을 받습니다."},
            {"[일반] 바디백", "몸통 데미지 경감 효과가 3% 추가됩니다."}
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
            {"[희귀] 하이패스", "25초 간 무적이 됩니다."},
            {"[희귀] 트리플악셀", "여분의 탄약과 함께 최대 탄약이 1/2인 COM-45를 지급받습니다."},
            {"[희귀] 연금", "3분 간, 1분마다 아이템을 하나 획득합니다."},
            {"[희귀] 계약", "지급된 동전을 튕기면 당장 죽지만, 다음 생에 능력 3개를 가진 채로 시작합니다."},
            {"[희귀] 반창고", "최초 1회, 체력이 절반 이하로 줄어들었을 경우 즉시 회복합니다."}
        };
        public Dictionary<string, string> EpicAbilities = new Dictionary<string, string>()
        {
            {"[영웅] 테러리스트의 유품", "핑크 사탕을 지급받습니다."},
            {"[영웅] 도박꾼", "아이템을 버리면 새로운 아이템을 받지만, 5% 확률로 손이 잘립니다."},
            {"[영웅] 랜덤상자", "랜덤하지만 좋은 아이템을 2개 지급받습니다."},
            {"[영웅] 수리 기사", "모든 잠겨진 문에 액세스할 수 있으며, 테슬라를 작동시키지 않습니다. (중첩 불가)"},
            {"[영웅] 슈퍼 스타", "자신의 마이크가 모두에게 공유되고, 사망 시 사망한 사실이 모두에게 공개됩니다. (중첩 불가)"},
            {"[영웅] 럭키비키", "이전에 방문했던 워크스테이션에서 다시 한번 더 능력을 획득할 수 있습니다."},
            {"[영웅] 극독", "누군가에게 죽으면 죽인 자에게 심장 마비 효과를 겁니다. (중첩 불가)"},
            {"[영웅] 구사일생", "사망 판정을 받을 경우 1번 버텨내며 3초간 무적이 됩니다."},
            {"[영웅] 최후의 발악", "5초 뒤 반드시 죽지만, 그동안 무적이 되며 속도가 매우 빨라집니다."},
            {"[영웅] 초재생", "핑크 콜라를 지급받습니다."},
            {"[영웅] 고스트룰", "문을 통과할 수 있습니다. (중첩 불가)"},
            {"[영웅] 잠수부", "시야가 개선되고 스테미나가 줄어들지 않습니다. (중첩 불가)"},
            {"[영웅] 점멸", "점프할 때마다 근처 문으로 텔레포트합니다. (쿨타임 15초)"},
        };
        public Dictionary<string, string> LegendAbilities = new Dictionary<string, string>()
        {
            {"[전설] 스피드왜건", "속도가 크게 증가합니다."},
            {"[전설] 뱀의 손 무전기", "무전기를 든 상태로 우클릭하면 뱀의 손 지원을 부르며, 자신도 뱀의 손 소속이 됩니다."},
            {"[전설] 랜덤택배", "서버 인원 수 만큼 랜덤한 아이템을 드롭합니다."},
            {"[전설] 마술사", "누군가에게 죽으면 죽인 자와 교체됩니다. (중첩 불가)"},
            {"[전설] 플래시라이트", "지급된 손전등을 들고 상대를 쳐다보면 눈뽕 공격을 가할 수 있습니다."},
            {"[전설] 킬스트릭", "누군가를 죽일 때마다 새로운 능력을 얻습니다. (중첩 불가)"},
            {"[전설] 화염 방사기", "위력은 20%로 낮아지지만, 상대를 불태우고 자동으로 충전되는 화염 방사기를 받습니다."},
            {"[전설] 영매", "당신을 바라보는 관전자 수에 비례하여 능력치가 상승합니다."},
            {"[전설] 괴성", "적을 보고 있을 때 마이크를 키면 시설 내의 적들을 일시적으로 둔해지게 만듭니다. (쿨타임 100초)"}
        };
        public Dictionary<string, string> MythicAbilities = new Dictionary<string, string>()
        {
            {"[신화] 로켓 런처", "5% 확률로 상대방을 하늘로 승천시킬 수 있습니다! (중첩 불가)"},
            {"[신화] 스피릿", "2초마다 영혼 상태가 됩니다! (중첩 불가)"},
            {"[신화] 눈빛맨", "상대는 눈에 띄거나 근처에 있는 것만으로도 압도당할 것입니다! (중첩 불가)"},
            {"[신화] 차원 강탈자", "죽인 누군가의 능력을 모조리 흡수합니다! (중첩 불가)"},
            {"[신화] 조커", "현실을 부정하라고~? (중첩 불가)"}
        };
        public Dictionary<string, string> ClassDAbilities = new Dictionary<string, string>()
        {
            {"[전용] 절도죄", "[ALT]를 눌러 상대의 아이템 중 하나를 빼앗을 수 있습니다. (쿨타임 1분) (중첩 불가)"},
            {"[전용] 주거칩입죄", "SCP-268을 지급받습니다."},
            {"[전용] 반란의 씨앗", "카오스 티켓을 전체 인원 수의 20% 만큼 추가합니다."}
        };
        public Dictionary<string, string> ScientistAbilities = new Dictionary<string, string>()
        {
            {"[전용] 05 평의회", "05등급 키카드를 지급받습니다."},
            {"[전용] 공학 전공", "SCP-2176을 지급받습니다."},
            {"[전용] 특무부대의 씨앗", "NTF 티켓을 진체 인원 수의 20% 만큼 추가합니다."}
        };
        public Dictionary<string, string> GuardAbilities = new Dictionary<string, string>()
        {
            {"[전용] 관리 의무자", "손전등과 Crossvec를 지급받습니다."},
            {"[전용] 보건소 직원", "주변에 있는 아군들에게 의료 아이템을 랜덤하게 지급합니다."},
            {"[전용] 산업재해보험", "보험 능력을 3개 얻습니다."}
        };
        public Dictionary<string, string> NtfAbilities = new Dictionary<string, string>()
        {
            {"[전용] 격리 의무자", "고폭 수류탄과 섬광탄을 지급받습니다."},
            {"[전용] 의무병", "주변에 있는 아군들을 매 초마다 0.5HP씩 치료합니다."},
            {"[전용] 집단 지성", "주변에 있는 아군 한명당 데미지가 10% 증가합니다."},
            {"[전용] 레이더", "지급된 무전기를 들면 가장 가까운 유기체와의 거리를 확인할 수 있습니다."}
        };
        public Dictionary<string, string> ChaosAbilities = new Dictionary<string, string>()
        {
            {"[전용] 혼돈의 카오스", "SCP-018을 지급받습니다."},
            {"[전용] 혼돈의 손길", "지급된 동전을 튕기면 보유한 능력을 전부 삭제합니다."},
            {"[전용] 혼돈의 가방", "아이템 인벤토리가 전부 채워질 때까지 아이템을 받습니다."}
        };
        public Dictionary<string, string> SnakeAbilities = new Dictionary<string, string>()
        {
            {"[전용] 세치 혀", "SCP-1576을 지급받습니다."},
            {"[전용] 제3세력", "뱀의 손 지원을 2명 더 부릅니다."},
            {"[전용] SCP 연구자", "SCP 아이템 중 하나를 지급받습니다."}
        };
        public Dictionary<string, string> Scp173Abilities = new Dictionary<string, string>()
        {
            {"[전용] 공포", "인간을 죽이면 근처에 있는 인간들이 10.75초 동안 움직일 수 없게 됩니다."},
            {"[전용] 괴이", "순간이동한 방이 0.5초 동안 정전됩니다."},
            {"[전용] 신기루", "데미지를 입을 때 5% 확률로 1.25초 동안 투명화가 됩니다."}
        };
        public Dictionary<string, string> Scp049Abilities = new Dictionary<string, string>()
        {
            {"[전용] 사자", "공격 속도가 1/2배가 됩니다."},
            {"[전용] 유능한 의사", "소생된 좀비의 체력이 50% 확률로 1.5배가 됩니다. (중첩 불가)"},
            {"[전용] 능수능란", "좀비 버프 능력 쿨타임이 1/2배가 됩니다."}
        };
        public Dictionary<string, string> Scp0492Abilities = new Dictionary<string, string>()
        {
            {"[전용] 허기", "인간을 섭취할 때 얻는 회복량이 2배가 됩니다. (중첩 불가)"},
            {"[전용] 급식", "최대 체력이 1.5배 증가합니다."},
            {"[전용] 당혹감", "목표물로 삼은 인간은 실명합니다. (중첩 불가)"}
        };
        public Dictionary<string, string> Scp096Abilities = new Dictionary<string, string>()
        {
            {"[전용] 격노", "분노 때에는 데미지를 50%만 받습니다. (중첩 불가)"},
            {"[전용] 별자리 찢기", "25% 확률로 공격한 대상을 즉사시킵니다. (중첩 불가)"},
            {"[전용] 천리안", "분노 시에 30m 내의 인간들을 모두 목격자에 포함시킵니다. (중첩 불가)"},
            {"[전용] 원수", "분노 충전 시간이 1/2배가 됩니다."}
        };
        public Dictionary<string, string> Scp106Abilities = new Dictionary<string, string>()
        {
            {"[전용] 회춘", "데미지를 20% 경감시키는 효과를 받습니다."},
            {"[전용] 끈적한 늪", "5m 내의 인간들을 느리게 만듭니다. (중첩 불가)"},
            {"[전용] 사냥감 모색", "공격 성공 후 속도가 일시적으로 증가합니다."}
        };
        public Dictionary<string, string> Scp939Abilities = new Dictionary<string, string>()
        {
            {"[전용] 흉내쟁이", "흉내 쿨타임이 사라집니다. (중첩 불가)"},
            {"[전용] 안아줘요", "안개 쿨타임이 1/2배가 됩니다."},
            {"[전용] 민첩한 사냥 도구", "공격 속도가 1/2배가 됩니다."}
        };
        public Dictionary<string, string> Scp3114Abilities = new Dictionary<string, string>()
        {
            {"[전용] 숙련된 암살자", "교살 데미지가 10배 증가합니다. (중첩 불가)"},
            {"[전용] 반블럭", "데미지를 입으면 이동 속도가 증가합니다."},
            {"[전용] 도라에몽 주머니", "변신을 해제할 때마다 아이템을 하나 지급받습니다. (중첩 불가)"}
        };
        public Dictionary<string, string> Scp079Abilities = new Dictionary<string, string>() 
        {
            {"[전용] 핑 리모컨", "핑을 찍은 장소에 0.5초 간 정전이 됩니다."},
            {"[전용] 간이 충전기", "즉시 20 경험치를 받습니다."},
            {"[전용] 과전류", "20초 간 전력이 무제한이 됩니다."},
            {"[전용] 랜덤 함수", "정전하면 랜덤한 방 5개를 더 정전합니다."},
            {"[전용] RTX4090", "격리당하면 튜토리얼(능력 5~10개)로 부활합니다."},
            {"[전용] 고대의 존재 압도", "V키를 연타하면 해당 방에 있는 인간의 속도가 감소합니다."}
        };

        public Dictionary<string, List<string>> Synergies = new Dictionary<string, List<string>>()
        {
            {"[시너지] 생존 전문가", new List<string>()
                {
                    "<보험, 구사일생, 최후의 발악> 즉시 현재 체력의 500%에 해당하는 최대 체력을 얻습니다.",
                    "[일반] 보험", "[영웅] 구사일생", "[영웅] 최후의 발악"
                }
            },
            {"[시너지] 시공간 초월자", new List<string>()
                {
                    "<순간이동, 갈고리, 고스트룰, 차원 강탈자> 시공간을 초월했습니다! 대단합니다!",
                    "[희귀] 순간이동", "[희귀] 갈고리", "[영웅] 고스트룰", "[신화] 차원 강탈자"
                }
            },
            {"[시너지] 광휘", new List<string>()
                {
                    "<플래시라이트, 횃불> 당신을 쳐다보는 눈은 멀어버릴 것입니다.",
                    "[전설] 플래시라이트", "[일반] 횃불"
                }
            },
            {"[시너지] 타고난 사냥꾼", new List<string>()
                {
                    "<매의 눈, 잠행, 무기 전문가, 보급, 운동> 사냥꾼의 기운을 타고났습니다!",
                    "[희귀] 매의 눈", "[일반] 잠행", "[일반] 무기 전문가", "[일반] 보급", "[일반] 운동"
                }
            },
            {"[시너지] 랜덤 컬렉션", new List<string>()
                {
                    "<랜덤박스, 랜덤상자, 랜덤택배> 3+1! 셋 중 하나를 더 받으세요!",
                    "[일반] 랜덤박스", "[영웅] 랜덤상자", "[전설] 랜덤택배"
                }
            },
            {"[시너지] 4대 운동", new List<string>()
                {
                    "<운동, 진화, 경공, 단련> 4대 운동을 모두 마쳤습니다! 능력을 하나 더 획득할 시간입니다.",
                    "[일반] 운동", "[일반] 진화", "[일반] 경공", "[일반] 단련"
                }
            },
            {"[시너지] 중복 기연", new List<string>()
                {
                    "<능력 선택지가 모두 중복일 경우> 당신은 해당 능력과 인연을 맺은 것 같습니다. 3개 전부 다 드릴게요!",
                    "[개발자] GoldenPig1205"
                }
            }
        };

        public Dictionary<string, string> RatingColor = new Dictionary<string, string>()
        {
            {"일반", "#A4A4A4"},
            {"희귀", "#2ECCFA"},
            {"영웅", "#FF00FF"},
            {"전설", "#ffd700"},
            {"신화", "#DF0101"},
            {"전용", "#F7819F"},
            {"시너지", "#DEEFED"}
        };

        public string ColorFormat(string text)
        {
            return text.Replace("[시너지]", $"<color={RatingColor["시너지"]}>[시너지]</color>")
                        .Replace("[전용]", $"<color={RatingColor["전용"]}>[전용]</color>")
                        .Replace("[신화]", $"<color={RatingColor["신화"]}>[신화]</color>")
                        .Replace("[전설]", $"<color={RatingColor["전설"]}>[전설]</color>")
                        .Replace("[영웅]", $"<color={RatingColor["영웅"]}>[영웅]</color>")
                        .Replace("[희귀]", $"<color={RatingColor["희귀"]}>[희귀]</color>")
                        .Replace("[일반]", $"<color={RatingColor["일반"]}>[일반]</color>");
        }

        public int DuplicateCount(Player player, string abilityName)
        {
            return PlayerAbilities[player].Count(a => a == abilityName);
        }

        public void CallSnakeHand(Player Convener, List<Player> PlayerList)
        {
            List<Player> SnakeHands = PlayerList;

            List<ItemType> Items = new List<ItemType>
                {
                    ItemType.KeycardFacilityManager,
                    ItemType.GunFSP9,
                    ItemType.GunRevolver,
                    ItemType.Adrenaline,
                    ItemType.AntiSCP207
                };

            List<ItemType> Ammos = new List<ItemType>
                {
                    ItemType.Ammo44cal,
                    ItemType.Ammo9x19
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

            if (Convener != null)
                Convener.ShowHint($"<i>{SnakeHands.Count()}명의 <color=#FE2EF7>동료</color>들이 당신과 함께합니다..</i>", 5f);
        }

        public string PickAbilityGrade(Player player, string force = null)
        {
            int grade = UnityEngine.Random.Range(1, 10001);
            string abilityGrade;
            if (force != null)
                abilityGrade = "[" + force.Substring(force.IndexOf('[') + 1, force.IndexOf(']') - force.IndexOf('[') - 1) + "]".Trim();

            else if (player.Role.Type == RoleTypeId.Scp079)
                abilityGrade = "[전용]";

            else
            {
                if (grade <= 5) // 0.05%
                    abilityGrade = "[신화]";

                else if (grade <= 40) // 0.35%
                    abilityGrade = "[전설]";

                else if (grade <= 250) // 2.1%
                    abilityGrade = "[영웅]";

                else if (grade <= 1500) // 12.5%
                    abilityGrade = "[희귀]";

                else if (grade <= 2000) // 5%
                    abilityGrade = "[전용]";

                else // 80%
                    abilityGrade = "[일반]";
            }

            return abilityGrade;
        }

        public Dictionary<string, string> AbilityList(Player player, string abilityGrade, bool get = true)
        {
            if (abilityGrade == "[일반]")
                return CommonAbilities;

            else if (abilityGrade == "[희귀]")
                return RareAbilities;

            else if (abilityGrade == "[영웅]")
            {
                if (get) 
                {
                    Cassie.Clear(); 
                    Server.ExecuteCommand($"/cassie_sl {player.DisplayNickname}(이)가 <color={RatingColor["영웅"]}>[영웅]</color> 업그레이드를 입수하였습니다.");
                }
                return EpicAbilities;
            }
            else if (abilityGrade == "[전설]")
            {
                if (get)
                {
                    Cassie.Clear(); 
                    Server.ExecuteCommand($"/cassie_sl {player.DisplayNickname}(이)가 <color={RatingColor["전설"]}>[전설]</color> 업그레이드를 입수하였습니다.");
                }
                return LegendAbilities;
            }
            else if (abilityGrade == "[신화]")
            {
                if (get)
                {
                    Cassie.Clear(); 
                    Server.ExecuteCommand($"/cassie_sl {player.DisplayNickname}(이)가 <color={RatingColor["신화"]}>[신화]</color> 업그레이드를 입수하였습니다.");
                }
                return MythicAbilities;
            }
            else if (abilityGrade == "[전용]")
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

                // Server.ExecuteCommand($"/cassie_sl {player.DisplayNickname}(이)가 <color={RatingColor["전용"]}>[전용]</color> 업그레이드를 입수하였습니다.");
                return Format();
            }
            else
            {
                Dictionary<string, string> SynergiesFormat = new Dictionary<string, string>();

                foreach (var kvp in Synergies)
                {
                    if (kvp.Value.Count > 0)
                        SynergiesFormat.Add(kvp.Key, kvp.Value[0]);
                }

                Cassie.Clear();
                Server.ExecuteCommand($"/cassie_sl {player.DisplayNickname}(이)가 <color={RatingColor["시너지"]}>[시너지]</color> 효과를 입수하였습니다.");
                return SynergiesFormat;
            }
        }

        public void ApplyGiveAbility(Player player, string abilityGrade, string abilityName)
        {
            PlayerAbilities[player].Add(abilityName);
            string styleName = ColorFormat(abilityName);

            string Message = $"<size=15><b>다음 능력이 추가되었습니다.</b></size>\n<size=25>{styleName}</size>\n<size=20>{AbilityList(player, abilityGrade)[abilityName]}</size>";
            player.AddBroadcast(8, Message);
            player.SendConsoleMessage($"\n{Message}", "white");
        }

        public async void AddAbilityVote(Player player)
        {
            List<string> AbilitesVote = new List<string>();
            List<string> DisplayVote = new List<string>();
            int SelectedAbilityNumber = 0;

            for (int i = 1; i < 4; i++)
            {
                List<string> abilityList = AbilityList(player, PickAbilityGrade(player), false).Keys.ToList();

                AbilitesVote.Add(Tools.GetRandomValue(abilityList));
            }

            for (int i = 1; i < 4; i++)
                DisplayVote.Add($"[{i}] {ColorFormat(AbilitesVote[i - 1])}");

            for (int i = 1; i < 21; i++)
            {
                if (player.IsDead)
                    return;

                player.ShowHint($"<align=left><size=30>{string.Join("\n", DisplayVote)}</size>\n\n<size=25><b>{21 - i}초 안에 [.(번호)] 명령어로 원하는 능력을 선택하세요. (ex .1)</b></size></align>\n\n", 1.2f);

                if (PlayerVotes.ContainsKey(player))
                {
                    SelectedAbilityNumber = int.Parse(PlayerVotes[player]);
                    break;
                }

                await Task.Delay(1000);
            }

            if (SelectedAbilityNumber == 0) SelectedAbilityNumber = UnityEngine.Random.Range(1, 4);

            AddAbility(player, AbilitesVote[SelectedAbilityNumber - 1]);

            if (AbilitesVote.All(x => x == AbilitesVote[0]))
            {
                AddAbility(player, "[시너지] 중복 기연");

                for (int i = 1; i < 3; i++)
                    AddAbility(player, AbilitesVote[0]);
            }
        }

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;
            Exiled.Events.Handlers.Player.Jumping += OnJumping;
            Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
            Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
            Exiled.Events.Handlers.Player.TogglingRadio += OnTogglingRadio;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.Escaping += OnEscaping;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Player.DroppedItem += OnDroppedItem;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.Hurt += OnHurt;
            Exiled.Events.Handlers.Player.TriggeringTesla += OnTriggeringTesla;
            Exiled.Events.Handlers.Player.ChangingMicroHIDState += OnChangingMicroHIDState;
            Exiled.Events.Handlers.Player.VoiceChatting += OnVoiceChatting;

            Exiled.Events.Handlers.Scp173.Blinking += OnBlinking;

            Exiled.Events.Handlers.Scp049.Attacking += OnScp049Attacking;
            Exiled.Events.Handlers.Scp049.FinishingRecall += OnFinishingRecall;

            Exiled.Events.Handlers.Scp0492.ConsumedCorpse += OnConsumedCorpse;
            Exiled.Events.Handlers.Scp0492.TriggeringBloodlust += OnTriggeringBloodlust;

            Exiled.Events.Handlers.Scp096.Charging += OnCharging;

            Exiled.Events.Handlers.Scp106.Attacking += OnScp106Attacking;

            Exiled.Events.Handlers.Scp3114.Revealed += OnRevealed;

            Exiled.Events.Handlers.Scp079.GainingLevel += OnGainingLevel;
            Exiled.Events.Handlers.Scp079.Pinging += OnPinging;
            Exiled.Events.Handlers.Scp079.ZoneBlackout += OnZoneBlackout;
            Exiled.Events.Handlers.Scp079.ChangingSpeakerStatus += OnChangingSpeakerStatus;

            MapEditorReborn.Events.Handlers.Map.LoadingMap += OnLoadingMap;

            Timing.RunCoroutine(OnModeStarted());
            Timing.RunCoroutine(RequestManager());
            Timing.RunCoroutine(SynergyManager());

            Timing.RunCoroutine(UpgradeBody());
            Timing.RunCoroutine(FlashLight());
            Timing.RunCoroutine(Flamethrower());
            Timing.RunCoroutine(Blessing());
            Timing.RunCoroutine(Spirit());
            Timing.RunCoroutine(Twinkle());
            Timing.RunCoroutine(Medical());
            Timing.RunCoroutine(Radar());
            Timing.RunCoroutine(Radiation());
            Timing.RunCoroutine(StickySwamp());
        }

        public void ShowStatus(Player player)
        {
            if (!PlayerWorkstation.ContainsKey(player))
            {
                PlayerWorkstation.Add(player, new List<Vector3>());
                PlayerAbilities.Add(player, new List<string>());
            }
            else
            {
                if (PlayerAbilities[player].Count() <= 0)
                {
                    if (player.Role.Type == RoleTypeId.Scp079)
                        player.ShowHint($"<align=left><b><size=22>레벨이 오를 때마다 능력을 획득할 수 있습니다.</size></b></align>", 1.2f);

                    else
                        player.ShowHint($"<align=left><b><size=22>워크스테이션 위에서 점프하면 능력을 획득할 수 있습니다.</size></b></align>", 1.2f);

                }
                else
                {
                    string abilitiesText = string.Join(", ", PlayerAbilities[player]);
                    abilitiesText = ColorFormat(abilitiesText);

                    player.ShowHint($"<align=left><b><size=25>보유 업그레이드</size></b>\n<size=20>{abilitiesText}</size></align>", 1.2f);
                }
            }
        }

        public IEnumerator<float> OnModeStarted()
        {
            dj = GGUtils.Gtool.Spawn(RoleTypeId.Overwatch, Vector3.zero);

            Dictionary<ReferenceHub, string> register = new Dictionary<ReferenceHub, string>()
            {
                { dj, "dj" }
            };

            foreach (var reg in register)
            {
                try
                {
                    GGUtils.Gtool.Register(reg.Key, reg.Value);
                }
                catch
                {
                }
            }

            GGUtils.Gtool.PlayerGet("dj").DisplayNickname = "DJ";

            Timing.CallDelayed(UnityEngine.Random.Range(1, 11), () => 
            {
                if (UnityEngine.Random.Range(1, 6) == 1)
                    IsFeverModeEnabled = true;

                if (IsFeverModeEnabled)
                    Server.ExecuteCommand($"/mp load ABattle");
            });

            while (true)
            {
                foreach (var player in Player.List)
                {
                    try
                    {
                        Hint CurrentHint = player.CurrentHint;
                        bool IsStatusHint = CurrentHint != null && (CurrentHint.Content.Contains("워크스테이션") || CurrentHint.Content.Contains("보유 업그레이드"));

                        if (CurrentHint == null || IsStatusHint)
                        {
                            if (player.IsAlive)
                                ShowStatus(player);

                            else
                            {
                                if (player.Role is SpectatorRole spectator)
                                {
                                    if (spectator.SpectatedPlayer != null && spectator.SpectatedPlayer.CurrentHint != null)
                                        player.ShowHint(spectator.SpectatedPlayer.CurrentHint.Content, 1.2f);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }

                yield return Timing.WaitForSeconds(0.5f);
            }
        }

        public IEnumerator<float> RequestManager()
        {
            while (true)
            {
                foreach (var Request in RGM.Instance.Requests.ToList())
                {
                    try
                    {
                        string[] req = Request.Split('/');

                        if (req[0] == "ABattle")
                        {
                            Player player = Player.Get(req[1]);

                            if (req[2] == "Add")
                            {
                                if (req[3] == "Random")
                                    AddAbility(player);

                                else
                                    AddAbility(player, req[3]);
                            }
                            else if (req[2] == "Vote")
                            {
                                string VoteNum;

                                if (req[3] == "Random")
                                    VoteNum = $"{UnityEngine.Random.Range(1, 4)}";

                                else
                                    VoteNum = req[3];

                                PlayerVotes.Add(player, VoteNum);

                                Timing.CallDelayed(1f, () =>
                                {
                                    if (PlayerVotes.ContainsKey(player))
                                        PlayerVotes.Remove(player);
                                });
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }

                RGM.Instance.Requests.Clear();

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public IEnumerator<float> SynergyManager()
        {
            while (true)
            {
                try
                {
                    foreach (var player in Player.List.Where(PlayerAbilities.ContainsKey))
                    {
                        foreach (var synergy in Synergies)
                        {
                            if (synergy.Value.Skip(1).All(x => PlayerAbilities[player].Intersect(synergy.Value.Skip(1)).Count() >= synergy.Value.Skip(1).Count()))
                            {
                                if (!PlayerAbilities[player].Contains(synergy.Key))
                                    AddAbility(player, synergy.Key);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public IEnumerator<float> UpgradeBody()
        {
            while (true)
            {
                try
                {
                    foreach (var player in Player.List.Where(PlayerAbilities.ContainsKey))
                    {
                        if (PlayerAbilities[player].Contains("[희귀] 육체 강화"))
                            if (player.MaxHealth > player.Health)
                                player.Health += DuplicateCount(player, "[희귀] 육체 강화");
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public IEnumerator<float> Spirit()
        {
            while (true)
            {
                try
                {
                    foreach (var player in Player.List.Where(PlayerAbilities.ContainsKey))
                    {
                        if (PlayerAbilities[player].Contains("[신화] 스피릿"))
                            player.EnableEffect(EffectType.Invisible);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                yield return Timing.WaitForSeconds(2f);
            }
        }

        public IEnumerator<float> FlashLight()
        {
            while (true)
            {
                try
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
                                    target.EnableEffect(EffectType.Flashed, 1, 1f);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                yield return Timing.WaitForSeconds(0.1f);
            }
        }

        public IEnumerator<float> Flamethrower()
        {
            while (true)
            {
                try
                {
                    foreach (var Item in Item.List.Where(x => x.Type == ItemType.MicroHID))
                    {
                        if (FlamethrowerSerials.Contains(Item.Serial))
                        {
                            MicroHid MicroHID = (MicroHid)Item;

                            MicroHID.Energy += 0.05f;
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public IEnumerator<float> Blessing()
        {
            while (true)
            {
                try
                {
                    foreach (var player in Player.List)
                    {
                        if (player.IsAlive && PlayerAbilities.ContainsKey(player) && PlayerAbilities[player].Contains("[전설] 영매"))
                        {
                            int s = player.CurrentSpectatingPlayers.Count();

                            player.GetEffect(EffectType.MovementBoost).Intensity = (byte)(2.5 * s * DuplicateCount(player, "[전설] 영매"));
                            player.GetEffect(EffectType.DamageReduction).Intensity = (byte)(2.5 * s * DuplicateCount(player, "[전설] 영매"));
                            player.Heal(0.35f * s * DuplicateCount(player, "[전설] 영매"));

                            if (player.Role is Scp079Role scp079)
                                scp079.Energy += 0.35f * s * DuplicateCount(player, "[전설] 영매");

                            if (s > 5)
                                player.IsUsingStamina = false;

                            else
                                player.IsUsingStamina = true;

                            if (s > 10)
                                player.IsBypassModeEnabled = true;

                            else
                                player.IsBypassModeEnabled = false;

                            if (s > 15)
                                player.EnableEffect(EffectType.Ghostly, 1, 1.2f);

                            if (s > 20)
                                player.EnableEffect(EffectType.Invisible, 1, 1.2f);

                            if (s > 25)
                            {
                                if (UnityEngine.Random.Range(1, 51) == 1)
                                {
                                    Item Item = player.AddItem(Tools.GetRandomValue(Tools.EnumToList<ItemType>()));

                                    if (player.IsScp)
                                        player.CurrentItem = Item;
                                }
                            }

                            if (s > 30)
                            {
                                if (!player.IsNoclipPermitted)
                                    player.IsNoclipPermitted = true;

                                player.AddBroadcast(1, "<b><i>[ALT] 키를 눌러 <color=red>신의 권능</color>을 사용할 수 있습니다!!!</i></b>");
                            }

                            else
                            {
                                if (player.IsNoclipPermitted)
                                    player.IsNoclipPermitted = false;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public IEnumerator<float> Twinkle()
        {
            while (true)
            {
                try
                {
                    foreach (var player in Player.List.Where(PlayerAbilities.ContainsKey))
                    {
                        if (PlayerAbilities.ContainsKey(player) && PlayerAbilities[player].Contains("[신화] 눈빛맨"))
                        {
                            foreach (var near in Player.List.Where(x => x.IsAlive && Vector3.Distance(x.Position, player.Position) < 11))
                            {
                                if (player != near && player.LeadingTeam != near.LeadingTeam)
                                {
                                    near.EnableEffect(EffectType.SinkHole, 1, 0.2f);
                                    near.EnableEffect(EffectType.Blinded, 1, 0.2f);
                                    near.Hurt(0.1f, "눈빛의 힘에 압도당했습니다.");
                                    Hitmarker.SendHitmarkerDirectly(player.ReferenceHub, 1f);
                                }
                            }

                            if (Tools.TryGetLookPlayer(player, 100f, out Player target))
                            {
                                if (player != target && player.LeadingTeam != target.LeadingTeam)
                                {
                                    target.EnableEffect(EffectType.SinkHole, 1, 0.2f);
                                    target.EnableEffect(EffectType.Blinded, 1, 0.2f);
                                    target.Hurt(0.1f, "눈빛의 힘에 압도당했습니다.");
                                    Hitmarker.SendHitmarkerDirectly(player.ReferenceHub, 0.5f);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                yield return Timing.WaitForSeconds(0.1f);
            }
        }

        public IEnumerator<float> Medical()
        {
            while (true)
            {
                try
                {
                    foreach (var player in Player.List.Where(x => x.IsAlive))
                    {
                        if (PlayerAbilities.ContainsKey(player))
                        {
                            if (PlayerAbilities[player].Contains("[전용] 의무병"))
                            {
                                foreach (var team in Player.List.Where(x => x.LeadingTeam == player.LeadingTeam && x.IsAlive && x != player))
                                {
                                    if (team.Health < team.MaxHealth)
                                        team.Health += 0.5f;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public IEnumerator<float> Radar()
        {
            while (true)
            {
                try
                {
                    foreach (var player in Player.List.Where(PlayerAbilities.ContainsKey))
                    {
                        if (player.CurrentItem != null && RadarSerials.Contains(player.CurrentItem.Serial))
                        {
                            if (Tools.TryGetNearestPlayer(player, out Player nearestPlayer, out float radius))
                            {
                                if (nearestPlayer != null && radius < 99999)
                                    player.ShowHint($"<color={nearestPlayer.Role.Color.ToHex()}>{Translations.Role[nearestPlayer.Role.Type]}</color> - {radius}m", 1.2f);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public IEnumerator<float> Radiation()
        {
            LightSourceSerializable LightSource = new LightSourceSerializable("#FFD700", 100, 20, true);
            LightSourceObject Light = ObjectSpawner.SpawnLightSource(LightSource, Vector3.zero);

            while (true)
            {
                try
                {
                    foreach (Player player in Player.List.Where(x => x.IsAlive))
                    {
                        if (Tools.TryGetLookPlayer(player, 45f, out Player target))
                        {
                            if (PlayerAbilities.ContainsKey(target))
                            {
                                if (PlayerAbilities[target].Contains("[시너지] 광휘"))
                                {
                                    if (player != target && player.LeadingTeam != target.LeadingTeam)
                                    {
                                        Light.Position = target.Position;

                                        Hitmarker.SendHitmarkerDirectly(target.ReferenceHub, 0.8f);
                                        player.EnableEffect(EffectType.Flashed, 1, 1f);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                yield return Timing.WaitForSeconds(0.1f);
            }
        }

        public IEnumerator<float> StickySwamp()
        {
            while (true)
            {
                try
                {
                    foreach (var player in Player.List.Where(PlayerAbilities.ContainsKey))
                    {
                        if (PlayerAbilities[player].Contains("[전용] 끈적한 늪"))
                        {
                            foreach (var near in Player.List.Where(x => x.IsAlive && Vector3.Distance(x.Position, player.Position) < 6))
                            {
                                if (player != near && player.LeadingTeam != near.LeadingTeam)
                                    near.EnableEffect(EffectType.SinkHole, 1, 0.2f);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                yield return Timing.WaitForSeconds(0.1f);
            }
        }

        // -------------------------------------------------------------------------------------------------------------------------- //

        public async void AddAbility(Player player, string force = null)
        {
            string abilityGrade = PickAbilityGrade(player, force);
            string abilityName = force == null ? Tools.GetRandomValue(AbilityList(player, abilityGrade).Keys.ToList()) : force;

            ApplyGiveAbility(player, abilityGrade, abilityName);

            string aT = abilityName.Replace("[전용] ", "").Replace("[일반] ", "").Replace("[희귀] ", "").Replace("[영웅] ", "").Replace("[전설] ", "").Replace("[신화] ", "").Replace("[시너지] ", "");

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
                    List<ItemType> RandomBox = Tools.EnumToList<ItemType>();

                    Item RandomBoxItem = player.AddItem(Tools.GetRandomValue(RandomBox));

                    if (player.IsScp)
                        player.CurrentItem = RandomBoxItem;
                    break;
                case "위치 추적":
                    Player target1 = Tools.GetRandomValue(Player.List.Where(x => x.IsAlive).ToList());

                    for (int i = 1; i < 11; i++)
                    {
                        player.ShowHint($"소속이 <color={target1.Role.Color.ToHex()}>{target1.Role.Name}</color>인 ???은(는) <b>{target1.CurrentRoom.Name}</b>에 있습니다.", 1.2f);
                        await Task.Delay(1000);
                    }
                    break;
                case "뽑기":
                    Item pc = player.AddItem(ItemType.Coin);
                    PickCoinSerials.Add(pc.Serial);

                    if (player.IsScp)
                        player.CurrentItem = pc;
                    break;
                case "보급":
                    List<string> Ammos = new List<string> { "19", "22", "27", "28", "29" };

                    for (int i = 1; i < 4; i++)
                        Server.ExecuteCommand($"/give {player.Id} {Tools.GetRandomValue(Ammos)}");
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
                            Timing.CallDelayed(1f, () => { player.AddBroadcast(5, $"<size=25>당신은 {player.CurrentSpectatingPlayers.ToList()[0].Nickname}로부터 신내림을 받았습니다.</size>"); } );
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
                case "잠행": player.GetEffect(EffectType.SilentWalk).Intensity += 3; break;
                case "위기 탈출":
                    Item ec = player.AddItem(ItemType.Coin);
                    EscapeCoinSerials.Add(ec.Serial);

                    if (player.IsScp)
                        player.CurrentItem = ec;
                    break;
                case "저체중":
                    if (player.Role is FpcRole fpc) fpc.JumpingSpeed *= 12 / 10; break;
                case "지진":
                    Item ec_1 = player.AddItem(ItemType.Coin);
                    EarthquakeCoinSerials.Add(ec_1.Serial);

                    if (player.IsScp)
                        player.CurrentItem = ec_1;
                    break;
                case "우애":
                    if (Tools.TryGetNearestPlayer(player, out Player nearestPlayer, out float radius))
                    {
                        Item Own = Tools.GetRandomValue(player.Items.ToList());

                        nearestPlayer.AddItem(Own.Type);

                        player.ShowHint($"{nearestPlayer}(에)게 {Translations.Item[Own.Type]}(을)를 나누어 주었습니다.");
                        nearestPlayer.ShowHint($"{nearestPlayer}(으)로부터 {Translations.Item[Own.Type]}(을)를 나누어 받았습니다.");

                        if (nearestPlayer.IsScp)
                            nearestPlayer.CurrentItem = Own;
                    }
                    break;
                case "무지개":
                    player.TryAddCandy(CandyKindID.Rainbow);

                    if (player.IsScp)
                        Server.ExecuteCommand($"/forceeq {player.Id} 42");
                    break;
                case "바디백": player.GetEffect(EffectType.BodyshotReduction).Intensity += 6; break;
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
                    g.SpawnActive(Tools.GetRandomValue(Player.List.ToList().Where(x => x.IsAlive && x.Role.Team != player.Role.Team && player != x).ToList()).Position, player);
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

                    Timing.CallDelayed(25, () => 
                    { 
                        if (player.GetEffect(EffectType.MovementBoost).Intensity >= 50)
                            player.GetEffect(EffectType.MovementBoost).Intensity -= 50; 
                    });
                    break;
                case "하이패스":
                    RGM.Instance.GodModePlayers.Add(player);
                    Timing.CallDelayed(25, () =>
                    {
                        if (RGM.Instance.GodModePlayers.Contains(player))
                            RGM.Instance.GodModePlayers.Remove(player);
                    });
                    break;
                case "트리플악셀":
                    Item COM45 = player.AddItem(ItemType.GunCom45);
                    COM45.As<Firearm>().MaxAmmo /= 2;
                    COM45.As<Firearm>().Ammo = COM45.As<Firearm>().MaxAmmo;

                    player.AddItem(ItemType.Ammo9x19);

                    if (player.IsScp)
                        player.CurrentItem = COM45;
                    break;
                case "연금":
                    for (int i=1; i<4; i++)
                    {
                        List<ItemType> Pension = Tools.EnumToList<ItemType>();

                        Item pen = player.AddItem(Tools.GetRandomValue(Pension));

                        if (player.IsScp)
                            player.CurrentItem = pen;

                        await Task.Delay(60 * 1000);
                    }
                    break;
                case "계약":
                    Item cc_1 = player.AddItem(ItemType.Coin);
                    ContractCoinSerials.Add(cc_1.Serial);

                    if (player.IsScp)
                        player.CurrentItem = cc_1;
                    break;
                case "테러리스트의 유품":
                    player.TryAddCandy(CandyKindID.Pink);

                    if (player.IsScp)
                        Server.ExecuteCommand($"/forceeq {player.Id} 42");
                    break;
                case "랜덤상자":
                    List<ItemType> RandomChest = new List<ItemType>() 
                    { 
                        ItemType.ParticleDisruptor,
                        ItemType.Jailbird,
                        ItemType.MicroHID,
                        ItemType.SCP018,
                        ItemType.SCP1576,
                        ItemType.SCP2176,
                        ItemType.SCP207,
                        ItemType.AntiSCP207,
                        ItemType.SCP268,
                        ItemType.SCP500,
                        ItemType.KeycardO5
                    };

                    for (int i=1; i<3; i++)
                    {
                        Item RandomChestItem = player.AddItem(Tools.GetRandomValue(RandomChest));

                        if (player.IsScp)
                            player.CurrentItem = RandomChestItem;
                    }
                    break;
                case "럭키비키": PlayerWorkstation[player].Clear(); break;
                case "슈퍼 스타": Server.ExecuteCommand($"/speak {player.Id} enable"); break;
                case "초재생":
                    Item AntiScp207 = player.AddItem(ItemType.AntiSCP207);

                    if (player.IsScp)
                        player.CurrentItem = AntiScp207;
                    break;
                case "고스트룰": player.GetEffect(EffectType.Ghostly).Intensity += 1; break;
                case "잠수부": player.EnableEffect(EffectType.Invigorated); break;
                case "스피드왜건": player.GetEffect(EffectType.MovementBoost).Intensity += 100; break;
                case "뱀의 손 무전기":
                    Item SH = player.AddItem(ItemType.Radio);
                    CallSnakeHandsSerials.Add(SH.Serial);

                    if (player.IsScp)
                        player.CurrentItem = SH;
                    break;
                case "랜덤택배":
                    List<ItemType> ItemTypes = Tools.EnumToList<ItemType>();

                    for (int i=1; i<Server.PlayerCount + 1; i++)
                    {
                        Item Item = Item.Create(Tools.GetRandomValue(ItemTypes));

                        Item.CreatePickup(new Vector3(player.Position.x, player.Position.y + 1, player.Position.z));
                    }
                    break;
                case "플래시라이트":
                    Item fl = player.AddItem(ItemType.Flashlight);
                    FlashLightSerials.Add(fl.Serial);

                    if (player.IsScp)
                        player.CurrentItem = fl;
                    break;
                case "화염 방사기":
                    Item ft = player.AddItem(ItemType.MicroHID);
                    FlamethrowerSerials.Add(ft.Serial);

                    if (player.IsScp)
                        player.CurrentItem = ft;
                    break;
                case "주거칩입죄": player.AddItem(ItemType.SCP268); break;
                case "반란의 씨앗": Respawn.ChaosTickets += (int)(Player.List.Count() * 0.2); break;
                case "05 평의회": player.AddItem(ItemType.KeycardO5); break;
                case "공학 전공": player.AddItem(ItemType.SCP2176); break;
                case "특수부대의 씨앗": Respawn.NtfTickets += (int)(Player.List.Count() * 0.2); break;
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
                case "보건소 직원":
                    List<ItemType> HealItem = new List<ItemType>() 
                    {
                        ItemType.Medkit,
                        ItemType.Painkillers,
                        ItemType.Adrenaline,
                        ItemType.SCP500,
                        ItemType.SCP330
                    };

                    foreach (var team in Player.List.Where(x => x.IsAlive && x.LeadingTeam == player.LeadingTeam && Vector3.Distance(player.Position, x.Position) < 11))
                        team.AddItem(Tools.GetRandomValue(HealItem));

                    break;
                case "산업재해보험":
                    for (int i=1; i < 4; i++)
                        AddAbility(player, "[일반] 보험");

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
                case "레이더":
                    Item rd = player.AddItem(ItemType.Radio);
                    RadarSerials.Add(rd.Serial);

                    if (player.IsScp)
                        player.CurrentItem = rd;
                    break;
                case "혼돈의 카오스":
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
                case "혼돈의 가방":
                    List<ItemType> ChaosBag = Tools.EnumToList<ItemType>();

                    while (!player.IsInventoryFull)
                    {
                        Item ChaosBagItem = player.AddItem(Tools.GetRandomValue(ChaosBag));

                        if (player.IsScp)
                            player.CurrentItem = ChaosBagItem;
                    }
                    break;
                case "세치 혀":
                    Item Scp1576 = player.AddItem(ItemType.SCP1576);

                    if (player.IsScp)
                        player.CurrentItem = Scp1576;
                    break;
                case "제3세력":
                    List<Player> DeadPlayers = Player.List.Where(x => x.IsDead).ToList();
                    DeadPlayers.ShuffleList();

                    CallSnakeHand(player, DeadPlayers.Take(2).ToList());
                    break;
                case "SCP 연구자":
                    List<ItemType> SCPItems = new List<ItemType>()
                    {
                        ItemType.SCP018,
                        ItemType.SCP1576,
                        ItemType.SCP1853,
                        ItemType.SCP207,
                        ItemType.SCP2176,
                        ItemType.SCP244a,
                        ItemType.SCP244b,
                        ItemType.SCP268,
                        ItemType.SCP330,
                        ItemType.SCP500,
                        ItemType.AntiSCP207
                    };

                    Item SCPItem = player.AddItem(Tools.GetRandomValue(SCPItems));

                    if (player.IsScp)
                        player.CurrentItem = SCPItem;
                    break;
                case "능수능란":
                    if (player.Role is Scp049Role Scp049_2)
                        Scp049_2.GoodSenseCooldown /= 2;
                    break;
                case "급식":
                    player.MaxHealth = player.MaxHealth * 1.5f;
                    break;
                case "원수":
                    if (player.Role is Scp096Role Scp096)
                        Scp096.ChargeCooldown /= 2;
                    break;
                case "흉내쟁이":
                    if (player.Role is Scp939Role Scp939)
                        Scp939.MimicryCooldown = 0;
                    break;
                case "안아줘요":
                    if (player.Role is Scp939Role Scp939_1)
                        Scp939_1.AmnesticCloudCooldown /= 2;
                    break;
                case "민첩한 사냥 도구":
                    if (player.Role is Scp939Role Scp939_2)
                        Scp939_2.AttackCooldown /= 2;
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
                case "생존 전문가":
                    player.Health = player.Health * 5;
                    
                    if (player.MaxHealth < player.Health)
                        player.MaxHealth = player.Health;
                    break;
                case "랜덤 컬렉션":
                    List<string> Randoms = new List<string>()
                    {
                        "[일반] 랜덤박스",
                        "[영웅] 랜덤상자",
                        "[전설] 랜덤택배"
                    };

                    AddAbility(player, Tools.GetRandomValue(Randoms));
                    break;
                case "4대 운동":
                    AddAbilityVote(player);
                    break;
            }
        }

        public async void OnTogglingNoClip(Exiled.Events.EventArgs.Player.TogglingNoClipEventArgs ev)
        {
            if (!ev.Player.IsCuffed)
            {
                if (PlayerAbilities.ContainsKey(ev.Player) && PlayerAbilities[ev.Player].Contains("[일반] 회축"))
                {
                    if (Tools.TryGetLookPlayer(ev.Player, 4f, out Player player))
                    {
                        if (ev.Player != player && !MeleeCooldown.Contains(ev.Player) && ev.Player.LeadingTeam != player.LeadingTeam)
                        {
                            Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 0.7f);
                            player.Hurt(12.05f * 3 * DuplicateCount(ev.Player, "[일반] 회축"), "무지성으로 구타당해 죽었습니다.");

                            MeleeCooldown.Add(ev.Player);
                            await Task.Delay(1000);
                            MeleeCooldown.Remove(ev.Player);
                        }
                    }
                }

                if (PlayerAbilities[ev.Player].Contains("[전용] 절도죄"))
                {
                    if (!PickPocketCooldown.Contains(ev.Player))
                    {
                        if (Tools.TryGetLookPlayer(ev.Player, 2f, out Player player))
                        {
                            if (ev.Player != player && !MeleeCooldown.Contains(ev.Player))
                            {
                                if (!player.IsInventoryEmpty)
                                {
                                    Item Item = Tools.GetRandomValue(player.Items.ToList());

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

        public void OnJumping(Exiled.Events.EventArgs.Player.JumpingEventArgs ev)
        {
            if (Physics.Raycast(ev.Player.Position, Vector3.down, out RaycastHit hit, 5, (LayerMask)1))
            {
                Transform WorkStation = hit.transform.parent.parent;

                if (WorkStation.name.Contains("Work Station") && !PlayerWorkstation[ev.Player].Contains(WorkStation.position))
                {
                    PlayerWorkstation[ev.Player].Add(WorkStation.position);

                    AddAbilityVote(ev.Player);
                }
            }

            if (PlayerAbilities.ContainsKey(ev.Player) && PlayerAbilities[ev.Player].Contains("[영웅] 점멸"))
            {
                if (!TeleportCooldown.Contains(ev.Player))
                {
                    TeleportCooldown.Add(ev.Player);

                    Door nearestDoor = null;
                    float radius = 99999;

                    foreach (var door in Door.List.Where(x => !x.IsElevator && x.Zone != ZoneType.LightContainment && !x.Type.ToString().Contains("Scp079")))
                    {
                        float Distance = Vector3.Distance(door.Position, ev.Player.Position);

                        if (Distance < radius)
                        {
                            nearestDoor = door;
                            radius = Distance;
                        }
                    }

                    Vector3 pos = nearestDoor.Position;

                    ev.Player.Position = new Vector3(pos.x, pos.y + 2, pos.z);

                    Timing.CallDelayed(15, () => { TeleportCooldown.Remove(ev.Player); });
                }
            }
        }

        public void OnChangedItem(Exiled.Events.EventArgs.Player.ChangedItemEventArgs ev)
        {
            if (ev.Item != null)
            {
                if (PickCoinSerials.Contains(ev.Item.Serial))
                    ev.Player.ShowHint($"이 동전을 튕기면 <b><color={RatingColor["일반"]}>뽑기</color></b> 능력을 사용할 수 있습니다.");

                else if (EscapeCoinSerials.Contains(ev.Item.Serial))
                    ev.Player.ShowHint($"이 동전을 튕기면 <b><color={RatingColor["일반"]}>위기 탈출</color></b> 능력을 사용할 수 있습니다.");

                else if (EarthquakeCoinSerials.Contains(ev.Item.Serial))
                    ev.Player.ShowHint($"이 동전을 튕기면 <b><color={RatingColor["일반"]}>지진</color></b> 능력을 사용할 수 있습니다.");

                else if (FollowCoinSerials.Contains(ev.Item.Serial))
                    ev.Player.ShowHint($"이 동전을 튕기면 <b><color={RatingColor["희귀"]}>순간이동</color></b> 능력을 사용할 수 있습니다.");

                else if (GrapCoinSerials.Contains(ev.Item.Serial))
                    ev.Player.ShowHint($"이 동전을 튕기면 <b><color={RatingColor["희귀"]}>갈고리</color></b> 능력을 사용할 수 있습니다.");

                else if (ClockCoinSerials.Contains(ev.Item.Serial))
                    ev.Player.ShowHint($"이 동전을 튕기면 <b><color={RatingColor["희귀"]}>회중시계</color></color></b> 능력을 사용할 수 있습니다.");

                else if (ContractCoinSerials.Contains(ev.Item.Serial))
                    ev.Player.ShowHint($"이 동전을 튕기면 <b><color={RatingColor["희귀"]}>계약</color></color></b> 능력을 사용할 수 있습니다.");

                else if (CallSnakeHandsSerials.Contains(ev.Item.Serial))
                    ev.Player.ShowHint($"우클릭하면 <b><color={RatingColor["전설"]}>뱀의 손 무전기</color></b> 능력을 사용할 수 있습니다.");

                else if (FlashLightSerials.Contains(ev.Item.Serial))
                    ev.Player.ShowHint($"손전등을 상대에게 비추면 <b><color={RatingColor["전설"]}>플래시라이트</color></b> 능력을 사용할 수 있습니다.");

                else if (FlamethrowerSerials.Contains(ev.Item.Serial))
                    ev.Player.ShowHint($"<b><color={RatingColor["전설"]}>화염 방사기</color></b> 능력이 있는 Micro-HID 입니다!");

                else if (ChaosCoinSerials.Contains(ev.Item.Serial))
                    ev.Player.ShowHint($"이 동전을 튕기면 <b><color={RatingColor["전용"]}>혼돈의 손길</color></color></b> 능력을 사용할 수 있습니다.");
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
            else if (EscapeCoinSerials.Contains(ev.Item.Serial))
            {
                if (Tools.TryGetLookPlayer(ev.Player, 10f, out Player player))
                {
                    if (player.LeadingTeam != ev.Player.LeadingTeam)
                    {
                        ev.Item.Destroy();

                        player.EnableEffect(EffectType.Ensnared, 3f);

                        Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 1f);
                    }
                    else
                        ev.Player.ShowHint("잘못된 대상입니다.");
                }
                else
                    ev.Player.ShowHint("대상을 정확히 지정해 주세요.");
            }
            else if (EarthquakeCoinSerials.Contains(ev.Item.Serial))
            {
                ev.Item.Destroy();

                Warhead.Shake();
            }
            else if (FollowCoinSerials.Contains(ev.Item.Serial))
            {
                ev.Item.Destroy();

                Player target = Tools.GetRandomValue(Player.List.Where(x => x != ev.Player && x.IsAlive && x.Role.Type != RoleTypeId.Scp079).ToList());
                ev.Player.Position = target.Position;
            }
            else if (GrapCoinSerials.Contains(ev.Item.Serial))
            {
                ev.Item.Destroy();

                Player target1 = Tools.GetRandomValue(Player.List.Where(x => x.IsAlive && x != ev.Player && x.Role.Type != RoleTypeId.Scp079).ToList());
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
            else if (ContractCoinSerials.Contains(ev.Item.Serial))
            {
                ev.Item.Destroy();

                ev.Player.Kill("계약에 따라 당신은 죽었습니다.");
                
                while (!ev.Player.IsAlive)
                    await Task.Delay(100);

                for (int i=1; i<4; i++)
                    AddAbility(ev.Player);
            }
            else if (ChaosCoinSerials.Contains(ev.Item.Serial))
            {
                ev.Item.Destroy();

                if (PlayerAbilities.ContainsKey(ev.Player))
                {
                    PlayerAbilities[ev.Player].Clear();
                    PlayerWorkstation[ev.Player].Clear();
                }

                ev.Player.DisableAllEffects();
            }
        }

        public void OnTogglingRadio(Exiled.Events.EventArgs.Player.TogglingRadioEventArgs ev)
        {
            if (CallSnakeHandsSerials.Contains(ev.Item.Serial))
            {
                ev.Item.Destroy();

                ev.Player.Role.Set(RoleTypeId.Tutorial, SpawnReason.ForceClass, RoleSpawnFlags.None);

                CallSnakeHand(ev.Player, Player.List.Where(x => x.IsDead).ToList());
            }
        }

        public async void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[전용] RTX4090"))
            {
                ev.IsAllowed = false;

                PlayerAbilities[ev.Player].Clear();

                Timing.CallDelayed(0.1f, () =>
                {
                    ev.Player.Role.Set(RoleTypeId.Tutorial);

                    Vector3 Pos = Door.Get(DoorType.Scp079First).Position;
                    ev.Player.Position = new Vector3(Pos.x, Pos.y + 2, Pos.z);

                    for (int i = 1; i < UnityEngine.Random.Range(7, 12) * DuplicateCount(ev.Player, "[전용] RTX4090"); i++)
                        AddAbility(ev.Player);
                });
            }

            if (PlayerAbilities.ContainsKey(ev.Attacker) && PlayerAbilities.ContainsKey(ev.Player) && ev.Attacker != null)
            {
                if (PlayerAbilities[ev.Player].Contains("[일반] 보험"))
                {
                    ev.IsAllowed = false;

                    PlayerAbilities[ev.Player].Remove("[일반] 보험");
                    return;
                }

                if (PlayerAbilities[ev.Player].Contains("[영웅] 구사일생"))
                {
                    ev.IsAllowed = false;

                    PlayerAbilities[ev.Player].Remove("[영웅] 구사일생");

                    ev.Player.EnableEffect(EffectType.Blinded, 1, 3);
                    ev.Player.EnableEffect(EffectType.Invisible, 1, 3);
                    ev.Player.GetEffect(EffectType.MovementBoost).Intensity += 20;
                    RGM.Instance.GodModePlayers.Add(ev.Player);

                    await Task.Delay(3000);

                    if (ev.Player.GetEffect(EffectType.MovementBoost).Intensity >= 20)
                        ev.Player.GetEffect(EffectType.MovementBoost).Intensity -= 20;
                    if (RGM.Instance.GodModePlayers.Contains(ev.Player))
                        RGM.Instance.GodModePlayers.Remove(ev.Player);

                    return;
                }

                if (PlayerAbilities[ev.Player].Contains("[전설] 마술사"))
                {
                    ev.IsAllowed = false;

                    PlayerAbilities[ev.Player].Remove("[전설] 마술사");

                    ev.Player.Role.Set(ev.Attacker.Role, SpawnReason.ForceClass, RoleSpawnFlags.None);
                    ev.Player.Health = ev.Attacker.Health;
                    foreach (Item Item in ev.Attacker.Items)
                        ev.Player.AddItem(Item.Type);

                    PlayerAbilities[ev.Player].Clear();

                    foreach (string ability in PlayerAbilities[ev.Attacker])
                        AddAbility(ev.Player, ability);

                    ev.Attacker.Kill($"몸이 교체되는 마술에 당했네요!");

                    return;
                }

                if (PlayerAbilities[ev.Player].Contains("[신화] 조커"))
                {
                    ev.IsAllowed = false;

                    PlayerAbilities[ev.Player].Remove("[신화] 조커");

                    Player.List.ToList().ForEach(x => x.ShowHint("<b><i><color=#FF0000>조</color><color=#E70717>커</color><color=#D00F2E>를</color> <color=#A21E5C>건</color><color=#8B2673>들</color><color=#732E8B>인</color> <color=#453DB9>죄</color><color=#2E45D0>다</color><color=#174DE7>!</color></i></b>", 3));

                    ev.Player.MaxHealth *= UnityEngine.Random.Range(1, 4);
                    ev.Player.Health = ev.Player.MaxHealth;

                    RGM.Instance.GodModePlayers.Add(ev.Player);

                    AddAbility(ev.Player, Tools.GetRandomValue(LegendAbilities.Keys.ToList()));
                    AddAbility(ev.Player, Tools.GetRandomValue(MythicAbilities.Keys.ToList()));

                    await Task.Delay(10000);

                    if (RGM.Instance.GodModePlayers.Contains(ev.Player))
                        RGM.Instance.GodModePlayers.Remove(ev.Player);

                    return;
                }

                // 죽음이 확정된 상황

                if (PlayerAbilities[ev.Player].Contains("[영웅] 최후의 발악"))
                {
                    ev.IsAllowed = false;

                    PlayerAbilities[ev.Player].Remove("[영웅] 최후의 발악");

                    ev.Player.GetEffect(EffectType.MovementBoost).Intensity += 30;
                    RGM.Instance.GodModePlayers.Add(ev.Player);

                    Timing.CallDelayed(5f, () =>
                    {
                        if (RGM.Instance.GodModePlayers.Contains(ev.Player))
                            RGM.Instance.GodModePlayers.Remove(ev.Player);

                        if (ev.Player.IsAlive)
                            ev.Player.Kill("최후의 발악의 효과로 사망하였습니다.");
                    });
                }

                if (PlayerAbilities[ev.Player].Contains("[희귀] 순교"))
                {
                    for (int i = 1; i < DuplicateCount(ev.Player, "[희귀] 순교") + 1; i++)
                    {
                        var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, ev.Player);
                        g.FuseTime = 3f;
                        g.SpawnActive(ev.Player.Position, ev.Player);
                    }
                }

                if (PlayerAbilities[ev.Player].Contains("[영웅] 슈퍼 스타"))
                {
                    foreach (var player in Player.List)
                        player.AddBroadcast(10, $"<size=20><color={RatingColor["영웅"]}>슈퍼 스타</color>였던 {ev.Player.Nickname}(<color={ev.Player.Role.Color.ToHex()}>{Translations.Role[ev.Player.Role.Type]}</color>)(은)는 " +
                            $"{ev.Attacker.Nickname}(<color={ev.Attacker.Role.Color.ToHex()}>{Translations.Role[ev.Attacker.Role.Type]}</color>)에 의해 <b>{ev.Player.CurrentRoom.Name}</b>에서 사망하였습니다.</size>");
                }

                if (PlayerAbilities[ev.Player].Contains("[영웅] 극독"))
                {
                    ev.Attacker.EnableEffect(EffectType.CardiacArrest, 1, 12 * DuplicateCount(ev.Player, "[영웅] 극독"));

                    ev.Attacker.ShowHint("극독에 당했습니다!");
                }

                if (ev.Attacker != null)
                {
                    if (PlayerAbilities[ev.Attacker].Contains("[신화] 차원 강탈자"))
                    {
                        foreach (var Ability in PlayerAbilities[ev.Player])
                            PlayerAbilities[ev.Attacker].Add(Ability);

                        ev.Player.ShowHint("능력을 강탈당했습니다!");
                    }
                }
            }
        }

        public void OnDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            try
            {
                if (ev.Attacker != null && PlayerAbilities.ContainsKey(ev.Attacker) && PlayerAbilities.ContainsKey(ev.Player))
                {
                    if (PlayerAbilities[ev.Attacker].Contains("[전설] 킬스트릭"))
                    {
                        if (UnityEngine.Random.Range(1, 3) == 1)
                            AddAbilityVote(ev.Attacker);

                        else
                            AddAbility(ev.Attacker);
                    }

                    if (PlayerAbilities[ev.Attacker].Contains("[전용] 공포"))
                    {
                        foreach (var player in Player.List.Where(x => !x.IsScp))
                        {
                            if (Vector3.Distance(player.Position, ev.Attacker.Position) <= 10)
                                player.EnableEffect(EffectType.Ensnared, 1, 0.75f * DuplicateCount(ev.Attacker, "[전용] 공포"));
                        }
                    }
                }
            }
            finally
            {
                PlayerWorkstation[ev.Player].Clear();
                PlayerAbilities[ev.Player].Clear();
                ev.Player.Scale = new Vector3(1, 1, 1);
                Server.ExecuteCommand($"/speak {ev.Player.Id} disable");
                ev.Player.IsUsingStamina = true;
                if (RGM.Instance.GodModePlayers.Contains(ev.Player))
                    RGM.Instance.GodModePlayers.Remove(ev.Player);
            }
        }

        public void OnEscaping(Exiled.Events.EventArgs.Player.EscapingEventArgs ev)
        {
            if (PlayerAbilities.ContainsKey(ev.Player) && new List<RoleTypeId>() 
                { 
                    RoleTypeId.Scientist, 
                    RoleTypeId.ClassD 
                }.Contains(ev.Player.Role.Type))
            {
                PlayerAbilities[ev.Player].Clear();
                PlayerWorkstation[ev.Player].Clear();
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

            else if (ev.Player != null && ((PlayerAbilities[ev.Player].Contains("[일반] 행운") && UnityEngine.Random.Range(1, 101) <= 5 * DuplicateCount(ev.Player, "[일반] 행운") 
                || PlayerAbilities[ev.Player].Contains("[영웅] 수리 기사"))))
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
                if (UnityEngine.Random.Range(1, 101) <= 10 * ((1 / 2) * DuplicateCount(ev.Player, "[영웅] 도박꾼")))
                    ev.Player.EnableEffect(EffectType.SeveredHands);

                else
                {
                    ev.Pickup.Destroy();

                    List<ItemType> ItemTypes = Tools.EnumToList<ItemType>();

                    Item Item = Item.Create(Tools.GetRandomValue(ItemTypes));
                    Item.CreatePickup(ev.Player.Position);
                }
            }
        }

        public void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            if (ev.Attacker != null)
            {
                if (PlayerAbilities[ev.Attacker].Contains("[일반] 단련"))
                    ev.DamageHandler.Damage = (int)(ev.DamageHandler.Damage * (1 + (0.2 * DuplicateCount(ev.Player, "[일반] 단련"))));

                if (PlayerAbilities[ev.Attacker].Contains("[희귀] 흡혈귀") && ev.Attacker.LeadingTeam != ev.Player.LeadingTeam)
                    ev.Attacker.AddAhp((20 * DuplicateCount(ev.Player, "[희귀] 흡혈귀")) * (ev.DamageHandler.Damage / 100));

                if (ev.Attacker.CurrentItem != null && FlamethrowerSerials.Contains(ev.Attacker.CurrentItem.Serial))
                {
                    ev.DamageHandler.Damage /= 5;

                    ev.Player.EnableEffect(EffectType.Burned, 1, 1.2f);
                }

                if (PlayerAbilities[ev.Player].Contains("[희귀] 반창고"))
                {
                    if (ev.Player.Health <= ev.Player.MaxHealth / 2)
                    {
                        PlayerAbilities[ev.Player].Remove("[희귀] 반창고");

                        ev.Player.Health = ev.Player.MaxHealth;
                    }
                }

                if (PlayerAbilities[ev.Attacker].Contains("[신화] 로켓 런처") && ev.Attacker.LeadingTeam != ev.Player.LeadingTeam)
                {
                    if (UnityEngine.Random.Range(1, 21) == 1)
                        Server.ExecuteCommand($"/rocket {ev.Player.Id} 1");
                }

                if (PlayerAbilities[ev.Attacker].Contains("[전용] 집단 지성") && ev.Attacker.LeadingTeam != ev.Player.LeadingTeam)
                {
                    int PowerCount = 0;

                    foreach (var player in Player.List.Where(x => x.IsAlive && x.LeadingTeam == ev.Player.LeadingTeam && x != ev.Player))
                    {
                        if (Vector3.Distance(player.Position, ev.Player.Position) < 11)
                            PowerCount++;
                    }

                    ev.DamageHandler.Damage = (int)(ev.DamageHandler.Damage * (1 + (0.1 * PowerCount)));
                }

                if (PlayerAbilities[ev.Player].Contains("[전용] 신기루"))
                {
                    if (UnityEngine.Random.Range(1, 21) == 1)
                        ev.Player.EnableEffect(EffectType.Invisible, 1, 1.25f * DuplicateCount(ev.Player, "[전용] 신기루"));
                }

                if (PlayerAbilities[ev.Player].Contains("[전용] 격노"))
                {
                    if (ev.Player.Role is Scp096Role Scp096)
                    {
                        if (Scp096.RageManager.IsEnraged && ev.Attacker != null && ev.Attacker != ev.Player)
                            ev.DamageHandler.Damage /= 2;
                    }
                }

                if (PlayerAbilities[ev.Attacker].Contains("[전용] 별자리 찢기"))
                {
                    if (UnityEngine.Random.Range(1, 5) == 1)
                        ev.DamageHandler.Damage = -1;
                }

                if (PlayerAbilities[ev.Attacker].Contains("[전용] 숙련된 암살자"))
                {
                    if (ev.DamageHandler.Type == DamageType.Strangled)
                        ev.DamageHandler.Damage *= 10;
                }

                if (PlayerAbilities[ev.Player].Contains("[전용] 반블럭"))
                {
                    ev.Player.GetEffect(EffectType.MovementBoost).Intensity += 25;

                    Timing.CallDelayed(3f, () =>
                    {
                        if (ev.Player.GetEffect(EffectType.MovementBoost).Intensity >= 25)
                            ev.Player.GetEffect(EffectType.MovementBoost).Intensity -= 25;
                    });
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

        public void OnChangingMicroHIDState(Exiled.Events.EventArgs.Player.ChangingMicroHIDStateEventArgs ev)
        {
            if (FlamethrowerSerials.Contains(ev.Player.CurrentItem.Serial))
            {
                if (ev.OldState == HidState.Idle && ev.NewState == HidState.PoweringUp)
                    ev.NewState = HidState.Firing;

                else if (ev.OldState == HidState.Firing && ev.NewState == HidState.PoweringDown)
                {
                    Timing.CallDelayed(0.1f, () => 
                    { 
                        ev.NewState = HidState.Idle; 
                    });
                }
            }
        }

        public async void OnVoiceChatting(Exiled.Events.EventArgs.Player.VoiceChattingEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[전설] 괴성"))
            {
                if (!RoaringSoundCooldown.Contains(ev.Player))
                {
                    if (Tools.TryGetLookPlayer(ev.Player, 10f, out Player target) && target.LeadingTeam != ev.Player.LeadingTeam)
                    {
                        RoaringSoundCooldown.Add(ev.Player);

                        foreach (var player in Player.List.Where(x => x.LeadingTeam != ev.Player.LeadingTeam && x.IsAlive))
                        {
                            GGUtils.Gtool.PlayerGet("dj").DisplayNickname = $"{player.Nickname}의 괴성";

                            await Task.Delay(10);

                            GGUtils.Gtool.PlaySound("dj", "GmanRoaringSound", VoiceChat.VoiceChatChannel.Intercom);

                            await Task.Delay(1250);

                            player.EnableEffect(EffectType.Blinded, 1, 5f);
                            player.EnableEffect(EffectType.SinkHole, 1, 10f);
                        }

                        Player.List.ToList().ForEach(x => x.ShowHint("<b><i><color=#B08A03>저</color><color=#9C7A02>?</color><color=#886B02>!</color><color=#755C01>주</color><color=#614C01>받</color><color=#4E3D01>은</color> <color=#271E00>과</color><color=#130F00>성</color></i></b>", 5));

                        for (int i = 1; i < 51; i++)
                        {
                            Warhead.Shake();

                            await Task.Delay(100);
                        }

                        await Task.Delay(98 * 1000 * (1 / 2 * (DuplicateCount(ev.Player, "[전설] 괴성") - 1)));

                        if (RoaringSoundCooldown.Contains(ev.Player))
                            RoaringSoundCooldown.Remove(ev.Player);
                    }
                }
            }
        }

        public void OnBlinking(Exiled.Events.EventArgs.Scp173.BlinkingEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[전용] 괴이"))
            {
                if (!ev.Player.CurrentRoom.AreLightsOff)
                    ev.Player.CurrentRoom.TurnOffLights(0.5f * DuplicateCount(ev.Player, "[전용] 괴이"));
            }
        }

        public void OnScp049Attacking(Exiled.Events.EventArgs.Scp049.AttackingEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[전용] 사자"))
            {
                Timing.CallDelayed(0.1f, () =>
                {
                    ev.Scp049.RemainingAttackCooldown /= 2;
                });
            }
        }

        public void OnFinishingRecall(Exiled.Events.EventArgs.Scp049.FinishingRecallEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[전용] 유능한 의사"))
            {
                if (UnityEngine.Random.Range(1, 3) == 1)
                {
                    Timing.CallDelayed(0.1f, () =>
                    {
                        ev.Target.MaxHealth *= 3 / 2;
                        ev.Target.Health = ev.Target.MaxHealth;
                    });
                }
            }
        }

        public void OnConsumedCorpse(Exiled.Events.EventArgs.Scp0492.ConsumedCorpseEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[전용] 허기"))
                ev.ConsumeHeal *= 2;
        }
        
        public void OnTriggeringBloodlust(Exiled.Events.EventArgs.Scp0492.TriggeringBloodlustEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[전용] 당혹감"))
            {
                ev.Target.EnableEffect(EffectType.Blinded, 1, 0.5f);
                Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 0.2f);
            }
        }

        public void OnCharging(Exiled.Events.EventArgs.Scp096.ChargingEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[전용] 천리안"))
            {
                foreach (var player in Player.List.Where(x => x.IsHuman))
                {
                    if (Vector3.Distance(player.Position, ev.Player.Position) < 31)
                        ev.Scp096.AddTarget(player);
                }
            }
        }

        public void OnScp106Attacking(Exiled.Events.EventArgs.Scp106.AttackingEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[전용] 사냥감 모색"))
            {
                ev.Player.GetEffect(EffectType.MovementBoost).Intensity += 25;

                Timing.CallDelayed(3f, () => 
                {
                    if (ev.Player.GetEffect(EffectType.MovementBoost).Intensity >= 25)
                        ev.Player.GetEffect(EffectType.MovementBoost).Intensity -= 25;
                });
            }
        }

        public void OnRevealed(Exiled.Events.EventArgs.Scp3114.RevealedEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[전용] 도라에몽 주머니"))
            {
                List<ItemType> ItemTypes = Tools.EnumToList<ItemType>();

                Item Item = ev.Player.AddItem(Tools.GetRandomValue(ItemTypes));

                if (ev.Player.IsScp)
                    ev.Player.CurrentItem = Item;
            }
        }


        public void OnGainingLevel(Exiled.Events.EventArgs.Scp079.GainingLevelEventArgs ev)
        {
            AddAbility(ev.Player);
        }

        public void OnPinging(Exiled.Events.EventArgs.Scp079.PingingEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[전용] 핑 리모컨"))
            {
                if (!ev.Room.AreLightsOff)
                    ev.Room.TurnOffLights(0.5f * DuplicateCount(ev.Player, "[전용] 핑 리모컨"));
            }
        }

        public void OnZoneBlackout(Exiled.Events.EventArgs.Scp079.ZoneBlackoutEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[전용] 랜덤 함수"))
            {
                for (int i = 1; i < 6 * DuplicateCount(ev.Player, "[전용] 랜덤 함수"); i++)
                {
                    Room SelectedRoom = Tools.GetRandomValue(Room.List.ToList());

                    SelectedRoom.TurnOffLights(10);
                }
            }
        }

        public void OnChangingSpeakerStatus(Exiled.Events.EventArgs.Scp079.ChangingSpeakerStatusEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[전용] 고대의 존재 압도"))
            {
                foreach (var player in Player.List.Where(x => !x.IsScp && x.IsAlive))
                {
                    if (player.CurrentRoom == ev.Room)
                        player.EnableEffect(EffectType.SinkHole, 1, 1 * DuplicateCount(ev.Player, "[전용] 고대의 존재 압도"));
                }
            }
        }

        public void OnLoadingMap(MapEditorReborn.Events.EventArgs.LoadingMapEventArgs ev)
        {
            if (ev.NewMap.Name == "ABattle")
                Player.List.ToList().ForEach(x => x.AddBroadcast(10, "<size=25><b><i><color=#FF00EA>피</color><color=#EF00EB>버</color> <color=#CF00ED>모</color><color=#BF00EF>드</color><color=#AF00F0>가</color> <color=#8F00F3>활</color><color=#7F00F4>성</color><color=#6F00F5>화</color><color=#5F00F7>되</color><color=#4F00F8>었</color><color=#3F00F9>습</color><color=#2F00FB>니</color><color=#1F00FC>다</color><color=#0F00FD>!</color></i></b></size>"));
        }
    }
}
