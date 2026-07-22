using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Epic;

[Ability("신성방어", "모든 디버프에 면역을 가집니다.", AbilityCategory.Epic, AbilityType.EPIC_HOLYPROTECTION)]
public class HolyProtection : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.ReceivingEffect += OnReceivingEffect;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.ReceivingEffect -= OnReceivingEffect;
    }
    public void OnReceivingEffect(ReceivingEffectEventArgs ev)
    {
        if (ev.Player != Owner) return;

        var effectType = ev.Effect.GetEffectType();

        if (!EffectManager.IsKeptBuff(effectType))
        {
            Timing.CallDelayed(Timing.WaitForOneFrame, () =>
            {
                ev.Player.DisableEffect(effectType);
            });
        }
    }
}