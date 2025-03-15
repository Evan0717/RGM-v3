using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace RGM.Modes.Abilities.Dummy;

[Ability("식은 스튜", "이 스튜 좀 맛있나..?", AbilityCategory.Dummy, AbilityType.DUMMY_COLDSTEW)]
public class ColdStew : Ability
{
    public override void OnEnabled()
    {
    }

    public override void OnDisabled()
    {
    }
}