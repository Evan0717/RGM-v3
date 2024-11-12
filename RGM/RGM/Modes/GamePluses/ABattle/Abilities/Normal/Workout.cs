namespace RGM.Modes.Abilities.Normal;

[Ability("운동", "25%만큼 최대 체력을 추가합니다.", AbilityCategory.Common, AbilityType.NORMAL_WORKOUT)]
public class Workout : Ability
{
    private float _originalMaxHealth;
    private float _additionHealth;

    public override void OnEnabled()
    {
        _additionHealth = _originalMaxHealth * 0.25f;
        Owner.MaxHealth += _additionHealth;
        Owner.Health += _additionHealth;
    }

    public override void OnDisabled()
    {
        Owner.MaxHealth -= _additionHealth;
    }
}