using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;
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
            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;

            RankSetting.Init();

            ServerSpecificSettingsSync.ServerOnSettingValueReceived += RankSetting.OnSSInput;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;

            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= RankSetting.OnSSInput;

            Timing.KillCoroutines(_onModeStarted);
        }

        IEnumerator<float> OnModeStarted()
        {
            foreach (var p in Player.List)
                Verified(p);

            Tools.PlayGlobalAudio("RankCountdown", 1.5f);

            for (int i = 0; i < 20; i++)
            {
                foreach (var player in Player.List)
                {
                    player.AddBroadcast(1, $"<size=30>시작까지 <size=50><b>{20 - i}</b></size>초</size>\n" +
                        $"<size=20>[ESC] -> [Settings] -> [Server-specific]ㅣ" +
                        $"<color={RankAbilityCategory.변칙성.GetColor()}>변칙성</color>, <color={RankAbilityCategory.가젯.GetColor()}>가젯</color>, <color={RankAbilityCategory.기어.GetColor()}>기어</color>를 미리 설정해두세요.</size>");

                    player.AddEffect(EffectType.Ensnared, 1, 1);
                    player.AddEffect(EffectType.HeavyFooted, 100, 1);
                    player.AddEffect(EffectType.Blinded, 55, 1);
                }

                yield return Timing.WaitForSeconds(1);
            }

            foreach (var player in RankInfo.PlayerRankSettingAbilities.Keys.ToList())
            {
                if (player is null)
                    continue;

                RankCategory rankCategory = player.GetRankCategory();

                if (!RankInfo.PlayerRankSettingAbilities.TryGetValue(player, out var playerSettings) ||
                    playerSettings is null ||
                    !playerSettings.TryGetValue(rankCategory, out List<RankAbilityType> list) ||
                    list is null)
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

                    player.AddBroadcast(3, $"<size=20>{ability.ToString()}</size>");
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
            if (ev.Player.IsDead || ev.NewRole.IsDead())
            {
                Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                {
                    RankBattle.Reset(ev.Player);
                });
            }
        }
    }
}
