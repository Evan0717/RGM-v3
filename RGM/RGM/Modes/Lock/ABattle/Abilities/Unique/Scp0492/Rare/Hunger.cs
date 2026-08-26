using MEC;

namespace RGM.Modes.Abilities.Unique.Scp0492.Rare;

[Ability("허기",
    "치료 효율이 100% 증가되고, 시체 섭취 시 마다 치료 효율과 최대 HP가 20% 증가합니다.",
    AbilityCategory.Rare, AbilityType.RARE_SCP0492_HUNGER, RoleAbility.Scp0492)]
public class Hunger : Ability
{
    private float _additionalHealing;
    private float _maximumHealthIncrease;

    public override void OnEnabled()
    {
        _additionalHealing = 100f;
        _maximumHealthIncrease = Owner.MaxHealth * 0.2f;
        Exiled.Events.Handlers.Scp0492.ConsumingCorpse += OnConsumingCorpse;
    }
    
    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp0492.ConsumingCorpse -= OnConsumingCorpse;
    }

    private void OnConsumingCorpse(Exiled.Events.EventArgs.Scp0492.ConsumingCorpseEventArgs ev)
    {
        if (ev.Player != Owner || !ev.IsAllowed)
            return;

        float additionalHealing = _additionalHealing;
        _additionalHealing += 20f;

        Timing.CallDelayed(0.1f, () =>
        {
            Owner.MaxHealth += _maximumHealthIncrease;
            Owner.Health += additionalHealing;
        });
    }
}
