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
    public static class Scp0492Events
    {
        public static void OnConsumedCorpse(Exiled.Events.EventArgs.Scp0492.ConsumedCorpseEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[전용] 허기"))
                ev.ConsumeHeal *= 2;
        }

        public static void OnTriggeringBloodlust(Exiled.Events.EventArgs.Scp0492.TriggeringBloodlustEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[전용] 당혹감"))
            {
                ev.Target.EnableEffect(EffectType.Blinded, 1, 0.5f);
                Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 0.2f);
            }
        }
    }
}
