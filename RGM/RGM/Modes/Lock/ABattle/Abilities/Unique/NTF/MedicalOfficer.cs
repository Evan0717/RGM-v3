using System.Collections.Generic;
using System.Linq;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.NTF;

[Ability("의무병", "본인을 포함하여 주변에 있는 아군들을 매 초마다 2HP씩 치료합니다.", AbilityCategory.Normal, AbilityType.NORMAL_NTF_MEDICALOFFICER, RoleAbility.NTF)]
public class MedicalOfficer : Ability
{
    CoroutineHandle _medical;

    public override void OnEnabled()
    {
        _medical = Timing.RunCoroutine(Medical());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_medical);
    }

    private IEnumerator<float> Medical()
    {
        while (true)
        {
            foreach (var team in PlayerManager.List.Where(x => x.LeadingTeam == Owner.LeadingTeam && x.IsAlive && Vector3.Distance(x.Position, Owner.Position) < 7))
            {
                if (team.Health < team.MaxHealth)
                    team.Health += 2f;
            }

            yield return Timing.WaitForSeconds(1f);
        }
    }
}
