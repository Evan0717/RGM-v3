namespace RGM.Modes.Abilities.Unique.NTF;

[Ability("산업재해보상보험", "보험 능력을 3개 얻습니다.", AbilityCategory.NTF, AbilityType.NTF_INDUSTRIALACCIDENTINSURANCE)]
public class IndustrialAccidentInsurance : Ability
{
    public override void OnEnabled()
    {
        for (int i = 1; i < 4; i++)
            Owner.AddAbility(AbilityType.NORMAL_INSURANCE);
    }

    public override void OnDisabled()
    {
    }
}
