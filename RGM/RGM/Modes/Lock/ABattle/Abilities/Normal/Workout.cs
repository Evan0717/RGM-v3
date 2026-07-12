using RGM.API.Features;

namespace RGM.Modes.Abilities.Normal;

[Ability("운동", "30만큼 최대 체력을 추가합니다. (SCP는 8배의 보너스를 받습니다.)", AbilityCategory.Common, AbilityType.NORMAL_WORKOUT)]
public class Workout : Ability
{
    private float _additionHealth;

    public override void OnEnabled()
    {
        _additionHealth = Owner.IsScpRole() ? 30*8 : 30;
        Owner.MaxHealth += _additionHealth;
        Owner.Health += _additionHealth;
    }

    public override void OnDisabled()
    {
        Owner.MaxHealth -= _additionHealth;
    }
}