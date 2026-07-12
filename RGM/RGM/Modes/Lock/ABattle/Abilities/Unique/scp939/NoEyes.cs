using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;

namespace RGM.Modes.Abilities.Unique.Scp939;

[Ability("실명", "섬광탄 효과에 면역을 가집니다.", AbilityCategory.Scp939, AbilityType.SCP939_NOEYES)]
public class NoEyes : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.ReceivingEffect += OnReceivingEffect;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.ReceivingEffect -= OnReceivingEffect;
    }

    List<EffectType> _effects =
    [
        EffectType.Blurred,
        EffectType.Deafened,
        EffectType.Flashed
    ];
    public void OnReceivingEffect(ReceivingEffectEventArgs ev)
    {
        if (ev.Player != Owner) return;
        
        if (ev.Effect.GetEffectType() == EffectType.Flashed)
        {
            Timing.CallDelayed(Timing.WaitForOneFrame, () => {
                foreach (var effect in _effects) {
                    ev.Player.DisableEffect(effect);
                }
            });
        }
    }
}
