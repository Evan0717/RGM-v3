using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace RGM.Modes.Abilities.Dummy;

[Ability("만료된 보험", "보험을 사용하여 만료 상태로 전환되었습니다.", AbilityCategory.Dummy, AbilityType.NONE_EXPIREDINSURANCE)]
public class ExpiredInsurance : Ability
{
    public override void OnEnabled()
    {
    }

    public override void OnDisabled()
    {
    }
}