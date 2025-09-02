using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using ProjectMER.Features.Objects;
using ProjectMER.Features.Serializable;
using ProjectMER.Features;
using MEC;
using RGM.API.Features;
using UnityEngine;
using RGM.API.DataBases;
using Exiled.API.Features.DamageHandlers;

namespace RGM.Modes.Abilities.Synergy;

[RequiresAbility(AbilityType.RARE_STEELSHELL, AbilityType.RARE_TRANSPARENTCLOAK, AbilityType.LEGEND_SCREAM, AbilityType.MYTHIC_EYEMAN)]
[Ability("G맨", "<강철 껍질, 투명 망토, 괴성, 눈빛맨> G맨 토일렛을 연상캐 하는 조합입니다.", AbilityCategory.Synergy, AbilityType.SYNERGY_GMAN)]
public class Gman : Ability
{
    public override void OnEnabled()
    {
        Timing.CallDelayed(1, () =>
        {
            Owner.AddAbility(AbilityType.MYTHIC_NOCLIP);
        });
    }

    public override void OnDisabled()
    {
    }
}
