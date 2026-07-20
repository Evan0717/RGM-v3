using System.Linq;
using Exiled.Events.EventArgs.Player;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.NTF;

[Ability("집단 지성", "주변에 있는 아군 한명당 데미지가 13% 증가합니다.", AbilityCategory.Common, AbilityType.COMMON_NTF_COLLECTIVEINTELLIGENCE, RoleAbility.NTF)]
public class CollectiveIntelligence : Ability
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
        if (ev.Attacker == null || ev.Attacker != Owner)
            return;

        int PowerCount = 0;

        foreach (var player in PlayerManager.List.Where(x => !x.IsNPC && x.IsAlive && x.LeadingTeam == ev.Player.LeadingTeam && x != ev.Player))
        {
            if (Vector3.Distance(player.Position, ev.Player.Position) < 11)
                PowerCount++;
        }

        ev.DamageHandler.Damage = (int)(ev.DamageHandler.Damage * (1 + (0.13 * PowerCount)));
    }
}
