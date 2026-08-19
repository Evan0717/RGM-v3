namespace RGM.Modes.Abilities.Synergy;

[RequiresAbility(AbilityType.RARE_BULLSEYE,
    AbilityType.EPIC_SHARPEYES,
    AbilityType.EPIC_TURTLE,
    AbilityType.EPIC_HOLYPROTECTION,
    AbilityType.EPIC_FALLENKINGSSWORD,
    AbilityType.EPIC_CONTEXPERT,
    AbilityType.EPIC_RAMBO,
    AbilityType.SYNERGY_SURVIVALEXPERT)]
/*[Ability("저거너트",
    """
    <불스아이, 샤프 아이즈, 거북 도사, 신성방어, 몰락한 왕의 검, 격리 전문가, 람보, 구사일생, 방출된 도파민, 보험>
    모든 조건은 갖춰졌습니다. 이제 시설을 점령할 일만 남았습니다.
    """,
    AbilityCategory.Synergy,
    AbilityType.SYNERGY_JUGGERNAUT)]*/

public class Juggernaut : Ability
{
    public override void OnEnabled()
    {
        
    }

    public override void OnDisabled()
    {
        
    }
    
    /*
     * 저거너트 구현
     *
     * 1. Role을 Tutorial로 변경
     * 2. 
     */
}