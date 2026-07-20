namespace RGM.Modes.Abilities.Unique.Scp079;

// [Ability("자폭 시퀸스", "즉시 핵 가동을 실시합니다. 이 핵은 중지할 수 없습니다.", AbilityCategory.Legend, AbilityType.LEGEND_SCP079_STARTWARHEAD, RoleAbility.Scp079)]
public class StartWarhead : Ability
{
    public override void OnEnabled()
    {
        DeadmanSwitch.StartWarhead();
    }

    public override void OnDisabled()
    {
    }
}
