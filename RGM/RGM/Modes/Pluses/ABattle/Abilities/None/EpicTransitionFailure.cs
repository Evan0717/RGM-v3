using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace RGM.Modes.Abilities.Dummy;

[Ability("변이 실패", "안타깝네요, 변이에 실패하였습니다.", AbilityCategory.Dummy, AbilityType.NONE_EPICTRANSITIONFAILURE)]
public class EpicTransitionFailure : Ability
{
    public override void OnEnabled()
    {
    }

    public override void OnDisabled()
    {
    }
}