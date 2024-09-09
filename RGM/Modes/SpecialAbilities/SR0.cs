using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using RGM.API;

namespace RGM.Modes.SpecialAbilities
{
    public class SR0 // 보급
    {
        Player target;

        public void OnEnabled(Player player)
        {
            target = player;

            Timing.RunCoroutine(OnStarted());
        }

        public IEnumerator<float> OnStarted()
        {
            int Stack = 0;
            while (target.IsAlive)
            {
                Stack += 1;

                if (Stack == 60)
                {
                    List<ItemType> itemList = Tools.EnumToList<ItemType>();
                    ItemType toGive = itemList[UnityEngine.Random.Range(0, itemList.Count())];
                    target.AddItem(toGive);
                    Stack = 0;
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
