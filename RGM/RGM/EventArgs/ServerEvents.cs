using MEC;
using PlayerRoles;
using RGM.API.Components;
using RGM.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Exiled.API.Features;
using MultiBroadcast.API;

using static RGM.Variables.Protocol;
using static RGM.Variables.ServerManagers;

using static RGM.Functions.ModeManagers;

using static RGM.IEnumerators.LobbyManagers;
using static RGM.IEnumerators.ServerManagers;

using static RGM.Modes.ABattleFunctions.SpecificAbilities;

namespace RGM.EventArgs
{
    public static class ServerEvents
    {
        public static async void OnWaitingForPlayers()
        {
            UsersManager.LoadUsers();

            Round.IsLobbyLocked = true;
            GameObject.Find("StartRound").transform.localScale = Vector3.zero;
            Server.ExecuteCommand($"/mp load RGMLobby");

            var webhook = new Discord.Webhook();
            webhook.OnEnabled();

            var command = new Discord.Command();
            command.OnEnabled();

            var donator = new Donator.Main();
            donator.OnEnabled();

            First = GameObject.FindObjectsOfType<Transform>().Where(t => t.name == "First").ToList();
            Second = GameObject.FindObjectsOfType<Transform>().Where(t => t.name == "Second").ToList();
            Third = GameObject.FindObjectsOfType<Transform>().Where(t => t.name == "Third").ToList();
            Numbers = GameObject.FindObjectsOfType<Transform>().Where(t => t.name == "Number").ToList();
            RandomColors = GameObject.FindObjectsOfType<Transform>().Where(t => t.name == "RandomColor").ToList();
            Balls = GameObject.FindObjectsOfType<Transform>().Where(t => t.name == "Ball").ToList();

            PickModes();
            Balls.ForEach(x => x.gameObject.AddComponent<BallComponent>());

            Timing.RunCoroutine(SendHeartbeat());
            Timing.RunCoroutine(GameStartButton());
            Timing.RunCoroutine(ModeResetButton());
            Timing.RunCoroutine(IsFallDown());
            Timing.RunCoroutine(ChattingCooldown());
            Timing.RunCoroutine(Ball());
            Timing.RunCoroutine(RenewalPlayersInfo());

            int rn = UnityEngine.Random.Range(1, 11);

            if (rn == 1)
            {
                SelectMode = "RandomSelect";
                Timing.RunCoroutine(RandomSelectMode());
            }
            else if (rn == 2)
            {
                SelectMode = "SimpleSelect";
            }
            else
            {
                SelectMode = "MostVote";
            }

            while (true)
            {
                UsersManager.LoadUsers();

                await Task.Delay(1000);
            }
        }

