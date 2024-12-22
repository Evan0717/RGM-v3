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

using static RGM.Variables.ServerManagers;

using static RGM.Functions.ModeManagers;

using static RGM.IEnumerators.LobbyManagers;
using static RGM.IEnumerators.ServerManagers;

using Exiled.API.Enums;
using RGM.API.DataBases;

namespace RGM.EventArgs
{
    public static class ServerEvents
    {
        public static IEnumerator<float> OnWaitingForPlayers()
        {
            UsersManager.LoadUsers();

            Round.IsLobbyLocked = true;
            GameObject.Find("StartRound").transform.localScale = Vector3.zero;
            Server.ExecuteCommand($"/mp load RGMLobby");

            var donator = new Donator.Main();
            donator.OnEnabled();

            First = Tools.GetObjectList("First");
            Second = Tools.GetObjectList("Second");
            Third = Tools.GetObjectList("Third");
            Numbers = Tools.GetObjectList("Number");
            RandomColors = Tools.GetObjectList("RandomColor");
            Balls = Tools.GetObjectList("Ball");

            PickModes();
            Balls.ForEach(x => x.gameObject.AddComponent<BallComponent>());

            Timing.RunCoroutine(SyncSpectatedHint());
            Timing.RunCoroutine(ThrowawayBroadcast());
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

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public static IEnumerator<float> OnRoundStarted()
        {
            Server.ExecuteCommand("/mp unload RGMLobby");

            Server.ExecuteCommand($"/speak {string.Join(".", Player.List.Select(x => x.Id))}. 0");

            MapEditorReborn.API.Features.ObjectSpawner.SpawnSchematic("glass", new Vector3(-39.88f, 990.92f, -36.14f), isStatic: false);

            if (CurrentMode == ModeType.None)
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
                            CurrentSubMode = SubModeVote[ModeVote.Keys.ToList().IndexOf(CurrentMode)];

                            Timing.CallDelayed(1f, () =>
                            {
                                foreach (var p in Player.List)
                                    p.AddBroadcast(10, $"<size=25><b>간이 셀렉트 당첨자({player.Nickname})</b>에 의해 모드가 {CurrentMode.GetModeData().Name}(으)로 선택되었습니다.</size>");
                            });
                        }
                    }
                }
                finally
                {
                    if (!ModeList.ContainsKey(CurrentMode))
                    {
                        CurrentMode = Tools.GetRandomValue(ModeList.Keys.Where(x => ModeList[x].Category == ModeCategory.Public).ToList());

                        foreach (var p in Player.List)
                            p.AddBroadcast(10, $"<size=25><b>알 수 없는 이유로 모드가 선택되지 않았으므로, 모드가 랜덤으로 선택되었습니다.</size>");
                    }
                }
            }

            List<string> ModeDesc = Tools.GetModeDesc(CurrentMode, CurrentSubMode);

            foreach (var player in Player.List)
            {
                player.ClearPlayerBroadcasts();
                player.AddBroadcast(10, ModeDesc[0]);

                player.SendConsoleMessage($"\n{ModeDesc[0].Replace("\n", "\n")}", "white");
                if (ModeDesc[3] == "")
                    player.SendConsoleMessage($"\n{ModeDesc[2]}", "white");

                else
                    player.SendConsoleMessage($"\n{ModeDesc[3]}", "white");
            }

            Tools.TryInstallMode(CurrentMode);

            if (CurrentSubMode != ModeType.None)
                Tools.TryInstallMode(CurrentSubMode);

            if (StartupRandom == 3)
                Tools.CallSnakeHand(null, Player.List.Where(x => x.Role == RoleTypeId.FacilityGuard).ToList());

            DiscordInteraction.Discord.Webhook.Send($"시작된 모드 : {CurrentMode.GetModeData().Name}");
            Log.Info($"시작된 모드 : {CurrentMode.GetModeData().Name}");

            yield return Timing.WaitForSeconds(20 * 60);

            if (Warhead.IsDetonated)
            {
                AutoNuke = true;
                Server.ExecuteCommand("/cassie_sl 시간이 너무 오래 걸립니다! 모두의 체력이 초당 1%씩 줄어듭니다!");

                while (true)
                {
                    Player.List.ToList().ForEach(x => x.Health -= (x.MaxHealth / 100));

                    yield return Timing.WaitForSeconds(1);
                }
            }
            else
            {
                AutoNuke = true;
                Warhead.Start();
                Server.ExecuteCommand("/cassie_sl <color=red>예정된 시설 자폭 프로세스가 시작되었습니다.</color> <b>대피하십시오.</b>");
            }

            yield return Timing.WaitForSeconds(300);

            AutoNuke = true;
            Server.ExecuteCommand("/cassie_sl 시간이 너무 오래 걸립니다! 모두의 체력이 초당 5%씩 줄어듭니다!");

            Player.List.ToList().ForEach(x => x.EnableEffect(EffectType.PocketCorroding));

            while (true)
            {
                foreach (var player in Player.List)
                {
                    player.Health -= player.MaxHealth / 20;

                    if (player.Health <= 0 && player.IsAlive)
                        player.Kill("게임을 질질 끌어서 죽었습니다.");
                }

                yield return Timing.WaitForSeconds(1);
            }
        }

        public static void OnRoundEnded(Exiled.Events.EventArgs.Server.RoundEndedEventArgs ev)
        {
            Tools.TryInstallMode(ModeType.FriendlyFire);

            foreach (var player in Player.List)
                Server.ExecuteCommand($"/speak {player.Id} 1");

            if (CurrentMode.GetModeData().Info == ModeInfo.Plus || new List<ModeType>() 
            {
                ModeType.DeathRun
            }.Contains(CurrentMode.GetModeData().Type)
            )
            {
                IEnumerable<Player> players = Player.List.Where(x => x.IsAlive && !x.IsNPC);

                if (players.Count() == 1)
                    Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 5));

                else if (players.Count() > 1)
                    Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 1));
            }

            Timing.CallDelayed(19f, () =>
            {
                Server.ExecuteCommand("sr");
            });
        }
    }
}
