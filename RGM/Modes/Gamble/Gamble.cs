using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using RGM.API;

namespace RGM.Modes
{
    public class Gamble
    {
        public static Gamble Instance;

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield break;
        }

        public void OnDroppingItem(Exiled.Events.EventArgs.Player.DroppingItemEventArgs ev)
        {
            List<ItemType> ItemList = Tools.EnumToList<ItemType>();
            ItemType Item = ItemList[UnityEngine.Random.Range(1, ItemList.Count())];

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
