using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.DataBases;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Normal;

[Ability("우애", "자신이 가진 아이템 중 하나를 복사하여 근처에 있는 플레이어에게 지급합니다.", AbilityCategory.Common, AbilityType.NORMAL_FRIENDSHIP)]
public class Friendship : Ability
{
    public override void OnEnabled()
    {
        if (Tools.TryGetNearestPlayer(Owner, out Player nearestPlayer, out float radius))
        {
            Item Own = Tools.GetRandomValue(Owner.Items.ToList());

            nearestPlayer.AddItem(Own.Type);

            Owner.ShowHint($"{nearestPlayer.DisplayNickname}(에)게 {Trans.Item[Own.Type]}(을)를 나누어 주었습니다.");
            nearestPlayer.ShowHint($"{Owner.DisplayNickname}(으)로부터 {Trans.Item[Own.Type]}(을)를 나누어 받았습니다.");
        }
    }

    public override void OnDisabled()
    {
    }
}
