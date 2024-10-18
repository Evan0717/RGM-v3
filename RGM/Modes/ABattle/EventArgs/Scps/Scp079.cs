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
    public static class Scp079Events
    {
        public static void OnGainingLevel(Exiled.Events.EventArgs.Scp079.GainingLevelEventArgs ev)
        {
            AddAbility(ev.Player);
        }

        public static void OnPinging(Exiled.Events.EventArgs.Scp079.PingingEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[전용] 핑 리모컨"))
            {
                if (!ev.Room.AreLightsOff)
                    ev.Room.TurnOffLights(0.5f * DuplicateCount(ev.Player, "[전용] 핑 리모컨"));
            }
        }

        public static void OnZoneBlackout(Exiled.Events.EventArgs.Scp079.ZoneBlackoutEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[전용] 랜덤 함수"))
            {
                for (int i = 1; i < 6 * DuplicateCount(ev.Player, "[전용] 랜덤 함수"); i++)
                {
                    Room SelectedRoom = Tools.GetRandomValue(Room.List.ToList());

                    SelectedRoom.TurnOffLights(10);
                }
            }
        }
    }
}
