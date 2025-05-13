using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace RGM.Modes.Abilities.Dummy;

[Ability("순간이동됨", "순간이동하셨군요!", AbilityCategory.Dummy, AbilityType.DUMMY_TELEPORTED)]
public class Teleported : Ability
{
    public override void OnEnabled()
    {
    }

    public override void OnDisabled()
    {
    }
}