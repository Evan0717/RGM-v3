using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using System.Reflection;

namespace RGM.Modes
{
    public class SpecialAbility
    {
        public static SpecialAbility Instance;

        public List<ushort> AbilityCoins = new List<ushort>();
        public Dictionary<Player, string> PlayerAbilities = new Dictionary<Player, string>();

        public Dictionary<string, string> N_Abilities = new Dictionary<string, string>()
        {
            { "재생", "N/0/매 초마다 체력이 2% 증가합니다." },
            { "운동", "N/1/최대 체력이 50% 추가로 증가합니다." },
            { "벌크업", "N/2/공격력이 20% 증가합니다." },
            { "종이접기", "N/3/몸이 종잇장이 됩니다." }
        };
        public Dictionary<string, string> R_Abilities = new Dictionary<string, string>()
        {
            { "광전사", "R/0/남은 체력이 적어질수록 공격력이 1%씩 증가합니다." },
            { "흡혈귀", "R/1/상대를 공격하면 데미지의 20%를 AHP로 받습니다." },
            { "행운아", "R/2/문에 상호작용하면 5% 확률로 문이 열립니다." },
            { "난쟁이", "R/3/몸의 크기가 50% 작아집니다." }
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

        Dictionary<string, string> AllAbilities = new Dictionary<string, string>();

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.ChangingItem += OnChangingItem;
            Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;

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
                .ToDictionary(x => x.Key, x => x.Value);

            foreach (var player in Player.List)
            {
                Spawned(player);
            }
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            Item Coin = player.AddItem(ItemType.Coin);
            AbilityCoins.Add(Coin.Serial);
        }

        public void OnChangingItem(Exiled.Events.EventArgs.Player.ChangingItemEventArgs ev)
        {
            if (ev.Item != null)
                return;

            if (ev.Item.Type == ItemType.Coin)
            {
                if (AbilityCoins.Contains(ev.Item.Serial))
                {
                    ev.Player.ShowHint("동전을 튕겨 <b><color=#FF0000>특</color><color=#CC1833>수</color><color=#993166>⭐</color><color=#664999>능</color><color=#3362CC>력</color></b>을 획득하세요.");
                }
            }
        }

        public void OnFlippingCoin(Exiled.Events.EventArgs.Player.FlippingCoinEventArgs ev)
        {
            string SelectedAbility;
            int Random = UnityEngine.Random.Range(1, 1001);

            if (Random < 5)
            {
                SelectedAbility = RGM.GetRandomValue(X_Abilities.Keys.ToList());
                PlayerAbilities.Add(ev.Player, SelectedAbility);
            }
            else if (Random < 30)
            {
                SelectedAbility = RGM.GetRandomValue(SSR_Abilities.Keys.ToList());
                PlayerAbilities.Add(ev.Player, SelectedAbility);
            }
            else if (Random < 100)
            {
                SelectedAbility = RGM.GetRandomValue(SR_Abilities.Keys.ToList());
                PlayerAbilities.Add(ev.Player, SelectedAbility);
            }
            else if (Random < 300)
            {
                SelectedAbility = RGM.GetRandomValue(R_Abilities.Keys.ToList());
                PlayerAbilities.Add(ev.Player, SelectedAbility);
            }
            else
            {
                SelectedAbility = RGM.GetRandomValue(N_Abilities.Keys.ToList());
                PlayerAbilities.Add(ev.Player, SelectedAbility);
            }

            string ColorPicker()
            {
                Dictionary<string, string> Colors = new Dictionary<string, string>()
                    {
                        { "N", "BDBDBD" },
                        { "R", "FF0000" },
                        { "SR", "FF00FF" },
                        { "SSR", "FFD700" },
                        { "X", "00FFFF" }
                    };

                return Colors[AllAbilities[SelectedAbility].Split('/')[0]];
            }

            ev.Player.ShowHint($"<b><color=#{ColorPicker()}>{PlayerAbilities[ev.Player].Split('/')[0]}</color></b> 특수능력을 획득하셨습니다.\n<size=20>{PlayerAbilities[ev.Player].Split('/')[1]}</size>");

            string[] strings = AllAbilities[PlayerAbilities[ev.Player]].Split('/');
            var modeType = Type.GetType($"RGM.Modes.SpecialAbilities.{strings[0]}{strings[1]}");
            if (modeType != null)
            {
                var modeInstance = Activator.CreateInstance(modeType);
                var onEnabledMethod = modeType.GetMethod("OnEnabled");
                onEnabledMethod?.Invoke(modeInstance, new object[] { ev.Player });
            }

            ev.Item.Destroy();
        }
    }
}
