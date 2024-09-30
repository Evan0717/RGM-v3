using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using MEC;
using UnityEngine;
using Exiled.API.Enums;
using RGM.API;

namespace RGM.Modes
{
    public class WhereamI
    {
        public static WhereamI Instance;

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(1f);

            foreach (var player in Player.List)
            {
                Spawned(player);
            }

            yield break;
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            Door SelectedDoor = null;

            if (Map.IsLczDecontaminated)
                SelectedDoor = Tools.GetRandomValue(Door.List.Where(x => !x.IsElevator && x.Zone != ZoneType.LightContainment && !x.Type.ToString().Contains("Scp079")).ToList());

            else
                SelectedDoor = Tools.GetRandomValue(Door.List.Where(x => !x.IsElevator && !x.Type.ToString().Contains("Scp079")).ToList());

            player.Position = new Vector3(SelectedDoor.Position.x, SelectedDoor.Position.y + 2, SelectedDoor.Position.z);
        }
    }
}
