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
using MapEditorReborn.Commands.ModifyingCommands.Rotation;
using RGM.API.Interfaces;

using static RGM.Variables.ServerManagers;
using MultiBroadcast.API;
using RGM.API.DataBases;
using Exiled.API.Features.Roles;

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

        public IEnumerator<float> KillEffect(List<string> PlayerData, Player Attacker, Player Player, Role _role, Vector3 _pos)
        {
            Quaternion Rotation = new Quaternion(0, Attacker.CameraTransform.rotation.y + 180, 0, 0);

            if (PlayerData[4] == "영혼 가출")
            {
                DamageHandlerBase DisruptorDamage = new DisruptorDamageHandler(Attacker.Footprint, -1);

                Ragdoll.CreateAndSpawn(_role.Type, PlayerData[4], DisruptorDamage, _pos, Rotation);
            }

            if (PlayerData[4] == "솔라 테라")
            {
                SchematicObject SolarTerra = ObjectSpawner.SpawnSchematic("SolarTerra", _pos, Rotation, isStatic: false);

                Timing.CallDelayed(1.5f, SolarTerra.Destroy);
            }

            if (PlayerData[4] == "Kerfus")
            {
                SchematicObject Kerfus = ObjectSpawner.SpawnSchematic("Kerfusa", _pos + new Vector3(0, 19, 0), Rotation, isStatic: false);

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
                SchematicObject SilverStake = ObjectSpawner.SpawnSchematic("SilverStake", _pos, Rotation, isStatic: false);

                Timing.CallDelayed(1.5f, SilverStake.Destroy);
            }

            if (PlayerData[4] == "KO 사인")
            {
                SchematicObject KO = ObjectSpawner.SpawnSchematic("KO", _pos, Rotation, isStatic: false);

                Timing.CallDelayed(1.5f, KO.Destroy);
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
