using MEC;

using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Rare;

[Ability("하이패스", "25초 간 무적이 됩니다. (SCP-106을 포함한 일부 공격 제외)", AbilityCategory.Rare, AbilityType.RARE_HYPASS)]
public class Hypass : Ability
{
    public override void OnEnabled()
    {
        GodModePlayers.Add(Owner);

        Timing.CallDelayed(25, () => 
        {
            if (GodModePlayers.Contains(Owner))
                GodModePlayers.Remove(Owner);
        });
    }

    public override void OnDisabled()
    {
    }
}
