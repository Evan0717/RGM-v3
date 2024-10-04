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
using Exiled.API.Features.Toys;

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
                Quaternion Rotation = new Quaternion(0, ev.Attacker.CameraTransform.rotation.y + 180, 0, 0);

                if (Attacker[4] != "0")
                {
                    if (Attacker[4] == "영혼 가출")
                    {
                        DamageHandlerBase DisruptorDamage = new DisruptorDamageHandler(ev.Attacker.Footprint, -1);

                        Ragdoll.CreateAndSpawn(ev.Player.Role.Type, Attacker[4], DisruptorDamage, ev.Player.Position, Rotation);
                    }

                    if (Attacker[4] == "솔라 테라")
                    {
                        SchematicObject SolarTerra =  ObjectSpawner.SpawnSchematic("SolarTerra", ev.Player.Position, Rotation, isStatic: false);

                        Timing.CallDelayed(1.5f, SolarTerra.Destroy);
                    }

                    if (Attacker[4] == "Kerfus")
                    {
                        SchematicObject Kerfus = ObjectSpawner.SpawnSchematic("Kerfusa", ev.Player.Position + new Vector3(0, 19, 0), Rotation, isStatic: false);

                        for (int i = 1; i < 11; i++)
                        {
                            Kerfus.Position += new Vector3(0, -2f, 0);

                            await Task.Delay(50);
                        }

                        await Task.Delay(1500);

                        for (int i = 1; i < 11; i++)
                        {
                            Kerfus.Position += new Vector3(0, 2f, 0);

                            await Task.Delay(50);
                        }

                        Kerfus.Destroy();
                    }

                    if (Attacker[4] == "은제 말뚝")
                    {
                        SchematicObject SilverStake = ObjectSpawner.SpawnSchematic("SilverStake", ev.Player.Position, Rotation, isStatic: false);

                        Timing.CallDelayed(1.5f, SilverStake.Destroy);
                    }

                    if (Attacker[4] == "KO 사인")
                    {
                        SchematicObject KO = ObjectSpawner.SpawnSchematic("KO", ev.Player.Position, Rotation, isStatic: false);

                        Timing.CallDelayed(1.5f, KO.Destroy);
                    }
                }
            }
        }
    }
}
