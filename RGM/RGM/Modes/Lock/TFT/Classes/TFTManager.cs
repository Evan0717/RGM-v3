using DAONTFT.Core.TFT;
using Exiled.API.Features;
using HintServiceMeow.Core.Enum;
using RGM.API.Features;
using System.Collections.Generic;
using System.Linq;
using static DAONTFT.Core.Variables.Base;
using Hint = HintServiceMeow.Core.Models.Hints.Hint;

namespace DAONTFT.Core.Classes
{
    public class Upgrade
    {
        public float X { get; set; }
        public string Emoji { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TFTAbilityType Type { get; set; }
        public TFTAbilityLevel Level { get; set; }
    }

    public static class TFTManager
    {
        public static List<Hint> GetUpgradeDisplay(Player player, List<Upgrade> upgrades, int x = 0)
        {
            List<Hint> hints = new();

            Hint get(string text, float x, float y)
            {
                Hint o = new Hint
                {
                    Text = text,
                    Id = "증강",
                    XCoordinate = x,
                    YCoordinate = y,
                    Alignment = HintAlignment.Center
                };

                return o;
            }

            Hint desc1 = get("<size=50>증강 선택</size>", 0, 150 + x);
            Hint desc2 = get("<size=20><i><color=#bababacc>증강을 선택하려면 콘솔(~ 또는 `)을 열고\n\".(번호)\" 명령어를 입력하십시오. (ex. .1 .2 .3)</color>\n\n리롤하려면 \".r(번호)\"를 입력하십시오.</i></size>", 0, 900 + x);

            hints.AddRange(new[] { desc1, desc2 });

            foreach (var upgrade in upgrades)
            {
                Hint o = get($"<color={upgrade.Level.GetColor()}><size=170>○</size></color>", upgrade.X, 350 + x);
                Hint emoji = get($"<color={upgrade.Level.GetColor()}><size=100>{upgrade.Emoji}</size></color>", upgrade.X, 350 + x);
                Hint title = get($"<size=32><b>{Tools.InsertBreaks(upgrade.Title, 12)}</b></size>", upgrade.X, 500 + x);
                Hint description = get($"<size=20>{Tools.InsertBreaks(upgrade.Description, 20)}</size>", upgrade.X, 610 + x);
                Hint chance = get($"<size=15>리롤 가능한 횟수: {Selections[player].First(x => x.Key == upgrade.Type).Value}</size>", upgrade.X, 750 + x);
                Hint box = get($"""
<size=180><color={upgrade.Level.GetColor()}>
┌─┐   


└─┘   
</color></size>
""", upgrade.X, 430 + x);
                hints.AddRange(new[] { o, emoji, title, description, chance, box });
            }  

            return hints;
        }
    }
}
