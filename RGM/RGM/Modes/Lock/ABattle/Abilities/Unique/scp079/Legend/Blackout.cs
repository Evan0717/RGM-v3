using Exiled.API.Features;

namespace RGM.Modes.Abilities.Unique.Scp079.Legend;

[Ability("블랙아웃", "시설 전체가 정전됩니다.", AbilityCategory.Legend, AbilityType.LEGEND_SCP079_BLACKOUT, RoleAbility.Scp079)]
public class Blackout: Ability
{
    public override void OnEnabled()
    {
        foreach (var room in Room.List)
        {
           room.TurnOffLights();
        }
    }

    public override void OnDisabled()
    {

    }
}