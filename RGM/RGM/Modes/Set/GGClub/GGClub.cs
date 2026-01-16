using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomPlayerEffects;
using CustomRendering;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using ProjectMER.Features.Objects;
using ProjectMER.Features.Serializable;
using MEC;
using Mirror;
using PlayerRoles;
using LabApi.Events;
using UnityEngine;
using SCPSLAudioApi.AudioCore;
using MultiBroadcast;
using RGM.API.Features;

using Exiled.Events.EventArgs.Server;
using Respawning;

using static RGM.Variables.Variable;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.GGClub)]
    class GGClub : Mode
    {
        public override string Name => "GG 클럽";
        public override string Description => "빠르게 황금색 플랫폼을 사수하세요!";
        public override string Detail =>
"""
<b><color=#F00000>페</color><color=#F54900>이</color><color=#FA9300>즈</color></b>가 총 10개로 이루어져 있습니다!

순발력을 마음껏 뽐내 보세요!
""";
        public override string Color => "C8FE2E";
        public override string Map => "GGClub";

        public static GGClub Instance;

        List<Player> pl = new List<Player>();
        List<Transform> ClubLights = new List<Transform>();
        List<Transform> Pads = new List<Transform>();
        List<Transform> goldPads = new List<Transform>();

        bool IsSongStopped = false;
        int Phase = 1;

        CoroutineHandle _onModeStarted;
        CoroutineHandle _dj;
        CoroutineHandle _gingerbreadHint;
        CoroutineHandle _showPhase;

        AudioClipPlayback audio;

        public override void OnEnabled()
        {
            Round.IsLocked = true;
            Respawn.PauseWaves(); 
            Server.FriendlyFire = true;

            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.SpawnedRagdoll += OnSpawnedRagdoll;

            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
            _dj = Timing.RunCoroutine(DJ());
            _gingerbreadHint = Timing.RunCoroutine(gingerbreadHint());
            _showPhase = Timing.RunCoroutine(ShowPhase());

            audio = Tools.PlayGlobalAudio("tothemoon", 1, true);
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            Exiled.Events.Handlers.Player.Died -= OnDied;
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Player.SpawnedRagdoll -= OnSpawnedRagdoll;

            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            Timing.KillCoroutines(_onModeStarted);
            Timing.KillCoroutines(_dj);
            Timing.KillCoroutines(_gingerbreadHint);
            Timing.KillCoroutines(_showPhase);

            audio.IsPaused = true;
        }

        public IEnumerator<float> OnModeStarted()
        {
            PlayerManager.List.CopyTo(pl);

            yield return Timing.WaitForOneFrame;

            foreach (var player in PlayerManager.List.Where(x => !x.IsNPC).ToList())
            {
                player.Role.Set(RoleTypeId.Scp3114);
                player.Position = new Vector3(77.66434f, 315.4858f, -42.20193f);

                Server.ExecuteCommand($"/speak {player.Id} 1");
                IntercomPlayers.Add(player);
            }

            for (int i = 1; i < 11; i++)
            {
                PlayerManager.List.ToList().ForEach(x => x.AddHint("GG클럽 게임 시작 알림", $"게임이 {11 - i}초 후에 시작됩니다.", 1.2f));

                yield return Timing.WaitForSeconds(1f);
            }

            ClubLights = Tools.GetObjectList("ClubLight");
            Pads = Tools.GetObjectList("Pad");

            while (Phase < 11)
            {
                yield return Timing.WaitForSeconds(11 - Phase);

                IsSongStopped = true;
                audio.IsPaused = true;

                for (int i = 1; i < 12 - Phase; i++)
                {
                    Transform goldPad = Tools.GetRandomValue(Pads);

                    if (!goldPads.Contains(goldPad))
                        goldPads.Add(goldPad);
                }

                yield return Timing.WaitForSeconds(3 - Phase * 0.2f);

                foreach (var player in PlayerManager.List.Where(pl.Contains))
                {
                    if (Physics.Raycast(player.Position, Vector3.down, out RaycastHit hit, 3f, (LayerMask)1))
                    {
                        try
                        {
                            if (hit.transform.GetComponent<AdminToys.PrimitiveObjectToy>().NetworkMaterialColor != UnityEngine.Color.yellow)
                            {
                                if (player.IsAlive && !player.IsNPC)
                                    player.Kill("황금색 발판을 밟지 못한 자여.");
                            }
                        }
                        catch (Exception e)
                        {
                            if (player.IsAlive && !player.IsNPC)
                                player.Kill("황금색 발판을 밟지 못한 자여..");
                        }
                    }
                    else
                    {
                        if (player.IsAlive && !player.IsNPC)
                            player.Kill("황금색 발판을 밟지 못한 자여...");
                    }
                }

                foreach (var Pad in Pads)
                {
                    if (Pad.GetComponent<AdminToys.PrimitiveObjectToy>().NetworkMaterialColor == UnityEngine.Color.white)
                        Pad.GetComponent<AdminToys.PrimitiveObjectToy>().NetworkMaterialColor = UnityEngine.Color.red;
                }

                yield return Timing.WaitForSeconds(1.25f);

                IsSongStopped = false;
                audio.IsPaused = false;
                goldPads.Clear();

                Phase++;
            }

            Round.IsLocked = false;

            if (pl.Count > 1)
                PlayerManager.List.ToList().ForEach(x => x.AddBroadcast(3, "게임이 종료되었습니다. 그나저나 어떻게 사셨"));
        }

        public IEnumerator<float> gingerbreadHint()
        {
            while (true)
            {
                foreach (var player in PlayerManager.List)
                    player.AddHint("3114 힌트", $"<b>[TIP]</b> <i>gingerbread</i>를 입력하면 춤출 수 있습니다.", 1.2f);

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public IEnumerator<float> DJ()
        {
            yield return Timing.WaitForSeconds(1f);

            while (true)
            {

                if (!IsSongStopped)
                {
                    foreach (var Pad in Pads)
                    {
                        Pad.GetComponent<AdminToys.PrimitiveObjectToy>().NetworkMaterialColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                    }

                    foreach (var ClubLight in ClubLights)
                    {
                        ClubLight.GetComponent<AdminToys.LightSourceToy>().NetworkLightColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                    }
                }
                else
                {
                    foreach (var Pad in Pads)
                    {
                        Pad.GetComponent<AdminToys.PrimitiveObjectToy>().NetworkMaterialColor = UnityEngine.Color.white;
                    }

                    foreach (var Pad in goldPads)
                    {
                        Pad.GetComponent<AdminToys.PrimitiveObjectToy>().NetworkMaterialColor = UnityEngine.Color.yellow;
                    }

                    foreach (var ClubLight in ClubLights)
                    {
                        ClubLight.GetComponent<AdminToys.LightSourceToy>().NetworkLightColor = UnityEngine.Color.white;
                    }
                }

                yield return Timing.WaitForSeconds(1.1f - Phase * 0.1f);
            }
        }

        public IEnumerator<float> ShowPhase()
        {
            while (Phase < 11)
            {
                MultiBroadcast.API.MultiBroadcast.AddMapBroadcast(1, $"<b><color=#FFF700>P</color><color=#FFC516>h</color><color=#FF942C>a</color><color=#FF6242>s</color><color=#FF3158>e</color></b> {Phase}");

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Server.ExecuteCommand($"/speak {ev.Player.Id} 1");
            IntercomPlayers.Add(ev.Player);
        }

        public void OnDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            if (pl.Contains(ev.Player))
            {
                pl.Remove(ev.Player);

                if (pl.Count < 3)
                    Round.IsLocked = false;
            }
        }

        public void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            if (ev.Player.IsNPC)
                ev.IsAllowed = false;
        }

        public void OnSpawnedRagdoll(Exiled.Events.EventArgs.Player.SpawnedRagdollEventArgs ev)
        {
            ev.Ragdoll.Destroy();
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            IEnumerable<Player> players = PlayerManager.List.Where(x => x.IsAlive && !x.IsNPC);

            if (players.Count() == 1)
                Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 5));

            else if (players.Count() > 1)
                Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 1));
        }
    }
}
