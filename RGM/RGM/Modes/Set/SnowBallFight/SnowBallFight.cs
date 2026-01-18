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
using MEC;
using Mirror;
using MultiBroadcast;
using UnityEngine;
using Exiled.API.Enums;
using PlayerRoles.FirstPersonControl;
using PlayerRoles;
using RGM.API.Features;

using Respawning;
using Exiled.API.Features.Items;
using CommandSystem.Commands.RemoteAdmin.Dummies;
using GameCore;
using RelativePositioning;

using static RGM.Variables.Variable;
using NetworkManagerUtils.Dummies;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.SnowBallFight, ModeHoliday.Christmas)]
    public class SnowBallFight : Mode
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
        public override string Map => "SnowBallFight";

        public static SnowBallFight Instance;

        CoroutineHandle _onModeStarted;
        CoroutineHandle _checkEnd;

        AudioClipPlayback audio;

        public override void OnEnabled()
        {
            Round.IsLocked = true;
            Respawn.PauseWaves();
            Server.FriendlyFire = true;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
            _checkEnd = Timing.RunCoroutine(CheckEnd());

            audio = Tools.PlayGlobalAudio("ChristmasRock", 0.3f, true);
        }

        public override void OnDisabled()
        {
            Round.IsLocked = false;
            Respawn.ResumeWaves();
            Server.FriendlyFire = false;

            Timing.KillCoroutines(_onModeStarted);
            Timing.KillCoroutines(_checkEnd);

            audio.IsPaused = true;
        }

        public IEnumerator<float> OnModeStarted()
        {
            ReferenceHub dummy = DummyUtils.SpawnDummy("귀여운 땅콩이 ❤️");
            Player bot = Player.Get(dummy);

            dummy.roleManager.ServerSetRole(RoleTypeId.Scp173, RoleChangeReason.RemoteAdmin);
            dummy.TryOverridePosition(new Vector3(-100, 1000, -100));

            FirstPersonMovementModule fpcModule = (bot.ReferenceHub.roleManager.CurrentRole as FpcStandardRoleBase).FpcModule;
            fpcModule.Noclip.IsActive = true;

            Vector3 spawnPoint = GameObject.Find("SnowBallFightSpawnPoint").transform.position;
            Vector3 firstPoint = GameObject.Find("FirstPoint").transform.position;
            Vector3 secondPoint = GameObject.Find("SecondPoint").transform.position;
            Vector3 busterCallPoint = GameObject.Find("BusterCallPoint").transform.position;

            foreach (var player in PlayerManager.List)
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
                foreach (var player in PlayerManager.List)
                {
                    player.AddBroadcast(10, $"<b><size=25>2분이 지나 버스터콜이 발동되었습니다.</size></b>");
                    
                    if (player.IsAlive && !player.IsNPC)
                        player.Position = busterCallPoint;
                }
            });

            while (!Round.IsEnded)
            {
                foreach (var player in PlayerManager.List.Where(x => !x.HasItem(ItemType.Snowball)))
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
                if (PlayerManager.List.Where(x => x.IsAlive && !x.IsNPC).Count() < 2)
                {
                    List<Player> playerList = PlayerManager.List.Where(x => x.IsAlive && !x.IsNPC).ToList();

                    Round.IsLocked = false;

                    DummyUtils.DestroyAllDummies();

                    PlayerManager.List.ToList().ForEach(x => x.AddBroadcast(20, $"승리자 : {playerList[0].DisplayNickname}"));
                    Timing.RunCoroutine(Tools.SetWinner(new List<Player>() { playerList[0] }, 5));

                    yield break;
                }

                yield return Timing.WaitForSeconds(1);
            }
        }
    }
}
