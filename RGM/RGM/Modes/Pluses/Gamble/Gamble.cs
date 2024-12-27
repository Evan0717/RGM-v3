using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using RGM.API.Features;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.Gamble)]
    public class Gamble : Mode
    {
        public override string Name => "도박";
        public override string Description => "아이템을 떨구면 새로운 아이템을 획득합니다. 단, 2% 확률로 손이 잘립니다.";
        public override string Detail =>
"""
생각 없이 도박을 하다 보면 2%는 금방이랍니다.
""";
        public override string Color => "8A4B08";


        public static Gamble Instance;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return 0f;
        }

        public void OnDroppingItem(Exiled.Events.EventArgs.Player.DroppingItemEventArgs ev)
        {
            List<ItemType> ItemList = Tools.EnumToList<ItemType>();
            ItemType Item = Tools.GetRandomValue(ItemList);

            int rand = UnityEngine.Random.Range(1, 101);
            if (0 < rand && rand < 3)
            {
                ev.Player.EnableEffect(Exiled.API.Enums.EffectType.SeveredHands);
            }
            else
            {
                ev.Item.Destroy();
                Item CurrentItem = ev.Player.AddItem(Item);
                ev.Player.DropItem(CurrentItem);
            }
        }
    }
}
