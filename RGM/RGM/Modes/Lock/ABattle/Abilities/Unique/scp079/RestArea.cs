using System.Linq;
using Exiled.Events.EventArgs.Scp079;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.Scp079;

[Ability("휴게소", "[경험치 획득]ㅣ생존한 SCP의 체력이 획득한 경험치의 10%만큼 회복됩니다.", AbilityCategory.Scp079, AbilityType.SCP079_RESTAREA)]
public class RestArea : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp079.GainingExperience += OnGainingExperience;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp079.GainingExperience -= OnGainingExperience;
    }

    public void OnGainingExperience(GainingExperienceEventArgs ev)
    {
        if (Owner != ev.Player)
            return;

        foreach (var scp in PlayerManager.List.Where(x => x.IsScpRole()))
        {
            if (scp.Health >= scp.MaxHealth)
                continue;

            scp.Health += ev.Amount * 0.1f;
        }
    }
}
