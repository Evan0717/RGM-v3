using MEC;
using PlayerRoles;
using RGM.API.Components;
using RGM.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Exiled.API.Features;
using MultiBroadcast.API;

using static RGM.Variables.ServerManagers;

using static RGM.Functions.ModeManagers;

using static RGM.IEnumerators.LobbyManagers;
using static RGM.IEnumerators.ServerManagers;

using Exiled.API.Enums;
using RGM.API.DataBases;
using InventorySystem.Configs;

namespace RGM.EventArgs
{
    public static class MEREvents
    {
        public static void OnLoadingMap(MapEditorReborn.Events.EventArgs.LoadingMapEventArgs ev)
        {
            Log.Info($"로드된 맵: {ev.NewMap.Name}");

            foreach (var player in Player.List)
            {
                player.AddBroadcast(10, $"<size=20>맵 불러오기 완료: {ev.NewMap.Name}</size>");
            }
        }
    }
}
