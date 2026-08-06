namespace RGM.Modes.Abilities.Rare;

[Ability("하이퍼 바디", "HP가 45% 증가합니다.", AbilityCategory.Rare, AbilityType.RARE_HYPERBODY)]

public class HyperBody : Ability
{
    public override void OnEnabled()
    {
        Owner.MaxHealth *= 1.45f;
        Owner.Health *= 1.45f;
    }
}