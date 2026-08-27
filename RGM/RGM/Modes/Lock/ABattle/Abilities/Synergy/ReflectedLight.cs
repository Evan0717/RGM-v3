using Exiled.Events.EventArgs.Player;
using RGM.API.Features;
using RGM.API.DataBases;

namespace RGM.Modes.Abilities.Synergy;

[RequiresAbility(AbilityType.LEGEND_FLASHLIGHT, AbilityType.LEGEND_REFLECTOR)]
[Ability("반사광", "<플래시라이트, 반사경> 빛을 강력하게 모읍니다. [<color=#ffd700>전설</color>] 플래시라이트, [<color=#DEEFED>시너지</color>] 광휘 능력의 효과가 강화됩니다.",
    AbilityCategory.Synergy, AbilityType.SYNERGY_REFLECTEDLIGHT)]
public class ReflectedLight : Ability;