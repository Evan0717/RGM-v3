using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomPlayerEffects;
using CustomRendering;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features.Serializable;
using MEC;
using Mirror;
using PlayerRoles;
using PluginAPI.Events;
using UnityEngine;
using SCPSLAudioApi.AudioCore;
using MultiBroadcast;

namespace RGM.Modes
{
    class GGClub
    {
        public static GGClub Instance;

        public List<Player> pl = new List<Player>();
        public List<Transform> ClubLights = new List<Transform>();
        public List<Transform> Pads = new List<Transform>();
        public List<Transform> goldPads = new List<Transform>();

        public bool IsSongStopped = false;

        ReferenceHub dj;

        public void OnEnabled()
        {
            Round.IsLocked = true;

            Timing.RunCoroutine(OnModeStarted());
            Timing.RunCoroutine(gingerbreadHint());
            Timing.RunCoroutine(DJHeadBanging());
            Timing.RunCoroutine(DJ());

            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.SpawnedRagdoll += OnSpawnedRagdoll;
        }

        public IEnumerator<float> OnModeStarted()
        {
            Server.ExecuteCommand($"/mp load GGClub");

            dj = GGUtils.Gtool.Spawn(RoleTypeId.Tutorial, new Vector3(76.17068f, 1015.741f, -46.65614f));

            Dictionary<ReferenceHub, string> register = new Dictionary<ReferenceHub, string>()
            {
                { dj, "dj" }
            };

            foreach (var reg in register)
            {
                try
                {
                    GGUtils.Gtool.Register(reg.Key, reg.Value);
                }
                catch (Exception e)
                {
                }
            }

            GGUtils.Gtool.PlayerGet("dj").DisplayNickname = "DJ";
            GGUtils.Gtool.PlaySound("dj", "tothemoon", VoiceChat.VoiceChatChannel.Intercom, 25, true);

            yield return Timing.WaitForSeconds(1f);

            foreach (var player in Player.List.Where(x => !x.IsNPC).ToList())
            {
                player.Role.Set(RoleTypeId.Scp3114);
                player.Position = new Vector3(74.99881f, 1012.823f, -43.1801f);
            }

            Player.List.CopyTo(pl);

            ClubLights = GameObject.FindObjectsOfType<Transform>().Where(t => t.name == "ClubLight").ToList();
            Pads = GameObject.FindObjectsOfType<Transform>().Where(t => t.name == "Pad").ToList();

            AudioPlayerBase val = AudioPlayerBase.Get(dj);

            while (true)
            {
                yield return Timing.WaitForSeconds(UnityEngine.Random.Range(3, 12));

                IsSongStopped = true;

                for (int i = 1; i < UnityEngine.Random.Range(2, 5); i++)
                {
                    Transform goldPad = RGM.GetRandomValue(Pads);

                    if (!goldPads.Contains(goldPad))
                        goldPads.Add(goldPad);
                }

                yield return Timing.WaitForSeconds(1.8f);

                foreach (var player in Player.List)
                {
                    if (Physics.Raycast(player.Position, Vector3.down, out RaycastHit hit, 1f, (LayerMask)1))
                    {
                        try
                        {
                            if (hit.transform.GetComponent<PrimitiveObject>().Primitive.Color != Color.yellow)
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

                yield return Timing.WaitForSeconds(2.5f);

                IsSongStopped = false;
                goldPads.Clear();
            }
        }

        public IEnumerator<float> gingerbreadHint()
        {
            while (true)
            {
                foreach (var player in Player.List)
                    player.ShowHint($"<b>[TIP]</b> <i>gingerbread</i>를 입력하면 춤출 수 있습니다.", 1.2f);

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public IEnumerator<float> DJHeadBanging()
        {
            yield return Timing.WaitForSeconds(1f);

            bool HeadUp = true;

            while (true)
            {
                if (HeadUp)
                {
                    GGUtils.Gtool.Rotate(dj, new Vector3(0, -1f, 0));

                    HeadUp = false;

                    yield return Timing.WaitForSeconds(0.2f);
                }
                else
                {
                    GGUtils.Gtool.Rotate(dj, new Vector3(0, 1f, 0));

                    HeadUp = true;

                    yield return Timing.WaitForSeconds(0.15f);
                }
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
                        Pad.GetComponent<PrimitiveObject>().Primitive.Color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                    }

                    foreach (var ClubLight in ClubLights)
                    {
                        ClubLight.GetComponent<Light>().color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                    }
                }
                else
                {
                    foreach (var Pad in Pads)
                    {
                        Pad.GetComponent<PrimitiveObject>().Primitive.Color = Color.white;
                    }

                    foreach (var Pad in goldPads)
                    {
                        Pad.GetComponent<PrimitiveObject>().Primitive.Color = Color.yellow;
                    }

                    foreach (var ClubLight in ClubLights)
                    {
                        ClubLight.GetComponent<Light>().color = Color.white;
                    }
                }

                yield return Timing.WaitForSeconds(0.5f);
            }
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
    }
}
