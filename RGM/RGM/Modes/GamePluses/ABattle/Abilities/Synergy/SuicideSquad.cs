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

[RequiresAbility(ㅎㅊㄹ)]
[Ability("수어사이드 스쿼드", "<봄버맨, 수어사이드 봄버맨> 사망할 경우 SCP-018를 3개 떨어트립니다.", AbilityCategory.Synergy, AbilityType.SYNERGY_SURVIVALEXPERT)]
public class SuicideSquad : Ability
{
    public override void OnEnabled()
    {
        Owner.Health = Owner.Health * 5;

        if (Owner.MaxHealth < Owner.Health)
            Owner.MaxHealth = Owner.Health;
    }

    public override void OnDisabled()
    {
    }
}
