using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using RGM.API.DataBases;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Normal;

[Ability("랜덤박스", "랜덤한 아이템을 지급받습니다.", AbilityCategory.Common, AbilityType.NORMAL_RANDOMBOX)]
public class RandomBox : Ability
{
    public override void OnEnabled()
    {
        List<ItemType> RandomBox = Tools.EnumToList<ItemType>().Where(x => x != ItemType.SCP1507Tape && !Datas.ExceptItems.Contains(x)).ToList();

        Item RandomBoxItem = Owner.AddItem(Tools.GetRandomValue(RandomBox));
    }

    public override void OnDisabled()
    {

    }
}
