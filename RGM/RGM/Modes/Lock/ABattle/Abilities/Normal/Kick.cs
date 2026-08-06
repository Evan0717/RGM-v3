using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Normal;

[Ability("회축", "[ALT]를 눌러 발차기 공격을 가할 수 있습니다. (쿨타임 1초)", AbilityCategory.Common, AbilityType.NORMAL_KICK)]
public class Kick : Ability
{
    private const float Meleedamage = 20f;
    private static int _meleeCooldown;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.TogglingNoClip -= OnTogglingNoClip;
    }

    public void OnTogglingNoClip(TogglingNoClipEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        if (ev.Player.IsCaptured(out Player None))
            return;

        if (ev.Player.TryGetLookPlayer(4.7f, out Player player, out RaycastHit? hit))
        {
            if (ev.Player != player && _meleeCooldown <= 0 && HitboxIdentity.IsEnemy(ev.Player.ReferenceHub, player.ReferenceHub))
            {
                float DamageCalcu(string pos)
                {
                    switch (pos)
                    {
                        case "Head":
                            return Meleedamage * 2f;

                        case "Chest":
                            return Meleedamage;

                        default:
                            return Meleedamage * 0.7f;
                    }
                }

                float damage = DamageCalcu(hit?.transform.name) * Owner.AbilityCount(AbilityType.NORMAL_KICK);

                Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, damage / Meleedamage);
                player.Hit(ev.Player, damage);
                ev.Player.Grab();

                _meleeCooldown = 1;

                Timing.CallDelayed(1f, () => _meleeCooldown = 0);
            }
        }
    }
}
