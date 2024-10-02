using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using PlayerRoles;
using RGM.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerStatsSystem;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using UnityEngine;

namespace RGM.Donator
{
    public class Main
    {
        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Dying += OnDying;
        }

        public async void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            if (ev.Attacker != null && UsersManager.UsersCache.ContainsKey(ev.Attacker.UserId))
            {
                List<string> Attacker = UsersManager.UsersCache[ev.Attacker.UserId];

                if (Attacker[4] != "0")
                {
                    if (Attacker[4] == "영혼 가출")
                    {
                        DamageHandlerBase DisruptorDamage = new DisruptorDamageHandler(ev.Attacker.Footprint, -1);

                        Ragdoll.CreateAndSpawn(ev.Player.Role.Type, Attacker[4], DisruptorDamage, ev.Player.Position, ev.Player.Rotation);
                    }

                    if (Attacker[4] == "솔라 테라")
                    {
                        SchematicObject SolarTerra =  ObjectSpawner.SpawnSchematic("SolarTerra", ev.Player.Position, isStatic: false);

                        Timing.CallDelayed(1.5f, () =>
                        {
                            SolarTerra.Destroy();
                        });
                    }

                    if (Attacker[4] == "Kerfus")
                    {
                        Vector3 pos = ev.Player.Position;

                        SchematicObject Kerfus = ObjectSpawner.SpawnSchematic("Kerfusa", new Vector3(pos.x, pos.y + 10, pos.z), isStatic: false);

                        for (int i = 1; i < 11; i++)
                        {
                            Kerfus.Position = new Vector3(pos.x, pos.y - 1, pos.z);

                            await Task.Delay(100);
                        }

                        await Task.Delay(350);

                        for (int i = 1; i < 11; i++)
                        {
                            Kerfus.Position = new Vector3(pos.x, pos.y + 1, pos.z);

                            await Task.Delay(100);
                        }

                        Kerfus.Destroy();
                    }
                }
            }
        }
    }
}
