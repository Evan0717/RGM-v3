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

namespace RGM.Modes.Abilities.Synergy;

[Ability("중복 기연", "<능력 선택지가 모두 중복일 경우> 당신은 해당 능력과 인연을 맺은 것 같습니다. 3개 전부 다 드릴게요!", AbilityCategory.Synergy, AbilityType.SYNERGY_DUPLICATEFATE)]
public class DuplicateFate : Ability
{
    public override void OnEnabled()
    {

    }

    public override void OnDisabled()
    {
    }
}
