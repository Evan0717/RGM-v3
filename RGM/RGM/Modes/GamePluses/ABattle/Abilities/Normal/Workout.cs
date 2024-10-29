namespace RGM.Modes.Abilities.Normal;

[Ability("운동", "25%만큼 최대 체력을 추가합니다.", AbilityCategory.Common, AbilityType.NORMAL_WORKOUT)]
public class Workout : Ability
{
    private float originalMaxHealth;

    // 능력 활성화 시 실행
    public override void OnEnabled()
    {
        originalMaxHealth = Owner.MaxHealth;
        Owner.MaxHealth += (int)(Owner.MaxHealth * 0.25f);
        Owner.Health = Owner.MaxHealth;
    }

    // 능력 비활성화 시 실행
    public override void OnDisabled()
    {
        Owner.MaxHealth = originalMaxHealth;
    }
}