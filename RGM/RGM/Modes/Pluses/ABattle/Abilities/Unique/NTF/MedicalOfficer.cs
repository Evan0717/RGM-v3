using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.NTF;

[Ability("의무병", "주변에 있는 아군들을 매 초마다 0.5HP씩 치료합니다.", AbilityCategory.NTF, AbilityType.NTF_MEDICALOFFICER)]
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

    public IEnumerator<float> Medical()
    {
        while (true)
        {
            foreach (var team in PlayerManager.List.Where(x => x.LeadingTeam == Owner.LeadingTeam && x.IsAlive && x != Owner && Vector3.Distance(x.Position, Owner.Position) < 6))
            {
                if (team.Health < team.MaxHealth)
                    team.Health += 0.5f;
            }

            yield return Timing.WaitForSeconds(1f);
        }
    }
}
