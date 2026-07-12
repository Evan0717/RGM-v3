using Exiled.API.Enums;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;

namespace RGM.Modes.Abilities.Unique.Scp939;

//[Ability("그 시절 댕댕이", "피격 시 3초간 이동 속도가 30% 증가합니다.", AbilityCategory.Scp939, AbilityType.SCP939_HUGME)]
public class HugMe : Ability
{
    public override void OnEnabled()
    {
        
    }

    public override void OnDisabled()
    {
        
    }

    public void OnHurting(HurtingEventArgs ev)
    {
        byte intensity = ev.Player.GetEffect(EffectType.MovementBoost).Intensity;
        float duration = ev.Player.GetEffect(EffectType.MovementBoost).Duration;
    }
}
