using System;
using Exiled.API.Enums;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Scp079;

namespace RGM.Modes.Abilities.Unique.Scp079.Rare;

[Ability("전력 흡수", "[핑 -> 레일건]ㅣ전력을 50 얻습니다. [핑 -> 발전기] | 전력을 20 얻습니다.", AbilityCategory.Rare, AbilityType.RARE_SCP079_POWERABSORPTION, RoleAbility.Scp079)]
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

    private void OnPinging(PingingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        switch (ev.Type)
        {
            case PingType.MicroHid:
            {
                if (ev.Player.Role is Scp079Role scp079)
                    scp079.Energy += 50;
                break;
            }
            case PingType.Generator:
            {
                if (ev.Player.Role is Scp079Role scp079)
                    scp079.Energy += 20;
                break;
            }
            case PingType.Projectile:
            case PingType.Human:
            case PingType.Elevator:
            case PingType.Door:
            case PingType.Default:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
