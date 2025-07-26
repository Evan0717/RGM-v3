using RGM.Modes.Sets.AddScp.Scps;

namespace RGM.Modes.Abilities.Legend;

[Ability("SCP-999", "간지럼 괴물, SCP-999로 변경됩니다.", AbilityCategory.Legend, AbilityType.LEGEND_SCP999)]
public class ChangeScp999 : Ability
{
    public override void OnEnabled()
    {
        Scp999.Create(Owner);
    }

    public override void OnDisabled()
    {
    }
}