        public static async void OnRoundStarted()
        {
            Server.ExecuteCommand("/mp unload RGMLobby");

            Server.ExecuteCommand($"/speak {string.Join(".", Player.List.Select(x => x.Id))}. 0");

            if (CurrentMode == null)
            {
                try
                {
                    var maxLength = ModeVote.Values.Max(list => list.Count);
                    var longestKeys = ModeVote.Keys.Where(key => ModeVote[key].Count == maxLength).ToList();
                    var randomKey = longestKeys[UnityEngine.Random.Range(0, longestKeys.Count)];
                    CurrentMode = randomKey;
                    CurrentSubMode = SubModeVote[ModeVote.Keys.ToList().IndexOf(randomKey)];

                    if (SelectMode == "SimpleSelect")
                    {
                        List<Player> mergedPlayers = ModeVote.Values.SelectMany(x => x).ToList();
                        List<Player> filiteredPlayers = mergedPlayers.Where(mergedPlayers.Contains).ToList();

                        if (filiteredPlayers.Count > 0)
                        {
                            Player player = Tools.GetRandomValue(filiteredPlayers);
                            CurrentMode = ModeVote.FirstOrDefault(x => x.Value.Contains(player)).Key;

                            Timing.CallDelayed(1f, () =>
                            {
                                foreach (var p in Player.List)
                                    p.AddBroadcast(10, $"<size=25><b>간이 셀렉트 당첨자({player.Nickname})</b>에 의해 모드가 {CurrentMode}(으)로 선택되었습니다.</size>");
                            });
                        }
                    }
                }
                finally
                {
                    if (!ModeList.ContainsKey(CurrentMode))
                    {
                        CurrentMode = Tools.GetRandomValue(ModeList.Keys.Where(x => ModeList[x][3] == "public").ToList());

                        foreach (var p in Player.List)
                            p.AddBroadcast(10, $"<size=25><b>알 수 없는 이유로 모드가 선택되지 않았으므로, 모드가 랜덤으로 선택되었습니다.</size>");
                    }
                }
            }

            string ModeColor = ModeList[CurrentMode][0];
            string ModeDescription = ModeList[CurrentMode][1];
            string ModeFileName = ModeList[CurrentMode][2];
            string ModeDescriptionDetail = ModeList[CurrentMode][5];

            Log.Info($"이번 라운드의 모드 : [{CurrentMode}]");

            string Message = Notions.StartModeDescription
                .Replace("{ModeColor}", ModeColor)
                .Replace("{CurrentMode}", CurrentMode)
                .Replace("{CurrentSubMode}", CurrentSubMode != null ? $"<size=16>추가된 서브 모드 : <color=#{ModeList[CurrentSubMode][0]}>{CurrentSubMode}</color></size>\n" : "")
                .Replace("{ModeDescription}", ModeDescription);

            foreach (var player in Player.List)
            {
                player.ClearPlayerBroadcasts();
                player.AddBroadcast(10, Message);

                player.SendConsoleMessage($"\n{Message.Replace("\n", "\n")}", "white");
                if (ModeDescriptionDetail == "")
                    player.SendConsoleMessage($"\n해당 모드에 대한 자세한 설명이 없습니다.", "white");

                else
                    player.SendConsoleMessage($"\n{ModeDescriptionDetail}", "white");
            }

            Tools.TryInstallMode(ModeFileName);

            if (CurrentSubMode != null)
                Tools.TryInstallMode(CurrentSubMode);

            if (StartupRandom == 3)
                CallSnakeHand(null, Player.List.Where(x => x.Role == RoleTypeId.FacilityGuard).ToList());

            await Task.Delay(20 * 60 * 1000);

            if (Warhead.IsDetonated)
            {
                AutoNuke = true;
                Server.ExecuteCommand("/cassie_sl 시간이 너무 오래 걸립니다! 모두의 체력이 초당 1%씩 줄어듭니다!");

                while (true)
                {
                    Player.List.ToList().ForEach(x => x.Health -= (x.MaxHealth / 100));
                    await Task.Delay(1000);
                }
            }
            else
            {
                AutoNuke = true;
                Warhead.Start();
                Server.ExecuteCommand("/cassie_sl <color=red>예정된 시설 자폭 프로세스가 시작되었습니다.</color> <b>대피하십시오.</b>");
            }

            await Task.Delay(300 * 1000);

            AutoNuke = true;
            Server.ExecuteCommand("/cassie_sl 시간이 너무 오래 걸립니다! 모두의 체력이 초당 1%씩 줄어듭니다!");

            while (true)
            {
                foreach (var player in Player.List)
                {
                    player.Health -= player.MaxHealth / 100;

                    if (player.Health <= 0 && player.IsAlive)
                        player.Kill("게임을 질질 끌어서 죽었습니다.");
                }

                await Task.Delay(1000);
            }
        }

        public static void OnRoundEnded(Exiled.Events.EventArgs.Server.RoundEndedEventArgs ev)
        {
            try
            {
                foreach (var player in Player.List.Where(x => !x.IsNPC))
                {
                    UsersManager.UsersCache[player.UserId][0] = (int.Parse(UsersManager.UsersCache[player.UserId][0]) + 1).ToString();
                    UsersManager.UsersCache[player.UserId][1] = (int.Parse(UsersManager.UsersCache[player.UserId][1]) + 1).ToString();
                }

                UsersManager.SaveUsers();

                Tools.TryInstallMode("어제의 동지는 오늘의 적");
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            Timing.CallDelayed(19f, () =>
            {
                Server.ExecuteCommand("sr");
            });
        }
    }
}
