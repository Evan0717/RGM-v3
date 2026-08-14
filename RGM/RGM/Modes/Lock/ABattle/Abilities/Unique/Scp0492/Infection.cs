using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.Scp0492;

[Ability("감염", "사망시키면 같은 진영으로 만듭니다.", AbilityCategory.Common, AbilityType.NORMAL_SCP0492_INFECTION, RoleAbility.Scp0492)]
public class Infection : Ability
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
        if (ev.Attacker == null || ev.Attacker != Owner)
            return;

        ev.Player.Role.Set(ev.Attacker.IsScpRole() ? RoleTypeId.Scp0492 : ev.Attacker.Role.Type, RoleSpawnFlags.None);

        ev.Player.AddHint("감염", $"{ev.Attacker.DisplayNickname}에 의해 감염되었습니다.", 5);
    }
}
