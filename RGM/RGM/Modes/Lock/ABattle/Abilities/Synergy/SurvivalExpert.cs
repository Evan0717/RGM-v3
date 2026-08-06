using RGM.API.Features;

namespace RGM.Modes.Abilities.Synergy;

[RequiresAbility(AbilityType.NORMAL_INSURANCE, AbilityType.DUMMY_DOPAMINERELEASED, AbilityType.EPIC_SURVIVOR)]
[Ability("생존 전문가", "<보험, 방출된 도파민, 구사일생> 즉시 500HP를 얻습니다. (최대 체력 반영, SCP진영 획득 시 3배수 보정)", AbilityCategory.Synergy, AbilityType.SYNERGY_SURVIVALEXPERT)]
public class SurvivalExpert : Ability
{
    private const float Health = 500f;
    private float _additionHealth;
    
    public override void OnEnabled()
    {
        _additionHealth = Owner.IsScpRole() ? Health * 3 : Health;
        Owner.MaxHealth += _additionHealth;
        Owner.Health += _additionHealth;
    }

    public override void OnDisabled()
    {
    }
}
