using Exiled.API.Extensions;
using Exiled.API.Features;
using PlayerRoles;
using RGM.API.Features;
using RGM.Modes.SubClass;
using SecretAPI.Features.UserSettings;
using System;
using System.Linq;
using UnityEngine;
using static RGM.Variables.Variable;

namespace RGM.UserSettings
{
    public static class MainSetting
    {
        public static CustomHeader Setting { get; } = new("<b>랜덤게임모드</b>");

        public static CustomKeybindSetting ScpCanEquipRandomItem { get; private set; }
        public static CustomButtonSetting SpectatorToNone { get; private set; }
        public static CustomButtonSetting SwitchToSpectator { get; private set; }
        public static CustomTwoButtonSetting MuteBGM { get; private set; }
        public static CustomDropdownSetting Translation { get; private set; }
        public static CustomKeybindSetting UpKey { get; private set; }
        public static CustomKeybindSetting DownKey { get; private set; }
        public static CustomKeybindSetting LeftKey { get; private set; }
        public static CustomKeybindSetting RightKey { get; private set; }
        public static CustomKeybindSetting EnterKey { get; private set; }
        public static CustomKeybindSetting DetailInfoKey { get; private set; }

        public static void Init()
        {
            if (ScpCanEquipRandomItem != null)
                return;

            ScpCanEquipRandomItem = new ScpCanEquipRandomItemSetting();
            SpectatorToNone = new SpectatorToNoneSetting();
            SwitchToSpectator = new SwitchToSpectatorSetting();
            MuteBGM = new MuteBGMSetting();
            Translation = new TranslationSetting();
            UpKey = new UpKeySetting();
            DownKey = new DownKeySetting();
            LeftKey = new LeftKeySetting();
            RightKey = new RightKeySetting();
            EnterKey = new EnterKeySetting();
            DetailInfoKey = new DetailInfoKeySetting();

            CustomSetting.Register(
                ScpCanEquipRandomItem,
                SpectatorToNone,
                SwitchToSpectator,
                MuteBGM,
                Translation,
                UpKey,
                DownKey,
                LeftKey,
                RightKey,
                EnterKey,
                DetailInfoKey);
        }

        private sealed class ScpCanEquipRandomItemSetting : CustomKeybindSetting
        {
            public ScpCanEquipRandomItemSetting()
                : base(12050, "SCP의 아이템 장착ㅣEquipping SCP items", KeyCode.H, allowSpectatorTrigger: false, hint: "SCP가 보유한 아이템 중 무작위로 하나를 장착합니다.\n\nEquip a random item from the SCP's inventory.")
            {
            }

            public override CustomHeader Header => Setting;

            protected override CustomSetting CreateDuplicate() => new ScpCanEquipRandomItemSetting();

            protected override void HandleSettingUpdate()
            {
                if (!IsPressed || KnownOwner == null)
                    return;

                Player player = Player.Get(KnownOwner.ReferenceHub);
                if (!player.IsScpRole())
                    return;

                var candidates = player.Items
                    .Where(x => player.CurrentItem != x)
                    .ToList();

                candidates.Add(null);

                if (candidates.Count == 0)
                    return;

                player.CurrentItem = candidates.GetRandomValue();
            }
        }

        private sealed class SpectatorToNoneSetting : CustomButtonSetting
        {
            public SpectatorToNoneSetting()
                : base(12051, "관전석 <-> 훈련장ㅣSpectator <-> Training ground", "GO!", 0.5f, "관전석에서 훈련장으로 이동합니다.\n• Set 모드 또는 특정 모드에서 사용 불가\n• 사망 후 10초가 지나야 사용 가능\n\nMove from the spectator seats to the training grounds.\n• Not available in Set mode or certain modes.\n• Available 10 seconds after death.")
            {
            }

            public override CustomHeader Header => Setting;

            protected override CustomSetting CreateDuplicate() => new SpectatorToNoneSetting();

            protected override void HandleSettingUpdate()
            {
                if (KnownOwner == null)
                    return;

                Player player = Player.Get(KnownOwner.ReferenceHub);

                if ((CurrentMode == ModeType.None || CurrentMode.GetModeData().Info == ModeInfo.Plus) &&
                    IsNonePlayerAllowed &&
                    (Round.IsLobby || (DateTime.UtcNow - PlayersReport[player.UserId].LastDeath).TotalSeconds >= 10))
                {
                    if (player.IsAlive && NonePlayer.Players.Contains(player))
                    {
                        player.ClearInventory();
                        player.Kill("관전석으로 되돌아갑니다.");
                    }
                    else if (Round.IsLobby ? true : player.IsDead)
                    {
                        NonePlayer.Create(player);
                    }
                    else
                    {
                        PlayersAudio[player].TryPlay("nope");
                    }
                }
                else
                {
                    PlayersAudio[player].TryPlay("nope");
                }
            }
        }

        private sealed class SwitchToSpectatorSetting : CustomButtonSetting
        {
            public SwitchToSpectatorSetting()
                : base(12052, "관전자 <-> 오버워치ㅣSpectator <-> Overwatch", "<->", 0.5f, "관전자와 오버워치 상태를 변경합니다.\n• 사망 후 10초가 지나야 사용 가능\n\nChanges between spectator and Overwatch status.\n• Available 10 seconds after death.")
            {
            }

