using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace RGM.Modes.Abilities.Dummy;

[Ability("뱀의 손 수장", "뱀의 손 무전기를 사용한 장본인입니다.", AbilityCategory.Dummy, AbilityType.DUMMY_USEDSNAKEHANDRADIO)]
public class UsedSnakeHandRadio : Ability
{
    public override void OnEnabled()
    {
    }

    public override void OnDisabled()
    {
    }
}