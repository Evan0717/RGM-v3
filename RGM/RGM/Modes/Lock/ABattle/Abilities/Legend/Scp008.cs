using RGM.Modes.Sets.AddScp.Scps;

namespace RGM.Modes.Abilities.Legend;

[Ability("SCP-008", "좀비 전염병, SCP-008로 변경됩니다.", AbilityCategory.Legend, AbilityType.LEGEND_SCP008)]
public class ChangeScp008 : Ability
{
    public override void OnEnabled()
    {
        Scp008.Create(Owner);
    }

    public override void OnDisabled()
    {
    }
}
