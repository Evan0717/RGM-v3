using MEC;

namespace RGM.Modes.Abilities.Unique.Scp0492;

[Ability("허기", "인간을 섭취할 때 얻는 회복량이 80HP 추가되고, 회복량 만큼 최대 체력이 증가합니다.", AbilityCategory.Common, AbilityType.COMMON_SCP0492_HUNGER, RoleAbility.Scp0492)]
public class Hurger : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp0492.ConsumingCorpse += OnConsumingCorpse;
    }
    
    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp0492.ConsumingCorpse -= OnConsumingCorpse;
    }

    public void OnConsumingCorpse(Exiled.Events.EventArgs.Scp0492.ConsumingCorpseEventArgs ev)
    {
        if (ev.Player != Owner || !ev.IsAllowed)
            return;

        Timing.CallDelayed(0.1f, () =>
        {
            ev.Player.MaxHealth += 80;
            ev.Player.Health += 80;
        });
    }
}
