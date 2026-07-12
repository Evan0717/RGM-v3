using Exiled.Events.EventArgs.Player;
using PlayerStatsSystem;

namespace RGM.Modes.Abilities.Legend;

//[Ability("솔져: 76", "당신의 데미지는 절반으로 줄어들지만 에임이 자동으로 보정됩니다.", AbilityCategory.Legend, AbilityType.LEGEND_AIMHACK)]
public class AimHack : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Shooting += OnShooting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Shooting -= OnShooting;
    }

    public void OnShooting(ShootingEventArgs ev)
    {
        if (ev.Player == Owner)
        {
            if (ev.ClaimedTarget != null)
            {
                ev.IsAllowed = false;

                ev.ClaimedTarget.Hurt(new ScpDamageHandler(ev.Player.ReferenceHub, ev.Firearm.Damage / 2, DeathTranslations.Recontained));

                ev.Player.ShowHitMarker();
            }
        }
    }
}
