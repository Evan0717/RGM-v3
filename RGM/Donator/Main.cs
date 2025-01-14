using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using PlayerRoles;
using RGM.API.Features;
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
using RGM.API.Interfaces;

using static RGM.Variables.ServerManagers;
using MultiBroadcast.API;
using RGM.API.DataBases;
using Exiled.API.Features.Roles;
using AdminToys;
using InventorySystem.Items.Firearms.ShotEvents;
using InventorySystem.Items.Firearms;
using InventorySystem.Items;

namespace RGM.Donator
{
    public class Main
    {
        public static Main Instance;

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Dying += OnDying;

            Timing.RunCoroutine(CustomermizingRotation());
        }

        public void PlaySound(Vector3 pos, string clip, float volume = 1)
        {
            AudioPlayer audioPlayer = AudioPlayer.CreateOrGet($"{UnityEngine.Random.Range(1, 10000001)}", onIntialCreation: (p) =>
            {
                Speaker speaker = p.AddSpeaker("Main", isSpatial: true, minDistance: 1, maxDistance: 5);

                speaker.transform.position = pos;
            });

            audioPlayer.AddClip($"KillEffect_{clip}", volume: volume);
        }

        public IEnumerator<float> KillEffect(List<string> PlayerData, Player Attacker, Player Player, Role _role, Vector3 _pos)
        {
            Quaternion rot = Attacker.Rotation;

            if (PlayerData[4] == "영혼 가출")
            {
                PlaySound(_pos, "1", 4);

                DamageHandlerBase DisruptorDamage = new DisruptorDamageHandler(new DisruptorShotEvent(ItemIdentifier.None, Attacker.Footprint, InventorySystem.Items.Firearms.Modules.DisruptorActionModule.FiringState.FiringSingle), Player.Position, -1);

                Ragdoll.CreateAndSpawn(_role.Type, PlayerData[4], DisruptorDamage, _pos, rot);
            }

            if (PlayerData[4] == "솔라 테라")
            {
                PlaySound(_pos, "2", 10);

                SchematicObject SolarTerra = ObjectSpawner.SpawnSchematic("SolarTerra", _pos, rot, null, null);

                Timing.CallDelayed(1.5f, SolarTerra.Destroy);
            }

            if (PlayerData[4] == "Kerfus")
            {
                PlaySound(_pos, "3", 10);

                SchematicObject Kerfus = ObjectSpawner.SpawnSchematic("Kerfusa", _pos + new Vector3(0, 19, 0), rot, null, null);

                List<PrimitiveObject> primitiveObjects = new List<PrimitiveObject>();

                void applyPrimitiveFlags(Transform parentTransform)
                {
                    foreach (Transform childTransform in parentTransform)
                    {
                        PrimitiveObject primitiveObject = childTransform.GetComponent<PrimitiveObject>();

                        if (primitiveObject != null)
                        {
                            primitiveObject.Base.PrimitiveFlags = PrimitiveFlags.Visible;
                            primitiveObject.UpdateObject();
                        }

                        applyPrimitiveFlags(childTransform);
                    }
                }

                applyPrimitiveFlags(Kerfus.transform);

                for (int i = 1; i < 11; i++)
                {
                    Kerfus.Position += new Vector3(0, -2f, 0);

                    yield return Timing.WaitForSeconds(0.05f);
                }

                yield return Timing.WaitForSeconds(1.5f);

                for (int i = 1; i < 11; i++)
                {
                    Kerfus.Position += new Vector3(0, 2f, 0);

                    yield return Timing.WaitForSeconds(0.05f);
                }

                Kerfus.Destroy();
            }

            if (PlayerData[4] == "은제 말뚝")
            {
                PlaySound(_pos, "4", 7.5f);

                SchematicObject SilverStake = ObjectSpawner.SpawnSchematic("SilverStake", _pos, rot, null, null);

                Timing.CallDelayed(1.5f, SilverStake.Destroy);
            }

            if (PlayerData[4] == "KO 사인")
            {
                PlaySound(_pos, "5", 7.5f);

                SchematicObject KO = ObjectSpawner.SpawnSchematic("KO", _pos, rot, null, null);

                Timing.CallDelayed(1.5f, KO.Destroy);
            }

            if (PlayerData[4] == "크리스마스 트리")
            {
                PlaySound(_pos, "6", 2);

                SchematicObject XmasTree = ObjectSpawner.SpawnSchematic("XmasTree", new Vector3(_pos.x, _pos.y - 0.9f, _pos.z), rot, null, null);

                Timing.CallDelayed(1.9f, XmasTree.Destroy);
            }

            if (PlayerData[4] == "크리스마스 볼")
            {
                PlaySound(_pos, "7", 2);

                SchematicObject XmasTree = ObjectSpawner.SpawnSchematic("XmasBall", new Vector3(_pos.x, _pos.y - 0.9f, _pos.z), rot, null, null);

                Timing.CallDelayed(1.9f, XmasTree.Destroy);
            }
        }

        public IEnumerator<float> OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            Role _role = ev.Player.Role;
            Vector3 _pos = ev.Player.Position;

            yield return Timing.WaitForOneFrame;

            if (ev.Player.IsDead)
            {
                if (ev.Attacker != null && UsersManager.UsersCache.ContainsKey(ev.Attacker.UserId))
                {
                    List<string> AttackerData = UsersManager.UsersCache[ev.Attacker.UserId];

                    if (AttackerData[4] != "0")
                    {
                        Timing.RunCoroutine(KillEffect(AttackerData, ev.Attacker, ev.Player, _role, _pos));

                        foreach (var _player in Player.List.Where(x => x.IsDead || Vector3.Distance(x.Position, _pos) <= 10 || Vector3.Distance(x.Position, ev.Attacker.Position) <= 10))
                            _player.AddBroadcast(6, $"<size=25>{Tools.BadgeFormat(ev.Attacker)}<color=#CEF6F5>{ev.Attacker.DisplayNickname}</color>(이)가 {Datas.KillEffectData[AttackerData[4]][0]}(으)로 {Tools.BadgeFormat(ev.Player)}<color=#CEF6F5>{ev.Player.DisplayNickname}</color>(을)를 {Datas.KillEffectData[AttackerData[4]][1]}시켰습니다!</size>");
                    }
                }
            }

            yield break;
        }

        public IEnumerator<float> CustomermizingRotation()
        {
            while (true)
            {
                try
                {
                    foreach (var player in Player.List.Where(x => !x.IsNPC))
                    {
                        if (UsersManager.UsersCache.ContainsKey(player.UserId))
                        {
                            List<string> userValues = UsersManager.UsersCache[player.UserId];

                            if (userValues[5] != "0")
                                player.DisplayNickname = Tools.CustomFormatter(player, userValues[5]);

                            else
                                player.DisplayNickname = "";

                            if (userValues[6] != "0")
                                player.CustomInfo = Tools.CustomFormatter(player, userValues[6]);

                            else
                                player.CustomInfo = "";
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
