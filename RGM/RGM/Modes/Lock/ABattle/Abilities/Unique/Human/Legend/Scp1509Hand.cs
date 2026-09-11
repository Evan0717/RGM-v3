using Exiled.Events.EventArgs.Player;
using RGM.Modes.Sets.AddScp.Scps;

namespace RGM.Modes.Abilities.Unique.Human.Legend;

/*[Ability("마체테 클로", "처치한 대상자를 자신의 진영으로 즉시 변경시킵니다.", 
    AbilityCategory.Legend, AbilityType.LEGEND_HUMAN_SCP1509Hand, RoleAbility.Human)]*/
public class ChangeScp1509Hand : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Died += OnDied;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Died -= OnDied;
    }

    void OnDied(DiedEventArgs ev)
    {
        if (ev.Attacker != null && ev.Attacker == Owner)
        {
            ev.Player.Role.Set(ev.Attacker.Role.Type);
        }
    }
}
