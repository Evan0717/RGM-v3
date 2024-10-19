using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using UnityEngine;
using PlayerRoles;

using static RGM.Modes.ABattleVariables.Abilities;
using static RGM.Modes.ABattleVariables.AbiltiyManagers;
using static RGM.Modes.ABattleVariables.MainManagers;
using static RGM.Modes.ABattleVariables.Cooldowns;
using static RGM.Modes.ABattleVariables.Serials;

namespace RGM.Modes.ABattleFunctions
{
    public static class MainManagers
    {
        public static string ColorFormat(string text)
        {
            return text.Replace("[시너지]", $"<color={RatingColor["시너지"]}>[시너지]</color>")
                        .Replace("[전용]", $"<color={RatingColor["전용"]}>[전용]</color>")
                        .Replace("[신화]", $"<color={RatingColor["신화"]}>[신화]</color>")
                        .Replace("[전설]", $"<color={RatingColor["전설"]}>[전설]</color>")
                        .Replace("[영웅]", $"<color={RatingColor["영웅"]}>[영웅]</color>")
                        .Replace("[희귀]", $"<color={RatingColor["희귀"]}>[희귀]</color>")
                        .Replace("[일반]", $"<color={RatingColor["일반"]}>[일반]</color>");
        }

        public static int DuplicateCount(Player player, string abilityName)
        {
            return PlayerAbilities[player].Count(a => a == abilityName);
        }

        public static void ShowStatus(Player player)
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
                string abilitiesText = string.Join(", ", PlayerAbilities[player].GroupBy(x => x).Select(g => g.Count() > 1 ? $"{g.Key} ({g.Count()})" : g.Key).ToList());
                abilitiesText = ColorFormat(abilitiesText);

                player.ShowHint($"<align=left><b><size=25>보유 업그레이드</size></b>\n<size=20>{abilitiesText}</size></align>", 1.2f);
            }
        }
    }
}
