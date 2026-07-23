using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using Exiled.Events.Patches.Events.Player;
using MEC;
using PlayerRoles;
using ProjectMER.Features.Serializable;
using RGM.API.Features;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static RGM.Variables.Variable;
using RGM.API.DataBases;
using Exiled.API.Enums;
using Exiled.Events.Patches.Generic;

namespace RGM.Modes.Abilities.Unique.Scp079.Mythic;


[Ability("초월", "핑을 찍으면 핑 근처 가장 가까운 인간 1명이 20% 확률로 승천합니다. (사거리 5m)", AbilityCategory.Mythic, AbilityType.MYTHIC_SCP079_TRANSENDENCE, RoleAbility.Scp079)]
public class Transendence : Ability
{
    List<Player> isInRocket = new();
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp079.Pinging += OnPinging;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp079.Pinging -= OnPinging;
    }

    public void OnPinging(PingingEventArgs ev)
    {
        if (ev.Player == null || ev.Player != Owner)
            return;
        Vector3 pingPosition = ev.Position;
        float searchRadius = 5f;


        Player player = PlayerManager.List
            .Where(p => p.IsAlive)
            .Where(p => Vector3.Distance(pingPosition, p.Position) <= searchRadius)
            .Where(p => HitboxIdentity.IsEnemy(ev.Player.ReferenceHub, p.ReferenceHub))
            .OrderBy(p => Vector3.Distance(pingPosition, p.Position))
            .FirstOrDefault();

        if (player == null) return;


        if (Random.Range(1, 101) <= 20)
        {
            isInRocket.Add(player);

            Timing.RunCoroutine(Tools.DoRocket(Owner, player, 1));
            Tools.MessageTranslated("", $"{player.DisplayNickname}(<color={player.Role.Color.ToHex()}>{(Trans.Role[player.Role.Type])}</color>)(이)가 하늘로 승천했습니다.");

            Timing.CallDelayed(1, () =>
            {
                isInRocket.Remove(player);
            });
        }
        
    }
}