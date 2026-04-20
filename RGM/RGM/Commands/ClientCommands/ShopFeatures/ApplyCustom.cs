using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using RGM.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RGM.Commands.ClientCommands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ApplyChangeDisplayNickname : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);
            string args = string.Join(" ", arguments).Trim();

            if (args.Count() > 16)
            {
                response = "닉네임은 최대 16자까지만 허용됩니다.\n-";
                return false;
            }

            if (args.Contains(",") || args.Contains("\"") || args.Contains("["))
            {
                response = "닉네임에 허용되지 않는 특수문자(',', '\"', '[')가 포함되어 있습니다.\n-";
                return false;
            }

            if (UsersManager.UsersCache.ContainsKey(player.UserId))
            {
                List<string> uc = UsersManager.UsersCache[player.UserId];

                if (uc[7].Split('/').Contains("커스텀 닉네임"))
                {
                    uc[5] = args == "" ? "0" : args;
                    UsersManager.UsersCache[player.UserId] = uc;

                    response = "닉네임 변경 완료!\n-";

                    UsersManager.SaveUsers();
                    return true;
                }
                else
                {
                    response = "해당 기능은 상점에서 구매할 수 있습니다.\n-";
                    return false;
                }
            }
            else
            {
                response = "플레이어 정보를 찾을 수 없습니다.\n-";
                return false;
            }
        }

        public string Command { get; } = "applychangedisplaynickname";

        public string[] Aliases { get; } = { "acdn", "닉네임" };

        public string Description { get; } = "[RGM] 다른 유저에게 보여지는 이름을 수정합니다.";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class ApplyChangeCustomInfo : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);
            string args = string.Join(" ", arguments).Trim();

            if (args.Count() > 16)
            {
                response = "인포는 최대 16자까지만 허용됩니다.\n-";
                return false;
            }

            if (args.Contains(",") || args.Contains("\"") || args.Contains("["))
            {
                response = "인포에 허용되지 않는 특수문자(',', '\"', '[')가 포함되어 있습니다.\n-";
                return false;
            }

            if (UsersManager.UsersCache.ContainsKey(player.UserId))
            {
                List<string> uc = UsersManager.UsersCache[player.UserId];

                if (uc[7].Split('/').Contains("커스텀 인포"))
                {
                    uc[6] = args == "" ? "0" : args;
                    UsersManager.UsersCache[player.UserId] = uc;

                    response = "인포 변경 완료!\n-";

                    UsersManager.SaveUsers();
                    return true;
                }
                else
                {
                    response = "해당 기능은 상점에서 구매할 수 있습니다.\n-";
                    return false;
                }
            }
            else
            {
                response = "플레이어 정보를 찾을 수 없습니다.\n-";
                return false;
            }
        }

        public string Command { get; } = "인포";

        public string[] Aliases { get; } = { "acci" };

        public string Description { get; } = "[RGM] 다른 유저에게 보여지는 역할 설명을 수정합니다.";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class CustomKeycard : ICommand
    {
        private static readonly Dictionary<string, ItemType> TypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["TaskForce"] = ItemType.KeycardCustomTaskForce,
            ["Site02"] = ItemType.KeycardCustomSite02,
            ["Management"] = ItemType.KeycardCustomManagement,
            ["MetalCase"] = ItemType.KeycardCustomMetalCase,
        };

        private static readonly Dictionary<string, string> NamedColors = new(StringComparer.OrdinalIgnoreCase)
        {
            ["red"] = "#ff0000",
            ["blue"] = "#0000ff",
            ["green"] = "#00ff00",
            ["yellow"] = "#ffff00",
            ["orange"] = "#ffa500",
            ["purple"] = "#800080",
            ["pink"] = "#ff69b4",
            ["white"] = "#ffffff",
            ["black"] = "#000000",
            ["gray"] = "#808080",
            ["grey"] = "#808080",
            ["cyan"] = "#00ffff",
            ["magenta"] = "#ff00ff",
        };

        private static Dictionary<ItemType, string> CustomKeycardCmdFormat { get; set; } = new Dictionary<ItemType, string>()
        {
            {ItemType.KeycardCustomTaskForce,   "{sys_keycard} {string_inv_name} {sys_level1} {sys_level2} {sys_level3} {color_permission} {color_tint} {string_label} {int_serial} {sys_rank}" },
            {ItemType.KeycardCustomSite02,      "{sys_keycard} {string_inv_name} {sys_level1} {sys_level2} {sys_level3} {color_permission} {color_tint} {string_label} {color_label} {string_holder} {int_wear}" },
            {ItemType.KeycardCustomManagement,  "{sys_keycard} {string_inv_name} {sys_level1} {sys_level2} {sys_level3} {color_permission} {color_tint} {string_label} {color_label}" },
            {ItemType.KeycardCustomMetalCase,   "{sys_keycard} {string_inv_name} {sys_level1} {sys_level2} {sys_level3} {color_permission} {color_tint} {string_label} {color_label} {string_holder} {int_serial}" }
        };

        private static HashSet<ItemType> CustomKeycards { get; set; } = new HashSet<ItemType>()
        {
            ItemType.KeycardCustomManagement,
            ItemType.KeycardCustomMetalCase,
            ItemType.KeycardCustomTaskForce,
            ItemType.KeycardCustomSite02,
            ItemType.KeycardChaosInsurgency,
            ItemType.KeycardO5,
            ItemType.SurfaceAccessPass
        };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "";
            Player player = Player.Get(sender);

            if (UsersManager.UsersCache.ContainsKey(player.UserId))
            {
                List<string> uc = UsersManager.UsersCache[player.UserId];

                if (uc[7].Split('/').Contains("커스텀 키카드")) 
                { 
                    // 허용
                }
                else
                {
                    response = "해당 기능은 상점에서 구매할 수 있습니다.";
                    return false;
                }
            }
            else
            {
                response = "해당 기능은 상점에서 구매할 수 있습니다.";
                return false;

            }

            if (!player.IsAlive)
            {
                response = "살아있을 때만 이 명령어를 사용할 수 있습니다.";
                return false;
            }

            if (arguments.Count != 5)
            {
                response = Usage();
                return false;
            }

            string typeArg = arguments.At(0);
            string labelArg = arguments.At(1);
            string permColorArg = arguments.At(2);
            string tintColorArg = arguments.At(3);
            string labelTextColorArg = arguments.At(4);

            if (!TypeMap.TryGetValue(typeArg, out ItemType targetCustomType))
            {
                response = "올바른 커스텀 키카드 타입이 아닙니다.\n" + Usage();
                return false;
            }

            Item item = player.CurrentItem;
            if (item is not Keycard heldKeycard)
            {
                response = "특정 키카드를 반드시 들어주셔야 합니다.";
                return false;
            }

            if (CustomKeycards.Contains(heldKeycard.Type))
            {
                response = "이 키카드는 변형이 불가한 카드입니다.";
                return false;
            }

            string label = (labelArg ?? "").Replace('_', ' ').Trim();
            if (string.IsNullOrWhiteSpace(label))
            {
                response = "라벨(Label) 부분은 비워둘 수 없습니다.";
                return false;
            }

            if (label.Length > 24)
            {
                response = "라벨(Label)은 최대 24자까지만 허용됩니다.";
                return false;
            }

            if (!TryNormalizeColor(permColorArg, out string permHex))
            {
                response = $"잘못된 권한 색상: {permColorArg}";
                return false;
            }
            if (!TryNormalizeColor(tintColorArg, out string tintHex))
            {
                response = $"잘못된 기본 색조 색상: {tintColorArg}";
                return false;
            }
            if (!TryNormalizeColor(labelTextColorArg, out string labelTextHex))
            {
                response = $"잘못된 라벨 텍스트 색상: {labelTextColorArg}";
                return false;
            }

            var (lvl1, lvl2, lvl3) = GetLevelsFromVanillaCard(heldKeycard.Type);

            string invName = label;
            string holder = player.DisplayNickname;

            int wear = 0;
            int serial = MakeSerial(player);
            int rankDetail = 0;

            player.RemoveItem(heldKeycard);

            string generated = BuildCustomKeycardArgs(
                targetCustomType,
                invName,
                label,
                permHex,
                tintHex,
                labelTextHex,
                holder,
                serial,
                wear,
                lvl1, lvl2, lvl3,
                rankDetail
            );

            Server.ExecuteCommand($"/ckeycard {player.Id} {generated}");

            response = $"커스텀 키카드가 만들어졌습니다: [{typeArg}] {label}";
            return true;
        }

        private static string Usage()
        {
            return "나만의 키카드를 만들려면 .keycard (키카드 유형) (라벨) (권한 색상) (기본 색조 색상) (라벨 텍스트 색상)을 입력하세요.\n" +
                    "| 라벨에는 단어 사이에 공백 대신 '_'를 입력하세요.\n" +
                    "| 색상에는 16진수 값 또는 이름을 사용하세요.\n" +
                    "| 키카드 유형은 TaskForce/Site02/Management/MetalCase 입니다.";
        }

        private static bool TryNormalizeColor(string input, out string hexOut)
        {
            hexOut = null;
            if (string.IsNullOrWhiteSpace(input)) return false;

            input = input.Trim();

            if (NamedColors.TryGetValue(input, out string mapped))
            {
                hexOut = mapped;
                return true;
            }

            if (!input.StartsWith("#")) input = "#" + input;

            if (!ColorUtility.TryParseHtmlString(input, out _))
                return false;

            hexOut = input.ToLowerInvariant();
            return true;
        }

        private static string EscapeArg(string s) => (s ?? "").Trim().Replace(' ', '_');

        private static int MakeSerial(Player p)
        {
            unchecked
            {
                int h = 17;
                h = h * 31 + p.Id;
                h = h * 31 + (int)(DateTime.UtcNow.Ticks & 0xFFFF);
                return Math.Abs(h % 1000000) + 1;
            }
        }

        private static string BuildCustomKeycardArgs(
            ItemType customType,
            string invName,
            string label,
            string permColor,
            string tintColor,
            string labelTextColor,
            string holder,
            int serial,
            int wear,
            int level1, int level2, int level3,
            int rankDetail
        )
        {
            if (!CustomKeycardCmdFormat.TryGetValue(customType, out string fmt))
                throw new Exception($"CustomKeycardCmdFormat missing for {customType}");

            string invArg = EscapeArg(invName);
            string labelArg = EscapeArg(label);
            string holderArg = EscapeArg(holder);

            fmt = ReplaceIfContains(fmt, "{sys_keycard}", customType.ToString());
            fmt = ReplaceIfContains(fmt, "{string_inv_name}", invArg);
            fmt = ReplaceIfContains(fmt, "{string_label}", labelArg);
            fmt = ReplaceIfContains(fmt, "{string_holder}", holderArg);

            fmt = ReplaceIfContains(fmt, "{sys_level1}", level1.ToString());
            fmt = ReplaceIfContains(fmt, "{sys_level2}", level2.ToString());
            fmt = ReplaceIfContains(fmt, "{sys_level3}", level3.ToString());

            fmt = ReplaceIfContains(fmt, "{color_permission}", permColor);
            fmt = ReplaceIfContains(fmt, "{color_tint}", tintColor);
            fmt = ReplaceIfContains(fmt, "{color_label}", labelTextColor);

            fmt = ReplaceIfContains(fmt, "{int_serial}", serial.ToString());
            fmt = ReplaceIfContains(fmt, "{int_wear}", wear.ToString());

            fmt = ReplaceIfContains(fmt, "{sys_rank}", rankDetail.ToString());

            return fmt;
        }

        private static string ReplaceIfContains(string src, string token, string value)
            => src.Contains(token) ? src.Replace(token, value) : src;

        private static (int level1, int level2, int level3) GetLevelsFromVanillaCard(ItemType vanilla)
        {
            return vanilla switch
            {
                ItemType.KeycardJanitor => (1, 0, 0),
                ItemType.KeycardZoneManager => (1, 0, 1),
                ItemType.KeycardScientist => (2, 0, 0),
                ItemType.KeycardResearchCoordinator => (2, 0, 1),
                ItemType.KeycardContainmentEngineer => (3, 0, 1),

                ItemType.KeycardGuard => (1, 1, 1),
                ItemType.KeycardMTFPrivate => (2, 2, 1),
                ItemType.KeycardMTFOperative => (2, 2, 2),
                ItemType.KeycardMTFCaptain => (2, 3, 2),

                ItemType.KeycardFacilityManager => (3, 0, 3),

                _ => (0, 0, 0),
            };
        }

        public string Command { get; } = "키카드";
        public string[] Aliases { get; } = { "keycard", "커스텀키카드" };
        public string Description { get; } = "[RGM] 커스텀 키카드를 만들어보세요.";
        public bool SanitizeResponse { get; } = true;
    }
}
