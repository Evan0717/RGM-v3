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
using InventorySystem.Configs;
using Respawning;
using Exiled.Events.EventArgs.Server;

namespace RGM.EventArgs
{
    public static class ServerEvents
    {
        public static IEnumerator<float> OnWaitingForPlayers()
        {
            yield return Timing.WaitForSeconds(1f);

            foreach (var _audioClip in System.IO.Directory.GetFiles(Paths.Plugins + "/audio/"))
                AudioClipStorage.LoadClip(_audioClip, _audioClip.Replace(Paths.Plugins + "/audio/", "").Replace(".ogg", ""));

            InventoryLimits.StandardCategoryLimits[ItemCategory.SpecialWeapon] = 8;
            InventoryLimits.StandardCategoryLimits[ItemCategory.SCPItem] = 8;
            InventoryLimits.Config.RefreshCategoryLimits();

            UsersManager.LoadUsers();

            GlobalPlayer = AudioPlayer.CreateOrGet($"Global AudioPlayer", onIntialCreation: (p) =>
            {
                Speaker speaker = p.AddSpeaker("Main", isSpatial: false, maxDistance: 5000);
            });

            Tools.PlayGlobalAudio("Holiday by GoldenPig1205", 0.3f, true);

            Round.IsLobbyLocked = true;
            GameObject.Find("StartRound").transform.localScale = Vector3.zero;
            Tools.LoadMap($"Past_Lobby");

            var donator = new Donator.Main();
            donator.OnEnabled();

            First = Tools.GetObjectList("First");
            Second = Tools.GetObjectList("Second");
            Third = Tools.GetObjectList("Third");
            Fourth = Tools.GetObjectList("Fourth");
            Numbers = Tools.GetObjectList("Number");
            RandomColors = Tools.GetObjectList("RandomColor");
            RandomLights = Tools.GetObjectList("RandomLight");
            Balls = Tools.GetObjectList("Ball");

            PickModes();
            Balls.ForEach(x => x.gameObject.AddComponent<BallComponent>());
            EnabledModeList.Add(ModeType.Develop);

            Timing.RunCoroutine(SyncSpectatedHint());
            Timing.RunCoroutine(ThrowawayBroadcast());
            Timing.RunCoroutine(GameStartButton());
            Timing.RunCoroutine(ModeResetButton());
            Timing.RunCoroutine(IsFallDown());
            Timing.RunCoroutine(InputCooldown());
            Timing.RunCoroutine(Ball());
            Timing.RunCoroutine(RenewalPlayersInfo());
            Timing.RunCoroutine(HintManager.OnStarted());
            Timing.RunCoroutine(HintManager.RemoveHint());

            int rn = UnityEngine.Random.Range(1, 6);

            if (rn == 1)
            {
                SelectMode = "RandomSelect";
                Timing.RunCoroutine(RandomSelectMode());
            }
            else if (rn == 2)
            {
                SelectMode = "SimpleSelect";
            }
            else if (rn == 3)
            {
                SelectMode = "SecretVote";
            }
            else if (rn == 4)
            {
                SelectMode = "FightVote";

                Server.FriendlyFire = true;
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
            Server.FriendlyFire = false;

            Server.ExecuteCommand("/mp unload RGMLobby");
            Server.ExecuteCommand($"/speak {string.Join(".", Player.List.Select(x => x.Id))}. 0");
            IntercomPlayers.Clear();
            EnabledModeList.Clear();

            if (AudioPlayer.TryGet("Global AudioPlayer", out AudioPlayer ap))
                ap.RemoveAllClips();

            Tools.LoadMap($"RGMBase");

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
                                    p.AddBroadcast(10, $"<size=25><b>롤토체스 당첨자({player.DisplayNickname})</b>에 의해 모드가 {CurrentMode.GetModeData().Name}(으)로 선택되었습니다.</size>");
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

            Server.Name = Server.Name.Replace("[라운드 시작 전 로비]", $"[현재 모드: <color=#{CurrentMode.GetModeData().Color}>{CurrentMode.GetModeData().Name}</color>{(CurrentSubMode == ModeType.None ? "" : $"<size=25> + <color=#{CurrentSubMode.GetModeData().Color}>{CurrentSubMode.GetModeData().Name}</color></size>")}]");

            List<string> ModeDesc = Tools.GetModeDesc(CurrentMode, CurrentSubMode);

            foreach (var player in Player.List)
            {
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

            if (CurrentMode.GetModeData().Info == ModeInfo.Plus)
            {
                Timing.RunCoroutine(HumanLoop());
                Timing.RunCoroutine(Scp079Broadcast());
            }

            Timing.RunCoroutine(Detonation());

            DiscordInteraction.Discord.Webhook.Send($"시작된 모드 : {CurrentMode.GetModeData().Name}");
            Log.Info($"시작된 모드 : {CurrentMode.GetModeData().Name}");

            yield return Timing.WaitForSeconds(20 * 60);

            if (!Warhead.IsDetonated && CurrentMode.GetModeData().Type != ModeType.Develop)
            {
                AutoNuke = true;
                Warhead.Start();
                Server.ExecuteCommand("/cassie_sl <color=red>예정된 시설 자폭 프로세스가 시작되었습니다.</color> <b>대피하십시오.</b>");
            }
        }

        public static void OnRoundEnded(RoundEndedEventArgs ev)
        {
            if (CurrentMode.GetModeData().Info == ModeInfo.Plus)
            {
                IEnumerable<Player> players = Player.List.Where(x => x.IsAlive && !x.IsNPC);

                if (players.Count() == 1)
                    Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 5));

                else if (players.Count() > 1)
                    Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 1));
            }

            Tools.TryInstallMode(ModeType.FriendlyFire);

            foreach (var player in Player.List)
            {
                Server.ExecuteCommand($"/speak {player.Id} 1");
                IntercomPlayers.Add(player);
            }

            Timing.CallDelayed(19f, () =>
            {
                Server.ExecuteCommand("sr");
            });

            var top10 = PlayersReport
                .OrderByDescending(kv => kv.Value.Damage)
                .Take(10)
                .ToList();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<size=30><b>이번 라운드 TOP 10</b></size>");
            int rank = 1;
            
            string ranking(int rank)
            {
                if (rank == 1)
                    return "fffa66";

                else if (rank == 2)
                    return "808d8e";

                else if (rank == 3)
                    return "dfae4d";

                else
                    return "ffffff";
            }

            foreach (var kv in top10)
            {
                var userId = kv.Key;
                var report = kv.Value;

                if (Player.TryGet(userId, out Player player))
                {
                    sb.AppendLine($"<size=25><color=#{ranking(rank)}>{rank}.</color> {player.DisplayNickname} - {report.Kill}킬 / {report.Death}데스 / {report.Damage}뎀</size>");
                    rank++;
                }
            }

            foreach (var player in Player.List)
            {
                player.AddHint("라운드 요약", $"<align=left>{sb}</align>\n\n\n\n", 20);
            }
        }
    }
}
