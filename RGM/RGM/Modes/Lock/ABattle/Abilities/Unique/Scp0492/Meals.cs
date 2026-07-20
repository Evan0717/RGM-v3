namespace RGM.Modes.Abilities.Unique.Scp0492;

[Ability("급식", "최대 체력이 50% 증가합니다.", AbilityCategory.Common, AbilityType.COMMON_SCP0492_MEALS, RoleAbility.Scp0492)]
public class Meals : Ability
{
    public override void OnEnabled()
    {
        Owner.MaxHealth += Owner.MaxHealth * 0.5f;
    }

    public override void OnDisabled()
    {
    }
}
