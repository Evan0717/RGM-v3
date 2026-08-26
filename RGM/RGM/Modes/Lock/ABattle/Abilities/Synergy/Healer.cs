using Exiled.Events.EventArgs.Player;

namespace RGM.Modes.Abilities.Synergy;

[RequiresAbility(AbilityType.NORMAL_HEALGUN, AbilityType.RARE_PHYSICALSTRENGTHENING, AbilityType.RARE_PANACEA)]
[Ability("비숍", "<치유 사제, 만병통치약, 육체 강화> 본인도 치료하고 상대방도 치료하고...", AbilityCategory.Synergy, AbilityType.SYNERGY_HEALER)]
public class Healer : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Healing += OnHealing;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Healing -= OnHealing;
    }

    private void OnHealing(HealingEventArgs ev)
    {
        if (ev.Player == Owner)
            ev.Amount *= 4.5f;
    }
}