using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.Scp106;

[Ability("사냥감 모색", "공격 성공 후 일시적으로 속도가 4% 증가합니다. (이 효과는 중첩됩니다.)", AbilityCategory.Common, AbilityType.COMMON_SCP106_HUNTINGPREY, RoleAbility.Scp106)]
public class HurtingPrey : Ability
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
        if (ev.Attacker == null || ev.Attacker != Owner)
            return;

        ev.Attacker.AddEffect(EffectType.MovementBoost, 4, 3);
    }
}
