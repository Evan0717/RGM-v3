using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Server;
using RGM.API.Features;
using static RGM.Variables.ServerManagers;
using UnityEngine;
using AdminToys;
using Interactables.Interobjects.DoorUtils;
using ProjectMER.Features;
using Exiled.API.Extensions;
using ProjectMER.Features.Objects;
using RGM.API.Components;
using MultiBroadcast.API;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.RUN)]
    public class RUN : Mode
    {
        public override string Name => "RUN FOR YOUR LIFE";
        public override string Description => "뛰세요, 따라잡히기 전에!";
        public override string Detail =>
"""
모든 플레이어는 SCP-096이 되며, 뒤따라오는 무언가에 잡히지 않도록 도망가야 합니다.
하지만 앞은 장애물들이 가로막고 있죠. 과연 무사히 도착 지점으로 갈 수 있을까요?
""";
        public override string Color => "da0101";

        public static RUN Instance;

        List<Transform> lightSources;
        List<string> objects = new()
        {
            "Babel",
            "Bed",
            "Bench",
            "BigDesk",
            "Bin",
            "Box",
            "CabinetHanger",
            "CabinetSink",
            "Chair",
            "ChemicalsCabinet",
            "ControlPanel",
            "Desk",
            "DrawCabinet",
            "Freeze",
            "Lift",
            "OvenWithGasGas",
            "RoundChair",
            "Seat",
            "Sofa",
            "Table",
            "TallBox",
            "ThreadMill",
            "TV",
            "Washer",
            "Whiteboard",
        };

        Vector3 pos;
        Vector3 finalDoor;
        Vector3 border;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Scp096.Enraging += OnEnraging;

            Exiled.Events.Handlers.Player.Died += OnDied;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            Server.FriendlyFire = true;
            Round.IsLocked = true;
            Respawn.PauseWaves();

            Tools.LoadMap("Run!");

            yield return Timing.WaitForSeconds(1);

            for (int i = 0; i < 400; i++)
            {
                SchematicObject schematic = ObjectSpawner.SpawnSchematic(
                    $"{objects.GetRandomValue()}", 
                    new Vector3(UnityEngine.Random.Range(-235.5703f, 121.7918f), UnityEngine.Random.Range(336.903f, 346.5427f), UnityEngine.Random.Range(-42.98623f, -51.62109f)), 
                    new Quaternion(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360))
                );
            }

            lightSources = Tools.GetObjectList("LightSource");
            pos = GameObject.Find("[SP] Base").transform.position;
            finalDoor = GameObject.Find("Final Door").transform.position;
            border = GameObject.Find("BorderForSomething").transform.position;

            foreach (var player in Player.List)
            {
                player.Role.Set(RoleTypeId.Scp096);
                player.Position = pos;
            }

            yield return Timing.WaitForSeconds(9);

            Timing.RunCoroutine(Loop());
            Timing.RunCoroutine(Raser());
            Timing.RunCoroutine(RaserCheck());

            foreach (var player in Player.List)
            {
                if (player.Role is Scp096Role scp096)
                {
                    scp096.Enrage(1205);
                }
            }

            GlobalPlayer.TryPlay("RUN FOR IT", 1.2f);

            yield return Timing.WaitForSeconds(130);

            Round.IsLocked = false;

            var players = Player.List.Where(x => x.IsAlive && !x.IsNPC);

            if (players.Count() == 1)
            {
                Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 5));
            }
            else
            {
                Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 1));
            }
        }

        public IEnumerator<float> Loop()
        {
            while (!Round.IsEnded)
            {
                foreach (var light in lightSources)
                {
                    light.GetComponent<LightSourceToy>().NetworkLightColor = new Color(2.55f, 2.55f, 2.55f);
                }

                yield return Timing.WaitForSeconds(1);

                foreach (var light in lightSources)
                {
                    light.GetComponent<LightSourceToy>().NetworkLightColor = new Color(2.18f, 0.01f, 0.01f);
                }

                yield return Timing.WaitForSeconds(1);
            }
        }

        public IEnumerator<float> Raser()
        {
            while (!Round.IsEnded)
            {
                if (UnityEngine.Random.Range(1, 3) == 1)
                {
                    SchematicObject raser = ObjectSpawner.SpawnSchematic(
                        $"Raser{UnityEngine.Random.Range(1, 11)}",
                        new Vector3(finalDoor.x, finalDoor.y - 2.5f, finalDoor.z),
                        new Quaternion(0, new List<int> { 0, 180 }.GetRandomValue(), 0, 0)
                    );

                    IEnumerator<float> enumerator()
                    {
                        while (!Round.IsEnded)
                        {
                            raser.Position += new Vector3(-0.075f, 0, 0);

                            yield return Timing.WaitForOneFrame;
                        }
                    }

                    Timing.RunCoroutine(enumerator());
                }

                yield return Timing.WaitForSeconds(1);
            }
        }

        public IEnumerator<float> RaserCheck()
        {
            while (!Round.IsEnded)
            {
                foreach (var player in Player.List.Where(x => x.IsAlive))
                {
                    foreach (var vector in new List<Vector3> { Vector3.down, Vector3.forward, Vector3.back, Vector3.left, Vector3.right })
                    {
                        if (Physics.Raycast(player.Position, vector, out RaycastHit hit, 1.2f))
                        {
                            if (new List<string> { "Oh no", "BorderForSomething" }.Contains(hit.transform.name))
                            {
                                player.Kill("안타깝군요..");
                            }
                        }
                    }
                }

                yield return Timing.WaitForOneFrame;
            }
        }

        public IEnumerator<float> OnEnraging(Exiled.Events.EventArgs.Scp096.EnragingEventArgs ev)
        {
            yield return Timing.WaitForOneFrame;

            ev.Scp096.EnrageCooldown = 0;
            ev.Scp096.EnragedTimeLeft = 99999;
            ev.Scp096.SprintingSpeed = 500;
        }

        public void OnDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            var players = Player.List.Where(x => x.IsAlive && !x.IsNPC);

            if (players.Count() == 0)
            {
                Round.IsLocked = false;
            }    
        }
    }
}
