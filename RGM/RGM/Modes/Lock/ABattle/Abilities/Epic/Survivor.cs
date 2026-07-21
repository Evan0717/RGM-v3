using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.DataBases;
using RGM.API.Features;

using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Epic;

[Ability("구사일생", "사망 판정을 받을 경우, 2초간 투명 상태와 무적이 되며, 체력을 20% 회복합니다. (최대 3번)", AbilityCategory.Epic, AbilityType.EPIC_SURVIVOR)]
public class Survivor : Ability
{
    int power = 3;
    bool isEnabled = false;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Dying += OnDying;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Dying -= OnDying;
    }

    public void OnDying(DyingEventArgs ev)
    {
        if (ev.Player != Owner || ABattle.Instance.IsLifeUsed[Owner] || Datas.BlockDamageTypes.Contains(ev.DamageHandler.Type))
            return;

        if (!isEnabled)
        {
            isEnabled = true;

            ev.IsAllowed = false;

            ev.Player.EnableEffect(EffectType.Invisible, 1, 2);
            ev.Player.EnableEffect(EffectType.Ghostly, 1, 2);
            ev.Player.AddEffect(EffectType.MovementBoost, 20, 2);

            GodModePlayers.Add(ev.Player);
            ev.Player.Heal(ev.Player.MaxHealth * 0.2f);
            
            Timing.CallDelayed(2f, () =>
            {
                if (GodModePlayers.Contains(ev.Player))
                    GodModePlayers.Remove(ev.Player);

                isEnabled = false;
            });

            ev.Player.AddHint("구사일생", $"<color={ABattle.RatingColor["영웅"]}>구사일생</color> 능력으로 인해 3초간 죽음을 피합니다. ({power - 1}번 남음)");

            ABattle.Instance.IsLifeUsed[Owner] = true;

            Timing.CallDelayed(Timing.WaitForOneFrame, () =>
            {
                ABattle.Instance.IsLifeUsed[Owner] = false;
            });

            if (power == 1)
                ev.Player.RemoveAbility(this);

            else
                power--;
        }
    }
}
