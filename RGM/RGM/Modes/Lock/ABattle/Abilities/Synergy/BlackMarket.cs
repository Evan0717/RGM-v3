namespace RGM.Modes.Abilities.Synergy;

[RequiresAbility(AbilityType.NORMAL_EXCHANGE, AbilityType.RARE_UPGRADE, AbilityType.RARE_COLLECTOR)]
[Ability("암시장",
    "<교환, 강화, 수집가> 다른 진영의 전용 능력이 능력 선택창에 나타날 수 있습니다.",
    AbilityCategory.Synergy,
    AbilityType.SYNERGY_BLACKMARKET)]
public class BlackMarket : Ability;
