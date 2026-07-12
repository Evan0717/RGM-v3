using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using MEC;
using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Legend;

[Ability("거북 도사", "받는 모든 데미지는 40를 넘을 수 없습니다.(일부 피해 제외)\n능력 발동 시, 2.2초간 무적이 됩니다.", AbilityCategory.Legend, AbilityType.LEGEND_TURTLE)]
public class Turtle : Ability
{
    const float MaxDamage = 40f;
    const float GodModeDuration = 2.2f;

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
        if (ev.Player != Owner || ev.DamageHandler.Type == DamageType.Crushed)
            return;

        if (ev.IsInstantKill)
        {
            ev.IsAllowed = false;
            ev.Player.Hurt(MaxDamage, ev.DamageHandler.Type);

            ActivateGodMode();
            return;
        }

        if (ev.DamageHandler.Damage > MaxDamage) {
            ev.DamageHandler.Damage = MaxDamage;

            ActivateGodMode();
        }
    }

    private void ActivateGodMode()
    {
        if (GodModePlayers.Contains(Owner))
            return;

        GodModePlayers.Add(Owner);
        Timing.CallDelayed(GodModeDuration, () =>
        {
            if (GodModePlayers.Contains(Owner))
                GodModePlayers.Remove(Owner);
        });
    }
}