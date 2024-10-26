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
using RGM.API.DataBases;
using MultiBroadcast.API;

using static RGM.Variables.ServerManagers;

using static RGM.Modes.ABattleVariables.Abilities;
using static RGM.Modes.ABattleVariables.AbiltiyManagers;
using static RGM.Modes.ABattleVariables.MainManagers;
using static RGM.Modes.ABattleVariables.Cooldowns;
using static RGM.Modes.ABattleVariables.Serials;

using static RGM.Modes.ABattleFunctions.AbilityManagers;
using static RGM.Modes.ABattleFunctions.MainManagers;
using static RGM.Modes.ABattleFunctions.SpecificAbilities;
using RGM.Discord;

namespace RGM.Modes.ABattleEventArgs
{
    public static class ItemEvents
    {
        public static void OnSwinging(Exiled.Events.EventArgs.Item.SwingingEventArgs ev)
        {
            if (LightWarriorSerials.Contains(ev.Item.Serial))
            {
                ev.Jailbird.TotalCharges = 0;

                var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, ev.Player);
                g.FuseTime = 0.1f;
                g.SpawnActive(ev.Player.Position, ev.Player);
            }
        }
    }
}
