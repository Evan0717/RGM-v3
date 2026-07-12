using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using RGM.Variables;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UserSettings.ServerSpecific;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Lock, ModeType.Rank)]
    public class Rank : Mode
    {
        public override string Name => "경쟁전";
        public override string Description => "직업별로 변칙성, 가젯, 기어를 설정하세요.";
        public override string Detail =>
"""
모든 능력들은 스폰 후, 30초 뒤에 적용됩니다.

[ESC] -> [Settings] -> [Server-specific]
""";
        public override string Color => "ea524c";

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            RankInfo.RankAbilities.Clear();

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var abilityAttribute = type.GetCustomAttribute<RankAbilityAttribute>();

                if (abilityAttribute == null)
                    continue;

                if (!typeof(RankAbility).IsAssignableFrom(type))
                    continue;

                RankInfo.RankAbilities.Add(abilityAttribute.Type, new RankAbilityData
                {
                    Type = type,
                    Name = abilityAttribute.Name,
                    Description = abilityAttribute.Description,
                    Emoji = abilityAttribute.Emoji,
                    RankAbilityType = abilityAttribute.Type,
                    RankCategory = abilityAttribute.RankCategory,
                    RankAbilityCategory = abilityAttribute.RankAbilityCategory
                });
            }

            Exiled.Events.Handlers.Player.Verified += OnVerified;
            
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            RankSetting.Init();

            ServerSpecificSettingsSync.ServerOnSettingValueReceived += RankSetting.OnSSInput;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;

            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= RankSetting.OnSSInput;

            Timing.KillCoroutines(_onModeStarted);
        }

        IEnumerator<float> OnModeStarted()
        {
            foreach (var p in Player.List)
                Verified(p);

            yield return Timing.WaitForSeconds(Variable.EnabledModeList.Select(x => x.Data.Type).Contains(ModeType.TFT) ? 12 : 0);

            Tools.PlayGlobalAudio("RankCountdown", 1.5f);

            for (int i = 0; i < 20; i++)
            {
                foreach (var player in Player.List)
                {
                    player.AddBroadcast(1, $"<size=30>적용까지 <size=50><b>{20 - i}</b></size>초</size>\n" +
                        $"<size=20>[ESC] -> [Settings] -> [Server-specific]ㅣ" +
                        $"<color={RankAbilityCategory.변칙성.GetColor()}>변칙성</color>, <color={RankAbilityCategory.가젯.GetColor()}>가젯</color>, <color={RankAbilityCategory.기어_메인.GetColor()}>기어</color>를 미리 설정해두세요.</size>");

                    player.AddEffect(EffectType.Ensnared, 1, 1);
                    player.AddEffect(EffectType.HeavyFooted, 100, 1);
                    player.AddEffect(EffectType.Blinded, 55, 1);
                }

                yield return Timing.WaitForSeconds(1);
            }

            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;

            foreach (var player in RankInfo.PlayerRankSettingAbilities.Keys.ToList())
            {
                if (player is null)
                    continue;

                player.ClearEffect();

                RankCategory rankCategory = player.GetRankCategory();

                if (!RankInfo.PlayerRankSettingAbilities.TryGetValue(player, out var playerSettings) ||
                    playerSettings is null)
                    continue;

                List<RankAbilityType> list = new();

                if (playerSettings.TryGetValue(rankCategory, out List<RankAbilityType> roleList) && roleList is not null)
                    list.AddRange(roleList);

                if (playerSettings.TryGetValue(RankCategory.공통, out List<RankAbilityType> commonList) && commonList is not null)
                {
                    foreach (var commonAbility in commonList)
                    {
                        if (!list.Contains(commonAbility))
                            list.Add(commonAbility);
                    }
                }

                if (list.Count == 0)
                    continue;

                foreach (var ability in list)
                {
                    var data = ability.GetData();
                    if (data is null)
                    {
                        Log.Warn($"Rank ability data not found for {ability}.");
                        continue;
                    }

                    RankBattle.AddRankAbility(player, ability);
                }
            }
        }

        void OnVerified(VerifiedEventArgs ev)
        {
            Verified(ev.Player);
        }
        
        void Verified(Player player)
        {
            Timing.RunCoroutine(RankBattle.UpgradeDisplay(player));
        }

        void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.NewRole.IsAlive())
            {
                Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                {
                    RankBattle.Reset(ev.Player);

                    if (ev.NewRole.IsAlive())
                    {
                        IEnumerator<float> enumerator()
                        {
                            for (int i = 0; i < 20; i++)
                            {
                                ev.Player.AddBroadcast(1, $"<size=30>적용까지 <size=50><b>{20 - i}</b></size>초</size>\n" +
                                    $"<size=20>[ESC] -> [Settings] -> [Server-specific]ㅣ" +
                                    $"<color={RankAbilityCategory.변칙성.GetColor()}>변칙성</color>, <color={RankAbilityCategory.가젯.GetColor()}>가젯</color>, <color={RankAbilityCategory.기어_메인.GetColor()}>기어</color>를 미리 설정해두세요.</size>");

                                yield return Timing.WaitForSeconds(1);
                            }

                            Variable.PlayersAudio[ev.Player].TryPlay("RankStart", 1.5f);

                            RankCategory rankCategory = ev.Player.GetRankCategory();

                            if (!RankInfo.PlayerRankSettingAbilities.TryGetValue(ev.Player, out var playerSettings) ||
                                playerSettings is null)
                                yield break;

                            List<RankAbilityType> list = new();

                            if (playerSettings.TryGetValue(rankCategory, out List<RankAbilityType> roleList) && roleList is not null)
                                list.AddRange(roleList);

                            if (playerSettings.TryGetValue(RankCategory.공통, out List<RankAbilityType> commonList) && commonList is not null)
                            {
                                foreach (var commonAbility in commonList)
                                {
                                    if (!list.Contains(commonAbility))
                                        list.Add(commonAbility);
                                }
                            }

                            if (list.Count == 0)
                                yield break;

                            foreach (var ability in list)
                            {
                                var data = ability.GetData();
                                if (data is null)
                                {
                                    Log.Warn($"Rank ability data not found for {ability}.");
                                    continue;
                                }

                                RankBattle.AddRankAbility(ev.Player, ability);
                            }
                        }

                        Timing.RunCoroutine(enumerator());
                    }
                });
            }
        }

        void OnRoundEnded(RoundEndedEventArgs ev)
        {
            IEnumerable<Player> players = PlayerManager.List.Where(x => x.IsAlive && !x.IsNPC);

            if (players.Count() == 1)
                Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 5));

            else if (players.Count() > 1)
                Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 1));
        }
    }
}
