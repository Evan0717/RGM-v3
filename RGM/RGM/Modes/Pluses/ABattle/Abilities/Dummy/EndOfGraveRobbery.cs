using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace RGM.Modes.Abilities.Dummy;

[Ability("도굴 끝!", "도굴을 완료했습니다.", AbilityCategory.Dummy, AbilityType.DUMMY_ENDOFGRAVEROBBERY)]
public class EndOfGraveRobbery : Ability
{
    public override void OnEnabled()
    {
    }

    public override void OnDisabled()
    {
    }
}