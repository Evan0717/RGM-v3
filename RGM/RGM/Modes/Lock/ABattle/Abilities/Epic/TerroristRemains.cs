using Exiled.API.Features.Items;
using InventorySystem.Items.Usables.Scp330;

namespace RGM.Modes.Abilities.Epic;

[Ability("테러리스트의 유품", "핑크 사탕(20% 확률로 사악한 사탕)이 포함된 SCP-330을 지급받습니다.", AbilityCategory.Epic, AbilityType.EPIC_TERRORISTREMAINS)]
public class TerroristRemains : Ability
{
    public override void OnEnabled()
    {
        Scp330 PinkCandy = (Scp330)Item.Create(ItemType.SCP330);
        Scp330 EvliCandy = (Scp330)Item.Create(ItemType.SCP330);
        if (UnityEngine.Random.Range(1, 6) == 1) {
            EvliCandy.RemoveAllCandy();
            EvliCandy.AddCandy(CandyKindID.Evil);
            Owner.AddItem(EvliCandy);
        }
        PinkCandy.RemoveAllCandy();
        PinkCandy.AddCandy(CandyKindID.Pink);
        Owner.AddItem(PinkCandy);
    }

    public override void OnDisabled()
    {
    }
}
