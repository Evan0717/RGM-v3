namespace RGM.Modes.Abilities.Synergy;

[RequiresAbility(AbilityType.NORMAL_EXCHANGE, AbilityType.RARE_UPGRADE, AbilityType.RARE_ALCHEMY, AbilityType.NORMAL_RANDOMBOX)]
[Ability("암시장",
    "<교환, 강화, 랜덤박스, 연금>\n다른 진영의 전용 능력이 능력 선택창에 나타날 수 있습니다.",
    AbilityCategory.Synergy,
    AbilityType.SYNERGY_BLACKMARKET)]
public class BlackMarket : Ability;
