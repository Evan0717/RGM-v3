using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using Player = Exiled.API.Features.Player;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.Scp079;

[Ability("끈질김", "사망하지 않습니다. 아군이 모두 죽으면 사망합니다.", AbilityCategory.Epic, AbilityType.EPIC_SCP079_IMPORTUNITY, RoleAbility.Scp079)]
public class Importunity : Ability
{
    private CoroutineHandle _deathCoroutine;
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Dying += OnDying;
        _deathCoroutine = Timing.RunCoroutine(DeathCoroutine());
    }

    public override void OnDisabled()
    {

        Exiled.Events.Handlers.Player.Dying -= OnDying;
        Timing.KillCoroutines(_deathCoroutine);

        List<Player> ScpListWithOut79 = PlayerManager.List.Where(x => x.IsScpRole() && x.Role.Type != RoleTypeId.Scp079).ToList();

        if (ScpListWithOut79.Count <= 0)
        {
            Owner.Kill("아군이 모두 사망했습니다.");
        }
    }

    public void OnDying(DyingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;
        ev.IsAllowed = false;
    }


    IEnumerator<float> DeathCoroutine()
    {
        List<Player> ScpListWithOut79 = PlayerManager.List.Where(x => x.IsScpRole() && x.Role.Type != RoleTypeId.Scp079).ToList();

        if (ScpListWithOut79.Count <= 0 )
        {
            Owner.Kill("아군이 모두 사망했습니다.");
            yield break;
        }

        yield return Timing.WaitForSeconds(1f);
    }


}
