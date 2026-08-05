using Exiled.Events.EventArgs.Player;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Rare;

[Ability("반창고", "체력이 절반 이하로 줄어들었을 경우 최대 체력의 75%(SCP는 25%)를 즉시 회복합니다. (최대 체력 무시)", AbilityCategory.Rare, AbilityType.RARE_ADHESIVEPLASTER)]
public class AdhesivePlaster : Ability
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

        if (!(ev.Player.Health <= ev.Player.MaxHealth / 2)) return;
        ev.Player.Health += ev.Player.IsScpRole() ? ev.Player.MaxHealth * 0.25f : ev.Player.MaxHealth * 0.75f;

        Owner.AddAbility(AbilityType.DUMMY_USEDADHESIVEPLASTER);
        Owner.AddHint("반창고", $"<color={ABattle.RatingColor["희귀"]}>반창고</color>를 사용하여 체력을 회복했습니다.");
        ev.Player.RemoveAbility(this);
    }
}
