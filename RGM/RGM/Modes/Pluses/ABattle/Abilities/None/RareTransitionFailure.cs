using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace RGM.Modes.Abilities.Dummy;

[Ability("하급 변이 실패", "안타깝네요, 하급 변이에 실패하였습니다.", AbilityCategory.Dummy, AbilityType.NONE_RARETRANSITIONFAILURE)]
public class RareTransitionFailure : Ability
{
    public override void OnEnabled()
    {
    }

    public override void OnDisabled()
    {
    }
}