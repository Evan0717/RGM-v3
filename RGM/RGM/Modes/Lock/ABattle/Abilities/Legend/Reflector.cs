namespace RGM.Modes.Abilities.Legend;

[Ability("반사경", "능력을 획득하면 33% 확률로 동일한 능력을 하나 더 얻습니다. (연쇄 작용 가능)", AbilityCategory.Legend, AbilityType.LEGEND_REFLECTOR)]
public class Reflector : Ability
{
    public override void OnEnabled()
    {
    }

    public override void OnDisabled()
    {
    }
}
