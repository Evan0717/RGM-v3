using RGM.Modes.Sets.AddScp.Scps;

namespace RGM.Modes.Abilities.Legend;

[Ability("SCP-457", "불타는 남자, SCP-457로 변경됩니다.", AbilityCategory.Legend, AbilityType.LEGEND_SCP457)]
public class ChangeScp457 : Ability
{
    public override void OnEnabled()
    {
        Scp457.Create(Owner);
    }

    public override void OnDisabled()
    {
    }
}
