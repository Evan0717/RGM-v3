using RGM.Modes.Sets.AddScp.Scps;

namespace RGM.Modes.Abilities.Legend;

[Ability("SCP-966", "잠을 죽이는 자, SCP-966으로 변경됩니다.", AbilityCategory.Legend, AbilityType.LEGEND_SCP966)]
public class ChangeScp966 : Ability
{
    public override void OnEnabled()
    {
        Scp966.Create(Owner);
    }

    public override void OnDisabled()
    {
    }
}
