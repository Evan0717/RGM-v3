using Exiled.Events.EventArgs.Player;

namespace RGM.Modes.Abilities.Normal;

[Ability("단련", "공격력이 10%p 추가됩니다.", AbilityCategory.Normal, AbilityType.NORMAL_TRAINING)]
public class Training : Ability
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
        if (ev.Attacker != Owner)
            return;

        if (ABattle.Instance.GetAbility(Owner, AbilityType.NORMAL_TRAINING) != this)
            return;

        ev.DamageHandler.Damage *= 1.0f + 0.1f * Owner.AbilityCount(AbilityType.NORMAL_TRAINING);
    }
}
