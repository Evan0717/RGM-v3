using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace RGM.Modes
{
    public class SpecialAbility
    {
        public static SpecialAbility Instance;

        public List<ushort> N_AbilityCoins = new List<ushort>();
        public List<ushort> R_AbilityCoins = new List<ushort>();
        public List<ushort> SR_AbilityCoins = new List<ushort>();
        public List<ushort> SSR_AbilityCoins = new List<ushort>();
        public List<ushort> X_AbilityCoins = new List<ushort>();

        public Dictionary<Player, string> PlayerAbilities = new Dictionary<Player, string>();

        public Dictionary<string, string> N_Abilities = new Dictionary<string, string>()
        {
            { "재생", "N/0/매 초마다 체력이 2% 증가합니다." },
            { "운동", "N/1/최대 체력이 50% 추가로 증가합니다." },
            { "벌크업", "N/2/공격력이 20% 증가합니다." },
            { "종이접기", "N/3/몸이 종잇장이 됩니다." },
            { "경공", "N/4/이동 속도가 조금 증가합니다." },
            { "만병통치약", "N/5/해로운 효과에 면역이 됩니다." },
            { "얼어붙은 심장", "N/6/스테미나가 무제한이 됩니다." },
            { "무기 전문가", "N/7/SCP-1853의 효과를 가지고 시작합니다." },
            { "가챠", "N/8/랜덤한 아이템을 가지고 시작합니다." },
            { "갑주", "N/9/5의 피해 감소 효과를 가지고 시작합니다." },
            { "권법", "N/10/ALT키를 눌러 근거리의 상대에게 30 데미지를 줍니다. (콜타임 1초)" },
            { "[여긴 어디?] 박스", "N/11/랜덤한 위치로 이동된 상태로 시작합니다." }
        };
        public Dictionary<string, string> R_Abilities = new Dictionary<string, string>()
        {
            { "광전사", "R/0/남은 체력이 적어질수록 공격력이 1%씩 증가합니다." },
            { "흡혈귀", "R/1/상대를 공격하면 데미지의 40%를 AHP로 받습니다." },
            { "행운아", "R/2/문에 상호작용하면 5% 확률로 문이 열립니다." },
            { "난쟁이", "R/3/몸의 크기가 50% 작아집니다." },
            { "킬스트릭", "R/4/상대를 죽일 때마다 랜덤한 버프를 얻습니다." }
        };
        public Dictionary<string, string> SR_Abilities = new Dictionary<string, string>()
        {
            { "보급", "SR/0/1분마다 랜덤한 아이템이 지급됩니다." }
        };
        public Dictionary<string, string> SSR_Abilities = new Dictionary<string, string>()
        {
            { "마술사", "SRR/0/죽으면 죽인 자와 몸이 교체됩니다." }
        };
        public Dictionary<string, string> X_Abilities = new Dictionary<string, string>()
        {
            { "눈빛맨", "X/0/매 초마다 쳐다본 객체는 하늘로 승천합니다." }
        };
        public Dictionary<string, string> CCTV_Abilities = new Dictionary<string, string>()
        {
            { "RTX4090", "CCTV/0/3티어부터 시작합니다." }
        };

        Dictionary<string, string> AllAbilities = new Dictionary<string, string>();

        string ColorPicker(string Rank)
        {
            Dictionary<string, string> Colors = new Dictionary<string, string>()
                    {
                        { "X", "FF0000" },
                        { "SSR", "F7FE2E" },
                        { "SR", "FE2EF7" },
                        { "R", "0080FF" },
                        { "N", "BDBDBD" },
                        { "CCTV", "58FA58" }
                    };

            return Colors[Rank];
        }

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(1f);

            AllAbilities = N_Abilities
                .Concat(R_Abilities)
                .Concat(SR_Abilities)
                .Concat(SSR_Abilities)
                .Concat(X_Abilities)
                .Concat(CCTV_Abilities)
                .ToDictionary(x => x.Key, x => x.Value);

            foreach (var player in Player.List)
            {
                Spawned(player);
            }

            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (player.CurrentItem != null)
                    {
                        string Rank()
                        {
                            if (N_AbilityCoins.Contains(player.CurrentItem.Serial))
                            {
                                return $"<color=#{ColorPicker("N")}>N</color>";
                            }
                            else if (R_AbilityCoins.Contains(player.CurrentItem.Serial))
                            {
                                return $"<color=#{ColorPicker("R")}>R</color>";
                            }
                            else if (SR_AbilityCoins.Contains(player.CurrentItem.Serial))
                            {
                                return $"<color=#{ColorPicker("SR")}>SR</color>";
                            }
                            else if (SSR_AbilityCoins.Contains(player.CurrentItem.Serial))
                            {
                                return $"<color=#{ColorPicker("SSR")}>SSR</color>";
                            }
                            else if (X_AbilityCoins.Contains(player.CurrentItem.Serial))
                            {
                                return $"<color=#{ColorPicker("X")}>X</color>";
                            }
                            else
                            {
                                return $"<color=#{ColorPicker("CCTV")}>CCTV</color>";
                            }
                        }

                        player.ShowHint($"동전을 튕겨 <b><color=#FF0000>특</color><color=#CC1833>수</color><color=#993166>⭐</color><color=#664999>능</color><color=#3362CC>력</color></b>을 획득하세요.\n<size=25>[ 현재 등급 : <b>{Rank()}</b> ]</size>", 1.2f);
                    }
                }

                yield return Timing.WaitForSeconds(1);
            }
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            Item Coin = player.AddItem(ItemType.Coin);
            N_AbilityCoins.Add(Coin.Serial);

            if (player.Role.Type == RoleTypeId.Scp079)
            {
                AddAbility(player, CCTV_Abilities);
            }
            else if (player.IsScp)
            {
                player.CurrentItem = Coin;
            }
        }

        public void OnChangingRole(Exiled.Events.EventArgs.Player.ChangingRoleEventArgs ev)
        {
            ev.Player.Scale = new Vector3(1, 1, 1);
            if (PlayerAbilities.ContainsKey(ev.Player))
                PlayerAbilities.Remove(ev.Player);
        }


        public void AddAbility(Player player, Dictionary<string, string> Abilities)
        {
            string SelectedAbility;
            int Random = UnityEngine.Random.Range(1, 1001);

            SelectedAbility = RGM.GetRandomValue(CCTV_Abilities.Keys.ToList());
            PlayerAbilities.Add(player, SelectedAbility);

            string[] strings = AllAbilities[SelectedAbility].Split('/');

            player.ShowHint($"<b><color=#{ColorPicker(strings[0])}>[{strings[0]}] {SelectedAbility}</color></b> 특수능력을 획득하였습니다.\n<size=20>{strings[2]}</size>", 10);

            var modeType = Type.GetType($"RGM.Modes.SpecialAbilities.{strings[0]}{strings[1]}");
            if (modeType != null)
            {
                var modeInstance = Activator.CreateInstance(modeType);
                var onEnabledMethod = modeType.GetMethod("OnEnabled");
                onEnabledMethod?.Invoke(modeInstance, new object[] { player });
            }
        }

        public void OnFlippingCoin(Exiled.Events.EventArgs.Player.FlippingCoinEventArgs ev)
        {
            Item Coin = ev.Player.CurrentItem;

            if (!PlayerAbilities.ContainsKey(ev.Player))
            {
                if (UnityEngine.Random.Range(1, 4) == 1) // 동전 등급 업그레이드
                {
                    if (N_AbilityCoins.Contains(Coin.Serial))
                    {
                        N_AbilityCoins.Remove(Coin.Serial);
                        R_AbilityCoins.Add(Coin.Serial);
                    }
                    else if (R_AbilityCoins.Contains(Coin.Serial))
                    {
                        R_AbilityCoins.Remove(Coin.Serial);
                        SR_AbilityCoins.Add(Coin.Serial);
                    }
                    else if (SR_AbilityCoins.Contains(Coin.Serial))
                    {
                        SR_AbilityCoins.Remove(Coin.Serial);
                        SSR_AbilityCoins.Add(Coin.Serial);
                    }
                    else if (SSR_AbilityCoins.Contains(Coin.Serial))
                    {
                        SSR_AbilityCoins.Remove(Coin.Serial);
                        X_AbilityCoins.Add(Coin.Serial);
                    }
                    else if (X_AbilityCoins.Contains(Coin.Serial))
                    {
                        X_AbilityCoins.Remove(Coin.Serial);
                        N_AbilityCoins.Add(Coin.Serial);
                    }
                    else
                    {
                        ev.Player.ShowHint("이 동전은 특수능력을 가지고 있지 않습니다.", duration: 1.5f);
                    }
                }
                else // 능력 뽑기
                {
                    if (N_AbilityCoins.Contains(Coin.Serial))
                    {
                        AddAbility(ev.Player, N_Abilities);
                    }
                    else if (R_AbilityCoins.Contains(Coin.Serial))
                    {
                        AddAbility(ev.Player, R_Abilities);
                    }
                    else if (SR_AbilityCoins.Contains(Coin.Serial))
                    {
                        AddAbility(ev.Player, SR_Abilities);
                    }
                    else if (SSR_AbilityCoins.Contains(Coin.Serial))
                    {
                        AddAbility(ev.Player, SSR_Abilities);
                    }
                    else if (X_AbilityCoins.Contains(Coin.Serial))
                    {
                        AddAbility(ev.Player, X_Abilities);
                    }
                    else
                    {
                        ev.Player.ShowHint("이 동전은 특수능력을 가지고 있지 않습니다.", duration: 1.5f);
                    }
                }

                ev.Item.Destroy();
            }
            else
            {
                ev.Player.ShowHint("이미 능력을 획득하였습니다.", duration: 1.5f);
            }
        }
    }
}
