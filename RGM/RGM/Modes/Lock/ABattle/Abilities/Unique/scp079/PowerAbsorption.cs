using Exiled.API.Enums;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Scp079;

namespace RGM.Modes.Abilities.Unique.Scp079;

[Ability("전력 흡수", "[핑 -> 레일건]ㅣ전력을 50 얻습니다.", AbilityCategory.Rare, AbilityType.RARE_SCP079_POWERABSORPTION, RoleAbility.Scp079)]
public class PowerAbsorption : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp079.Pinging += OnPinging;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp079.Pinging -= OnPinging;
    }

    public void OnPinging(PingingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        if (ev.Type == PingType.MicroHid)
        {
            if (ev.Player.Role is Scp079Role scp079)
                scp079.Energy += 50;
        }
    }
}
