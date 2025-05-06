using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using Scp914.Processors;
using Scp914;
using UnityEngine;

namespace RGM.Modes.Abilities.Normal;

[Ability("교환", "현재 들고 있는 아이템을 강화합니다. (1:1 기준)", AbilityCategory.Common, AbilityType.NORMAL_EXCHANGE)]
public class Exchange : Ability
{
    public override void OnEnabled()
    {
        if (Owner.CurrentItem == null)
            throw new System.Exception("현재 들고 있는 아이템이 없습니다.");

        else
        {
            if (Scp914Upgrader.TryGetProcessor(Owner.CurrentItem.Type, out Scp914ItemProcessor processor))
                processor.UpgradeInventoryItem(Scp914KnobSetting.OneToOne, Owner.CurrentItem.Base);
        }
    }

    public override void OnDisabled()
    {
    }

    public void OnHurting(HurtingEventArgs ev)
    {
    }
}
