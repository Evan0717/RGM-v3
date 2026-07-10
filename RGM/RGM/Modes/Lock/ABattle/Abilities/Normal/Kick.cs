using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Normal;

[Ability("회축", "[ALT]를 눌러 발차기 공격을 가할 수 있습니다. (쿨타임 1초, 중복 불가능)", AbilityCategory.Common, AbilityType.NORMAL_KICK)]
public class Kick : Ability
{
    public static int MeleeCooldown = 0;

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

        if (Tools.TryGetLookPlayer(ev.Player, 4f, out Player player, out RaycastHit? hit))
        {
            if (ev.Player != player && MeleeCooldown <= 0 && HitboxIdentity.IsEnemy(ev.Player.ReferenceHub, player.ReferenceHub))
            {
                float damageCalcu(string pos)
                {
                    switch (pos)
                    {
                        case "Head":
                            return 40f;

                        case "Chest":
                            return 20f;

                        default:
                            return 14f;
                    }
                }

                float damage = damageCalcu(hit.Value.transform.name);

                Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, damage / 20);
                player.Hit(ev.Player, damage);
                ev.Player.Grab();

                MeleeCooldown = 1;

                Timing.CallDelayed(1f, () => MeleeCooldown = 0);
            }
        }
    }
}
