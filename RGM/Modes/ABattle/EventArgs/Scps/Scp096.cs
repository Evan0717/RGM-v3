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
using RGM.API;
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
    public static class Scp096Events
    {
        public static void OnCharging(Exiled.Events.EventArgs.Scp096.ChargingEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[전용] 천리안"))
            {
                foreach (var player in Player.List.Where(x => x.IsHuman))
                {
                    if (Vector3.Distance(player.Position, ev.Player.Position) < 31)
                        ev.Scp096.AddTarget(player);
                }
            }
        }
    }
}
