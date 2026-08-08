using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.Scp079.Rare;

[Ability("시스템 해킹", "아군 SCP의 투명도를 영구적으로 35% 증가시킵니다.(중첩 불가)", AbilityCategory.Rare, AbilityType.RARE_SCP079_SYSTEMHACKING, RoleAbility.Scp079)]
public class SystemHacking : Ability
{
    private const byte FadeIntensity = 89;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.ReceivingEffect += OnReceivingEffect;

        foreach (var scp in GetTargetScps())
            scp.EnableEffect(EffectType.Fade, FadeIntensity);
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.ReceivingEffect -= OnReceivingEffect;
    }

    private void OnReceivingEffect(ReceivingEffectEventArgs ev)
    {
        if (ev.Effect.GetEffectType() != EffectType.Fade || !IsTarget(ev.Player))
            return;

        // 구속(Anchor) 중에는 Anchor의 Fade(179)를 우선한다.
        if (ev.Player.IsCaptured(out _))
            return;

        ev.Intensity = FadeIntensity;
        ev.Duration = 0f;
    }

    private static bool IsTarget(Player player) =>
        player != null && player.IsScpRole() && player.Role.Type != RoleTypeId.Scp079;

    private static IEnumerable<Player> GetTargetScps() =>
        PlayerManager.List.Where(IsTarget);
}
