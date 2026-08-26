using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;

namespace RGM.Modes.Abilities.Unique.Scp049.Rare;

[Ability("능수능란", "처치 시 대상을 즉시 049-2로 만듭니다.", AbilityCategory.Rare, AbilityType.RARE_SCP049_PROFICIENCY, RoleAbility.Scp049)]
public class Proficiency : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Died += OnDied;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Died -= OnDied;
    }

    private void OnDied(DiedEventArgs ev)
    {
        if (ev.Attacker != Owner || ev.Player == Owner)
            return;

        Timing.CallDelayed(0.1f, () =>
        {
            ev.Player.Role.Set(RoleTypeId.Scp0492, RoleSpawnFlags.None);
        });
    }
}
