using Exiled.Events.EventArgs.Player;

namespace RGM.Modes.Abilities.Rare;

[Ability("흡혈귀", "상대에게 입힌 피해량의 25%만큼 AHP를 받습니다.", AbilityCategory.Rare, AbilityType.RARE_VAMPIRE)]
public class Vampire : Ability
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
        if (ev.Attacker == null || ev.Attacker != Owner || !HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub))
            return;

        ev.Attacker.AddAhp((25 * ev.DamageHandler.Damage / 100));
    }
}
