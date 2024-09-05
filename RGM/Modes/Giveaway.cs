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
    public class Giveaway
    {
        public static Giveaway Instance;

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            List<ItemType> itemList = Tools.EnumToList<ItemType>();

            while (true)
            {
                foreach (var player in Player.List)
                {
                    var toGive = itemList[UnityEngine.Random.Range(0, itemList.Count())];
                    Item CurrentItem = player.AddItem(toGive);

                    if (player.IsScp)
                    {
                        player.CurrentItem = CurrentItem;
                    }
                }

                yield return Timing.WaitForSeconds(60);
            }
        }
    }
}
