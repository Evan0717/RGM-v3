using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Toys;
using HarmonyLib;
using MapEditorReborn.API.Features.Objects;
using MEC;
using Mirror;
using MultiBroadcast;
using site02;
using UnityEngine;
using Exiled.API.Enums;
using PlayerRoles.FirstPersonControl;
using PlayerRoles;
using RGM.API.Features;
using MultiBroadcast.API;
using MapEditorReborn.API.Features;
using Respawning;
using Exiled.API.Features.Items;
using CommandSystem.Commands.RemoteAdmin.Dummies;
using GameCore;
using Exiled.Events.Commands.Hub;
using RelativePositioning;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.SnowBallFight)]
    class SnowBallFight : Mode
    {
        public override string Name => "눈싸움";
        public override string Description => "[겨울 이벤트 전용] 눈싸움을 하세요!";
        public override string Detail =>
"""
개인전입니다. 눈싸움을 하세요!

가끔씩 하늘에서 무언가 떨어질 수 있어요.
2분 뒤 버스터콜이 진행됩니다.
""";
        public override string Color => "FFFFFF";

        public static SnowBallFight Instance;

        public override void OnEnabled()
        {
            Round.IsLocked = true;
            foreach (var spawn in WaveManager.Waves) spawn.Destroy();
            Server.FriendlyFire = true;

            Timing.RunCoroutine(OnModeStarted());
            Timing.RunCoroutine(CheckEnd());
        }

        public IEnumerator<float> OnModeStarted()
        {
            Server.ExecuteCommand("/mp load SnowBallFight");

            AudioPlayer audioPlayer = AudioPlayer.CreateOrGet($"Global AudioPlayer", onIntialCreation: (p) =>
            {
                Speaker speaker = p.AddSpeaker("Main", isSpatial: false, maxDistance: 5000f);
            });

            audioPlayer.AddClip("LobbyTheme", 0.3f, true);

            ReferenceHub dummy = DummyUtils.SpawnDummy("귀여운 땅콩이 ❤️");
            Player bot = Player.Get(dummy);

            dummy.roleManager.ServerSetRole(RoleTypeId.Scp173, RoleChangeReason.RemoteAdmin);
            dummy.TryOverridePosition(new Vector3(-100, 1000, -100), Vector3.zero);

            FirstPersonMovementModule fpcModule = (bot.ReferenceHub.roleManager.CurrentRole as FpcStandardRoleBase).FpcModule;
            fpcModule.Noclip.IsActive = true;

            Vector3 spawnPoint = GameObject.Find("SnowBallFightSpawnPoint").transform.position;
            Vector3 firstPoint = GameObject.Find("FirstPoint").transform.position;
            Vector3 secondPoint = GameObject.Find("SecondPoint").transform.position;
            Vector3 busterCallPoint = GameObject.Find("BusterCallPoint").transform.position;

            foreach (var player in Player.List)
            {
                player.Role.Set(RoleTypeId.ClassD);
                player.Position = spawnPoint;
            }

            float x1 = firstPoint.x;
            float y1 = firstPoint.y;
            float z1 = firstPoint.z;
            float x2 = secondPoint.x;
            float y2 = secondPoint.y;
            float z2 = secondPoint.z;

            Timing.CallDelayed(120, () =>
            {
                foreach (var player in Player.List)
                {
                    player.AddBroadcast(10, $"<b><size=25>2분이 지나 버스터콜이 발동되었습니다.</size></b>");
                    
                    if (player.IsAlive && !player.IsNPC)
                        player.Position = busterCallPoint;
                }
            });

            while (!Round.IsEnded)
            {
                foreach (var player in Player.List.Where(x => !x.HasItem(ItemType.Snowball)))
                {
                    player.AddItem(ItemType.Snowball);
                }

                if (UnityEngine.Random.Range(1, 31) == 1)
                {
                    Item Item = Item.Create(Tools.GetRandomValue(new List<ItemType>() { ItemType.Coal, ItemType.Snowball, ItemType.SpecialCoal }));

                    Item.CreatePickup(new Vector3(UnityEngine.Random.Range(x1, x2), UnityEngine.Random.Range(y1, y2), UnityEngine.Random.Range(z1, z2)));
                }

                yield return Timing.WaitForOneFrame;
            }
        }

        public IEnumerator<float> CheckEnd()
        {
            while (!Round.IsEnded)
            {
                if (Player.List.Where(x => x.IsAlive && !x.IsNPC).Count() < 2)
                {
                    List<Player> playerList = Player.List.Where(x => x.IsAlive && !x.IsNPC).ToList();

                    Round.IsLocked = false;

                    DummyUtils.DestroyAllDummies();

                    Player.List.ToList().ForEach(x => x.AddBroadcast(20, $"승리자 : {playerList[0].DisplayNickname}"));
                    Timing.RunCoroutine(Tools.SetWinner(new List<Player>() { playerList[0] }, 5));

                    yield break;
                }

                yield return Timing.WaitForSeconds(1);
            }
        }
    }
}
