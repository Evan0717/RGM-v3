using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Epic;

// [Ability("위장술", "항상 게임에서 유리한 세력의 모습을 지니게 됩니다.", AbilityCategory.Epic, AbilityType.EPIC_DISGUISE)]
public class Disguise : Ability
{
    CoroutineHandle _disguise;

    public override void OnEnabled()
    {
        _disguise = Timing.RunCoroutine(disguise());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_disguise);
    }

    public IEnumerator<float> disguise()
    {
        Team? currentTeam = null;
        List<RoleTypeId> _blockedRoles = new List<RoleTypeId>()
        {
            RoleTypeId.Spectator,
            RoleTypeId.Scp079,
            RoleTypeId.Overwatch,
            RoleTypeId.Filmmaker,
            RoleTypeId.CustomRole,
            RoleTypeId.Destroyed,
            RoleTypeId.Scp3114
        };

        while (Owner.IsAlive)
        {
            var teams = new Dictionary<Team, int>();

            foreach (var player in Player.List.Where(x => x.IsAlive))
            {
                if (player.Role.Team != Team.Dead && player.Role.Team != Team.OtherAlive)
                {
                    if (!teams.ContainsKey(player.Role.Team))
                        teams[player.Role.Team] = 0;

                    teams[player.Role.Team]++;
                }
            }

            var mostCommonTeam = teams.OrderByDescending(fc => fc.Value).FirstOrDefault().Key;

            if (currentTeam != mostCommonTeam)
            {
                currentTeam = mostCommonTeam;

                Exiled.API.Extensions.MirrorExtensions.ChangeAppearance(Owner, Tools.GetRandomValue(Tools.EnumToList<RoleTypeId>().Where(x => x.GetTeam() == mostCommonTeam && !_blockedRoles.Contains(x)).ToList()));
            }

            yield return Timing.WaitForSeconds(1);
        }
    }
}
