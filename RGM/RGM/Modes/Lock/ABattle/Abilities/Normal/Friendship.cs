using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using RGM.API.DataBases;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Normal;

[Ability("우애", "자신이 가진 아이템 중 하나를 복사하여 근처에 있는 플레이어에게 지급합니다.", AbilityCategory.Common, AbilityType.NORMAL_FRIENDSHIP)]
public class Friendship : Ability
{
    public override void OnEnabled()
    {
        if (Tools.TryGetNearestPlayer(Owner, out Player nearestPlayer, out float radius))
        {
            Item own = Owner.Items.GetRandomValue();
            if (own == null)
            {
                Owner.AddHint("우애","인벤토리에 나누어 줄 만한 아이템이 없습니다");
            }
            nearestPlayer.AddItem(own.Type);

            Owner.AddHint("우애", $"{nearestPlayer.DisplayNickname}(에)게 {( Trans.Item[own.Type])}(을)를 나누어 주었습니다.");
            nearestPlayer.AddHint("우애", $"{Owner.DisplayNickname}(으)로부터 {( Trans.Item[own.Type])}(을)를 나누어 받았습니다.");
        }
    }

    public override void OnDisabled()
    {
    }
}
