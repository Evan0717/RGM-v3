using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.Scp3114;

[Ability("반블럭", "데미지를 입으면 일시적으로 이동 속도가 증가합니다.", AbilityCategory.Scp3114, AbilityType.SCP3114_HALFBLOCK)]
public class HalfBlock : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
    }

    public void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        ev.Player.AddEffect(EffectType.MovementBoost, 2, 3);
    }
}
