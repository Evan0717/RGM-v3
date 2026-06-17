using Exiled.API.Features.DamageHandlers;
using Exiled.Events.EventArgs.Player;
using LabApi.Events.Arguments.PlayerEvents;

namespace RGM.Modes.Abilities.Epic;

[Ability("불스아이", "헤드샷 데미지 배율이 60% 증가합니다.", AbilityCategory.Epic, AbilityType.EPIC_BULLSEYE)]
public class BullsEye : Ability
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
        if (ev.Attacker != Owner || ev.DamageHandler.CustomBase is not FirearmDamageHandler damageHandler)
            return;

        if (damageHandler.Hitbox == HitboxType.Headshot)
            damageHandler.Damage *= 1.8f;
    }
}