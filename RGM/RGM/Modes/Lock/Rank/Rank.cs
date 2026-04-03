using CustomRendering;
using DAONTFT.Core.TFT;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.Events.EventArgs.Player;
using HarmonyLib;
using MEC;
using Mirror;
using MultiBroadcast;
using PlayerRoles;
using ProjectMER.Features;
using Respawning;
using RGM.API.Features;
using RGM.Modes.Abilities.Normal;
using RGM.UserSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UserSettings.ServerSpecific;
using static RGM.Variables.Variable;

namespace RGM.Modes
{
    [Mode(ModeCategory.Private, ModeInfo.Lock, ModeType.Rank)]
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
            yield return Timing.WaitForSeconds(30);

            foreach (var p in Player.List)
                Verified(p);

            foreach (var player in RankInfo.PlayerRankSettingAbilities.Keys.ToList())
            {
                RankCategory rankCategory = player.GetRankCategory();

                List<RankAbilityType> list = RankInfo.PlayerRankSettingAbilities[player][rankCategory];

                foreach (var ability in list)
                {
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
