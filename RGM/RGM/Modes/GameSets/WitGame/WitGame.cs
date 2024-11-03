using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using HarmonyLib;
using MEC;
using Mirror;
using MultiBroadcast;
using UnityEngine;
using Exiled.API.Enums;
using PlayerRoles;
using MultiBroadcast.API;
using RGM.API.Features;
using MapEditorReborn.API.Features;

using static RGM.Variables.ServerManagers;

namespace RGM.Modes
{
    class WitGame
    {
        public static WitGame Instance;

        public int Stack = 0;
        public int Remain = 0;
        public List<Player> JumpingPlayers = new List<Player>();
        public List<Player> PassPlayers = new List<Player>();

        public void OnEnabled()
        {
            Round.IsLocked = true;
            Respawn.TimeUntilNextPhase = 10000;

            Exiled.Events.Handlers.Player.Jumping += OnJumping;

            Timing.RunCoroutine(OnModeStarted());
            Timing.RunCoroutine(JumpHint());
        }

        public IEnumerator<float> OnModeStarted()
        {
            MapUtils.LoadMap("wg");

            foreach (var player in Player.List)
            {
                player.Role.Set(RoleTypeId.ClassD);
                player.EnableEffect(EffectType.Ensnared);
            }

            void SetPlayerLocation()
            {
                int playerCount = Player.List.Where(x => x.IsAlive).ToList().Count;
                float distance = 0f;

                distance = 1f + (playerCount - 2) * 0.3f;

                List<Vector3> platforms = Tools.GetCirclePoints(new Vector3(50.21484f, 1032.36f, -40.09766f), distance, playerCount);

                foreach (var player in Player.List.Where(x => x.IsAlive))
                    player.Position = platforms[Player.List.Where(x => x.IsAlive).ToList().IndexOf(player)];

                Stack = 0;

                PassPlayers.Clear();
            }

            while (Player.List.Where(x => x.IsAlive).ToList().Count > 1)
            {
                if (Player.List.Where(x => x.IsAlive).ToList().Count < 2)
                    break;

                if (JumpingPlayers.Where(x => x.IsAlive && x != null).ToList().Count > 0)
                {
                    if (JumpingPlayers.Where(x => x.IsAlive && x != null).ToList().Count > 1)
                    {
                        foreach (var boom in JumpingPlayers.Where(x => x.IsAlive && x != null))
                            Server.ExecuteCommand($"/rocket {boom.Id} 0.5");

                        foreach (var player in Player.List)
                            player.AddBroadcast(3, $"<size=25>마음이 너무 잘 맞았던 {JumpingPlayers.Where(x => x.IsAlive && x != null).ToList().Count}명({string.Join(", ", JumpingPlayers.Where(x => x.IsAlive && x != null).Select(x => x.DisplayNickname))})은 사이좋게 하늘로 갔습니다.</size>");

                        for (int i = 1; i < 4; i++)
                        {
                            foreach (var player in Player.List) player.AddBroadcast(1, $"<b>{i}</b>");

                            yield return Timing.WaitForSeconds(1f);
                        }
                    }
                    else
                    {
                        PassPlayers.Add(JumpingPlayers.Where(x => x.IsAlive && x != null).ToList()[0]);

                        Stack++;

                        foreach (var player in Player.List)
                            player.AddBroadcast(3, $"<size=20>{JumpingPlayers.Where(x => x.IsAlive && x != null).ToList()[0].DisplayNickname}, {Stack}!</size>");
                    }
                }

                if (PassPlayers.Count > 0)
                {
                    List<Player> Cowards = Player.List.Where(x => x.IsAlive && !PassPlayers.Contains(x)).ToList();

                    if (Cowards.Count < 2)
                    {
                        Server.ExecuteCommand($"/rocket {Cowards[0].Id} 0.5");

                        foreach (var player in Player.List)
                            player.AddBroadcast(3, $"<size=25>눈치가 느린 {Cowards[0].DisplayNickname}은(는) 하늘로 갔습니다.</size>");

                        for (int i = 1; i < 4; i++)
                        {
                            foreach (var player in Player.List) player.AddBroadcast(1, $"<b>{i}</b>");

                            yield return Timing.WaitForSeconds(1f);
                        }
                    }
                }

                if (Remain != Player.List.Where(x => x.IsAlive).ToList().Count)
                {
                    Remain = Player.List.Where(x => x.IsAlive).ToList().Count;

                    SetPlayerLocation();
                }

                JumpingPlayers.Clear();

                yield return Timing.WaitForSeconds(0.5f);
            }

            Round.IsLocked = false;

            Player.List.ToList().ForEach(x => x.AddBroadcast(20, $"<size=25>🎉 <b><color=yellow>{Player.List.Where(x => x.IsAlive).ToList()[0].Nickname}</color></b>(이)가 <color=#81F79F>눈치게임</color>에서 우승했습니다! 🎉</size>"));

            yield break;
        }

        public IEnumerator<float> JumpHint()
        {
            while (true)
            {
                foreach (var player in Player.List.Where(x => x.IsAlive && !PassPlayers.Contains(x)))
                    player.ShowHint("Spaceㅣ점프를 뛰어 눈치를 챙기세요.", 1.2f);

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public void OnJumping(Exiled.Events.EventArgs.Player.JumpingEventArgs ev)
        {
            if (!PassPlayers.Contains(ev.Player))
                JumpingPlayers.Add(ev.Player);
        }
    }
}