            public override CustomHeader Header => Setting;

            protected override CustomSetting CreateDuplicate() => new SwitchToSpectatorSetting();

            protected override void HandleSettingUpdate()
            {
                if (KnownOwner == null)
                    return;

                Player player = Player.Get(KnownOwner.ReferenceHub);

                if ((DateTime.UtcNow - PlayersReport[player.UserId].LastDeath).TotalSeconds >= 10)
                {
                    if (player.Role.Type == RoleTypeId.Overwatch)
                    {
                        player.Role.Set(RoleTypeId.Spectator);
                    }
                    else if (player.Role.Type == RoleTypeId.Spectator)
                    {
                        player.Role.Set(RoleTypeId.Overwatch);
                    }
                    else
                    {
                        PlayersAudio[player].TryPlay("nope");
                    }
                }
                else
                {
                    PlayersAudio[player].TryPlay("nope");
                }
            }
        }

        private sealed class MuteBGMSetting : CustomTwoButtonSetting
        {
            public MuteBGMSetting()
                : base(12053, "BGM 음소거ㅣBGM mute", "ON", "OFF", defaultIsB: true, hint: "음악이 유튜브 저작권에 걸릴 것 같다고요? 이 기능을 사용하세요.\n\nAre you worried BGM might be copyrighted by YouTube? Use this feature.")
            {
            }

            public override CustomHeader Header => Setting;

            protected override CustomSetting CreateDuplicate() => new MuteBGMSetting();

            protected override void HandleSettingUpdate()
            {
                if (KnownOwner == null)
                    return;

                Player player = Player.Get(KnownOwner.ReferenceHub);

                if (IsOptionA)
                {
                    if (!MuteBGMPlayers.Contains(player))
                        MuteBGMPlayers.Add(player);
                }
                else if (MuteBGMPlayers.Contains(player))
                {
                    MuteBGMPlayers.Remove(player);
                }
            }
        }

        private sealed class TranslationSetting : CustomDropdownSetting
        {
            public TranslationSetting()
                : base(12054, "번역ㅣTranslation", new[] { "Korean (ko)", "English (en)" }, defaultOptionIndex: Main.Instance.Config.EN ? 1 : 0, hint: "언어의 장벽을 부수려면 이 설정을 사용하세요.\n\nUse this setting to break the language barrier.")
            {
            }

            public override CustomHeader Header => Setting;

            protected override CustomSetting CreateDuplicate() => new TranslationSetting();

            protected override void HandleSettingUpdate()
            {
                if (KnownOwner == null)
                    return;

                Player player = Player.Get(KnownOwner.ReferenceHub);

                TranslatorPlayers[player] = SelectedOption.Split('(')[1].Replace(")", "");
            }
        }

        private sealed class UpKeySetting : CustomKeybindSetting
        {
            public UpKeySetting()
                : base(12055, "위 이동키ㅣUp movement key", KeyCode.UpArrow)
            {
            }

            public override CustomHeader Header => Setting;

            protected override CustomSetting CreateDuplicate() => new UpKeySetting();

            protected override void HandleSettingUpdate()
            {
            }
        }

        private sealed class DownKeySetting : CustomKeybindSetting
        {
            public DownKeySetting()
                : base(12056, "아래 이동키ㅣDown movement key", KeyCode.DownArrow)
            {
            }

            public override CustomHeader Header => Setting;

            protected override CustomSetting CreateDuplicate() => new DownKeySetting();

            protected override void HandleSettingUpdate()
            {
            }
        }

        private sealed class LeftKeySetting : CustomKeybindSetting
        {
            public LeftKeySetting()
                : base(12057, "왼쪽 이동키ㅣLeft movement key", KeyCode.LeftArrow)
            {
            }

            public override CustomHeader Header => Setting;

            protected override CustomSetting CreateDuplicate() => new LeftKeySetting();

            protected override void HandleSettingUpdate()
            {
            }
        }

        private sealed class RightKeySetting : CustomKeybindSetting
        {
            public RightKeySetting()
                : base(12058, "오른쪽 이동키ㅣRight movement key", KeyCode.RightArrow)
            {
            }

            public override CustomHeader Header => Setting;

            protected override CustomSetting CreateDuplicate() => new RightKeySetting();

            protected override void HandleSettingUpdate()
            {
            }
        }

        private sealed class EnterKeySetting : CustomKeybindSetting
        {
            public EnterKeySetting()
                : base(12059, "확인 키ㅣEnter key", KeyCode.KeypadEnter)
            {
            }

            public override CustomHeader Header => Setting;

            protected override CustomSetting CreateDuplicate() => new EnterKeySetting();

            protected override void HandleSettingUpdate()
            {
            }
        }

        private sealed class DetailInfoKeySetting : CustomKeybindSetting
        {
            public DetailInfoKeySetting()
                : base(12060, "자세한 설명 보기ㅣShow detailed info", KeyCode.F1, hint: "현재 모드의 자세한 정보를 확인합니다.")
            {
            }

            public override CustomHeader Header => Setting;

            protected override CustomSetting CreateDuplicate() => new DetailInfoKeySetting();

            protected override void HandleSettingUpdate()
            {
            }
        }
    }
}
