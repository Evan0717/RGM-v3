using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using UnityEngine;
using PlayerRoles;
using MEC;
using Exiled.API.Features.Roles;
using Exiled.API.Enums;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using InventorySystem.Items.MicroHID;
using Mirror;
using RGM.API.Features;
using MultiBroadcast.API;

using static RGM.Modes.ABattleVariables.Abilities;
using static RGM.Modes.ABattleVariables.AbiltiyManagers;
using static RGM.Modes.ABattleVariables.MainManagers;
using static RGM.Modes.ABattleVariables.Cooldowns;
using static RGM.Modes.ABattleVariables.Serials;

using static RGM.Modes.ABattleFunctions.AbilityManagers;
using static RGM.Modes.ABattleFunctions.MainManagers;
using static RGM.Modes.ABattleFunctions.SpecificAbilities;

namespace RGM.Modes.ABattleEventArgs.Scps
{
    public static class Scp3114Events
    {
        public static void OnRevealed(Exiled.Events.EventArgs.Scp3114.RevealedEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[전용] 도라에몽 주머니"))
            {
                List<ItemType> ItemTypes = Tools.EnumToList<ItemType>();

                Item Item = ev.Player.AddItem(Tools.GetRandomValue(ItemTypes));

                if (ev.Player.IsScp)
                    ev.Player.CurrentItem = Item;
            }
        }
    }
}
