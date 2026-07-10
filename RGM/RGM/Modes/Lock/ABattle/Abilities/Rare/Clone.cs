using System.Collections.Generic;
using CommandSystem.Commands.RemoteAdmin.Dummies;
using MEC;
using Mirror;
using PlayerRoles;
using RGM.API.Features;
using UnityEngine;
using NetworkManagerUtils.Dummies;

namespace RGM.Modes.Abilities.Rare;

//[Ability("분신", "자신을 따라다니는 분신을 소환합니다. (SCP의 경우 SCP-049-2로 대체)", AbilityCategory.Rare, AbilityType.RARE_CLONE)]
public class Clone : Ability
{
    CoroutineHandle _onStarted;
    ReferenceHub clone;

    public override void OnEnabled()
    {
        _onStarted = Timing.RunCoroutine(onStarted());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_onStarted);

        NetworkServer.Destroy(clone.gameObject);
    }

    public IEnumerator<float> onStarted()
    {
        clone = DummyUtils.SpawnDummy($"누군가의 분신");

        clone.roleManager.ServerSetRole(Owner.IsScpRole() ? RoleTypeId.Scp0492 : Owner.Role.Type, RoleChangeReason.ItemUsage);
        clone.transform.position = Owner.Position;

        while (Owner.IsAlive)
        {
            clone.gameObject.AddComponent<PlayerFollower>().Init(Owner.ReferenceHub, 125f, 1, 60f);

            if (Vector3.Distance(Owner.Position, clone.transform.position) > 50f)
            {
                clone.transform.position = Owner.Position;
            };

            yield return Timing.WaitForSeconds(2);
        }
    }
}
