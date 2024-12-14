using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace RGM.Modes.Abilities.Dummy;

[Ability("변이 성공", "축하합니다, 변이에 성공하였습니다!", AbilityCategory.Dummy, AbilityType.NONE_EPICTRANSITIONSUCCESS)]
public class EpicTransitionSuccess : Ability
{
    public override void OnEnabled()
    {
    }

    public override void OnDisabled()
    {
    }
}