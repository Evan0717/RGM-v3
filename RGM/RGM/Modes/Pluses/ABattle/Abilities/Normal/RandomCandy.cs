using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Normal;

[Ability("트릭 오어 트릿", "<b><color=#FF9500>[</color><color=#FF9F09>H</color><color=#FFA912>A</color><color=#FFB31B>L</color><color=#FFBD24>L</color><color=#FFC72E>O</color><color=#FFDC37>W</color><color=#FFF240>E</color><color=#FFFF49>E</color><color=#FFFF52>N</color><color=#FFFF5C>]</color></b> 랜덤한 SCP-330을 받습니다. 운이 좋다면 더 받을수도 있겠죠..", AbilityCategory.Common, AbilityType.NORMAL_RANDOMCANDY)]
public class RandomCandy : Ability
{
    public override void OnEnabled()
    {
        Owner.AddCandy(Tools.EnumToList<CandyKindID>().GetRandomValue());
    }

    public override void OnDisabled()
    {
    }
}
