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

[RequiresAbility(AbilityType.NORMAL_EXCHANGE, AbilityType.RARE_UPGRADE, AbilityType.EPIC_GAMBLER)]
[Ability("암시장", "<교환, 강화, 도박꾼> 다른 진영의 전용 능력이 능력 선택창에 나타날 수 있습니다.", AbilityCategory.Synergy, AbilityType.SYNERGY_BLACKMARKET)]
public class BlackMarket : Ability
{
    public override void OnEnabled()
    {
    }

    public override void OnDisabled()
    {
    }
}
