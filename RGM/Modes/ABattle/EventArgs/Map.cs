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

namespace RGM.Modes.ABattleEventArgs
{
    public static class MapEvents
    {
        public static void OnLoadingMap(MapEditorReborn.Events.EventArgs.LoadingMapEventArgs ev)
        {
            if (ev.NewMap.Name == "ABattle")
                Player.List.ToList().ForEach(x => x.AddBroadcast(10, "<size=25><b><i><color=#FF00EA>피</color><color=#EF00EB>버</color> <color=#CF00ED>모</color><color=#BF00EF>드</color><color=#AF00F0>가</color> <color=#8F00F3>활</color><color=#7F00F4>성</color><color=#6F00F5>화</color><color=#5F00F7>되</color><color=#4F00F8>었</color><color=#3F00F9>습</color><color=#2F00FB>니</color><color=#1F00FC>다</color><color=#0F00FD>!</color></i></b></size>"));
        }
    }
}
