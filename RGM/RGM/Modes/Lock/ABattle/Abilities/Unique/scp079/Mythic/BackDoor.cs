using Exiled.API.Extensions;
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

using System;

namespace RGM.Modes.Abilities.Unique.Scp079.Mythic;


[Ability("백도어", "핑을 찍으면 근처의 가장 가까운 인간의 능력을 삭제 후 랜덤한 Scp에게 지급합니다. (사거리 5m)", AbilityCategory.Mythic, AbilityType.MYTHIC_SCP079_BACKDOOR, RoleAbility.Scp079)]
public class BackDoor : Ability
{
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
        try
        {
            if (ev.Player == null || ev.Player != Owner)
                return;

            if (!ev.IsAllowed) return;

            Vector3 pingPosition = ev.Position;
            float searchRadius = 5f;


            Player player = PlayerManager.List
                .Where(p => p.IsAlive)
                .Where(p => Vector3.Distance(pingPosition, p.Position) <= searchRadius)
                .Where(p => HitboxIdentity.IsEnemy(ev.Player.ReferenceHub, p.ReferenceHub))
                .Where(p => p.GetAbilities().Any(a => a.Data.RoleAbility == RoleAbility.None))
                .OrderBy(p => Vector3.Distance(pingPosition, p.Position))
                .FirstOrDefault();

            if (player == null)
            {
                Owner.AddHint("힌트", "근처에 인간이 없거나 인간이 전용이 아닌 능력을 가지고 있지 않습니다.", 1f);
                return;
            }

            List<Ability> TargetList = player.GetAbilities().Where(a => a.Data.RoleAbility == RoleAbility.None).ToList();
            Ability TargetAbility = TargetList.GetRandomValue();

            Player scp = PlayerManager.List.Where(x => x.IsScpRole() && x.Role.Type != RoleTypeId.Scp079 && x.IsAlive).GetRandomValue();
            if (scp == null)
            {
                Owner.AddHint("힌트", "능력을 줄 SCP가 없습니다.", 1f);
                return;
            }


            player.RemoveAbility(TargetAbility.Data.AbilityType);
            scp.AddAbility(TargetAbility.Data.AbilityType);


            Owner.AddHint("힌트", $"{scp.DisplayNickname}(<color={scp.Role.Color.ToHex()}>{Trans.Role[scp.Role.Type]}</color>)에게 [{ABattle.SelectFormat[TargetAbility.Data.Category.GetCategoryTranslation()]}]{TargetAbility.Data.Name} 능력을 주었습니다!", 1f);

        }  

        catch (Exception e)
        {
            Log.Error($"BackDoor 오류: {e}");
        }
    }
}