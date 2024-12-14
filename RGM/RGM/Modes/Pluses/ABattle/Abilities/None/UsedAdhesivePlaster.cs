using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace RGM.Modes.Abilities.Dummy;

[Ability("해진 반창고", "더 이상 사용할 수 없어 보이는 반창고군요.", AbilityCategory.Dummy, AbilityType.NONE_USEDADHESIVEPLASTER)]
public class UsedAdhesivePlaster : Ability
{
    public override void OnEnabled()
    {
    }

    public override void OnDisabled()
    {
    }
}