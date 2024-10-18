using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using PlayerRoles;
using UnityEngine;

namespace RGM.Modes.ABattleFunctions
{
    public static class SpecificAbilities
    {
        public static void CallSnakeHand(Player Convener, List<Player> PlayerList)
        {
            List<Player> SnakeHands = PlayerList;

            List<ItemType> Items = new List<ItemType>
                {
                    ItemType.KeycardFacilityManager,
                    ItemType.GunFSP9,
                    ItemType.GunRevolver,
                    ItemType.Adrenaline,
                    ItemType.AntiSCP207
                };

            List<ItemType> Ammos = new List<ItemType>
                {
                    ItemType.Ammo44cal,
                    ItemType.Ammo9x19
                };

            foreach (var p in SnakeHands)
            {
                p.Role.Set(RoleTypeId.Tutorial);
                p.Position = new Vector3(-0.08203125f, 1000.96f, 6.828125f);

                foreach (ItemType Item in Items)
                    p.AddItem(Item);

                for (int i = 1; i < 3; i++)
                {
                    foreach (var Ammo in Ammos)
                        p.AddItem(Ammo);
                }
            }

            if (Convener != null)
                Convener.ShowHint($"<i>{SnakeHands.Count()}명의 <color=#FE2EF7>동료</color>들이 당신과 함께합니다..</i>", 5f);
        }
    }
}
