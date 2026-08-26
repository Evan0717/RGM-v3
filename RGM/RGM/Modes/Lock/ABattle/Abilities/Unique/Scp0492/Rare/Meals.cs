namespace RGM.Modes.Abilities.Unique.Scp0492.Rare;

[Ability("급식", "최대 HP가 100% 증가합니다.",
    AbilityCategory.Rare, AbilityType.RARE_SCP0492_MEALS, RoleAbility.Scp0492)]
public class Meals : Ability
{
    public override void OnEnabled()
    {
        Owner.MaxHealth *= 2f;
        Owner.Health *= 2f;
    }
}
