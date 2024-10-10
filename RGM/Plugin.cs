using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using UnityEngine;
using MapEditorReborn.API.Features.Objects;
using MultiBroadcast.API;
using Exiled.API.Enums;
using RGM.Features;
using MapEditorReborn.Events.Handlers;
using RGM.API;
using Discord;
using HarmonyLib;
using PlayerRoles.Visibility;

namespace RGM
{
    public class RGM : Plugin<Config>
    {
        public static RGM Instance;

        public static string WebhookURL;
        public static string BotAPIServer;

        public string CurrentMode = null;
        public string SelectMode = null;
        public string Tip = Tools.GetRandomValue(Tips.LobbyTips);
        public int StartupRandom = UnityEngine.Random.Range(1, 21);
        public bool FreezeGameStart = false;
        public bool AutoNuke = false;
        public bool IsScp3114Enabled = false;

        public Dictionary<string, List<string>> ModeList;
        public Dictionary<string, List<Player>> ModeVote = new Dictionary<string, List<Player>>();
        public Dictionary<Player, float> OnGround = new Dictionary<Player, float>();
        public Dictionary<Player, Room> CurrentRoom = new Dictionary<Player, Room>();
        public List<Player> GodModePlayers = new List<Player>();
        public List<Player> ChatCooldown = new List<Player>();

        public List<string> Requests = new List<string>();

        public Dictionary<string, string> KillEffects = new Dictionary<string, string>()
        {
            {"영혼 가출", "죽은 상대에게서 혼을 추출해냅니다!"},
            {"솔라 테라", "죽음에 햇빛 한 점 들기를.."},
            {"Kerfus", "귀여운 로봇으로 도장을 찍어보세요."},
            {"은제 말뚝", "비수를 꽂는 것처럼 소름끼칩니다!"},
            {"KO 사인", "넉 다운! 상대를 쓰러트리세요!"}
        };

        List<Transform> First;
        List<Transform> Second;
        List<Transform> Third;
        List<Transform> Numbers;
        List<Transform> RandomColors;
        List<Transform> Balls;

        public void PickModes()
        {
            ModeVote.Clear();

            for (int i = 1; i < 4; i++)
            {
                var StaticModeList = ModeList.Keys.Where(x => ModeList[x][3] != "private" && !ModeVote.ContainsKey(x)).ToList();
                var mode = StaticModeList[UnityEngine.Random.Range(0, StaticModeList.Count())];
                ModeVote.Add(mode, new List<Player>());
            }

            List<List<Transform>> Pads = new List<List<Transform>>() { First, Second, Third };

            for (int i = 0; i < 3; i++)
            {
                foreach (var Pad in Pads[i])
                    Pad.GetComponent<PrimitiveObject>().Primitive.Color = ColorUtility.TryParseHtmlString("#" + ModeList[ModeVote.Keys.ToList()[i]][0], out Color color) ? color : Color.white;
            }

            Color randomColor = Tools.GetRandomColor(true);

            Numbers.ForEach(x => x.GetComponent<PrimitiveObject>().Primitive.Color = randomColor);
            RandomColors.ForEach(x => x.GetComponent<PrimitiveObject>().Primitive.Color = randomColor);
            Balls.ForEach(x => x.GetComponent<PrimitiveObject>().Primitive.Color = Tools.GetRandomColor(true));
        }

        public override void OnEnabled()
        {
            Instance = this;
            base.OnEnabled();

            WebhookURL = Config.WebhookURL;
            BotAPIServer = Config.BotAPIServer;
            ModeList = ModeManager.Modes;

            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Player.Left += OnLeft;
            Exiled.Events.Handlers.Player.SpawningRagdoll += OnSpawningRagdoll;
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.Dying += OnDying;

            Exiled.Events.Handlers.Warhead.Stopping += OnStopping;
            Exiled.Events.Handlers.Warhead.Detonating += OnDetonating;

            Exiled.Events.Handlers.Scp330.InteractingScp330 += OnInteractingScp330;

            Exiled.Events.Handlers.Scp244.UsingScp244 += OnUsingScp244;
            Exiled.Events.Handlers.Scp244.OpeningScp244 += OnOpeningScp244;

            Exiled.Events.Handlers.Scp079.Recontained += OnRecontained;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Player.Left -= OnLeft;
            Exiled.Events.Handlers.Player.SpawningRagdoll -= OnSpawningRagdoll;
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Player.Dying -= OnDying;

            Exiled.Events.Handlers.Warhead.Stopping -= OnStopping;
            Exiled.Events.Handlers.Warhead.Detonating -= OnDetonating;

            Exiled.Events.Handlers.Scp330.InteractingScp330 -= OnInteractingScp330;

            Exiled.Events.Handlers.Scp244.UsingScp244 -= OnUsingScp244;
            Exiled.Events.Handlers.Scp244.OpeningScp244 -= OnOpeningScp244;

            Exiled.Events.Handlers.Scp079.Recontained -= OnRecontained;

            base.OnDisabled();
            Instance = null;
        }

        public async void OnWaitingForPlayers()
        {
            UsersManager.LoadUsers();

            Round.IsLobbyLocked = true;
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

            Timing.RunCoroutine(GameStartButton());
            Timing.RunCoroutine(ModeResetButton());
            Timing.RunCoroutine(IsFallDown());
            Timing.RunCoroutine(ChattingCooldown());
            Timing.RunCoroutine(Ball());

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

        public async void OnRoundStarted()
        {
            Server.ExecuteCommand("/mp unload RGMLobby");
            
            foreach (var player in Player.List)
                Server.ExecuteCommand($"/speak {player.Id} disable");
            
            if (CurrentMode == null)
            {
                var maxLength = ModeVote.Values.Max(list => list.Count);
                var longestKeys = ModeVote.Keys.Where(key => ModeVote[key].Count == maxLength).ToList();
                var randomKey = longestKeys[UnityEngine.Random.Range(0, longestKeys.Count)];
                CurrentMode = randomKey;

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

            string ModeColor = ModeList[CurrentMode][0];
            string ModeDescription = ModeList[CurrentMode][1];
            string ModeFileName = ModeList[CurrentMode][2];
            string ModeDescriptionDetail = ModeList[CurrentMode][5];

            Log.Info($"이번 라운드의 모드 : [{CurrentMode}]");

            string Message = Notions.StartModeDescription
                .Replace("{ModeColor}", ModeColor)
                .Replace("{CurrentMode}", CurrentMode)
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

            var modeType = Type.GetType($"RGM.Modes.{ModeFileName}");
            if (modeType != null)
            {
                var modeInstance = Activator.CreateInstance(modeType);
                var onEnabledMethod = modeType.GetMethod("OnEnabled");
                onEnabledMethod?.Invoke(modeInstance, null);
            }

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

        public async void OnRoundEnded(Exiled.Events.EventArgs.Server.RoundEndedEventArgs ev)
        {
            try
            {
                // 라운드 종료 시 Exp, RP 1씩 지급
                foreach (var player in Player.List.Where(x => !x.IsNPC))
                {
                    UsersManager.UsersCache[player.UserId][0] = (int.Parse(UsersManager.UsersCache[player.UserId][0]) + 1).ToString();
                    UsersManager.UsersCache[player.UserId][1] = (int.Parse(UsersManager.UsersCache[player.UserId][1]) + 1).ToString();
                }

                UsersManager.SaveUsers();
            }
            catch (Exception ex)
            {
                ServerConsole.AddLog(ex.ToString());
            }

            try
            {
                var modeType = Type.GetType($"RGM.Modes.FriendlyFire");
                if (modeType != null)
                {
                    var modeInstance = Activator.CreateInstance(modeType);
                    var onEnabledMethod = modeType.GetMethod("OnEnabled");
                    onEnabledMethod?.Invoke(modeInstance, null);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            await Task.Delay(19000);

            Server.ExecuteCommand("sr");
        }

        public async void OnVerified(Exiled.Events.EventArgs.Player.VerifiedEventArgs ev)
        {
            List<string> DefaultValues = Enumerable.Repeat("0", 10).ToList();

            if (!UsersManager.UsersCache.ContainsKey(ev.Player.UserId))
            {
                UsersManager.AddUser(ev.Player.UserId, DefaultValues);

                UsersManager.SaveUsers();
            }
            else
            {
                List<string> userValues = UsersManager.UsersCache[ev.Player.UserId];

                if (userValues.Count < DefaultValues.Count)
                {
                    int diff = DefaultValues.Count - userValues.Count;

                    for (int i = 0; i < diff; i++)
                        userValues.Add("0");

                    UsersManager.SaveUsers();
                }

                if (userValues[5] != "0")
                    ev.Player.DisplayNickname = userValues[5];

                if (userValues[6] != "0")
                    ev.Player.CustomInfo = userValues[6];
            }

            OnGround.Add(ev.Player, 5);

            if (Round.IsStarted)
            {
                string ModeColor = ModeList[CurrentMode][0];
                string ModeDescription = ModeList[CurrentMode][1];
                string ModeFileName = ModeList[CurrentMode][2];
                string ModeDescriptionDetail = ModeList[CurrentMode][5];

                string Message = Notions.LateJoinModeDescription
                .Replace("{ModeColor}", ModeColor)
                .Replace("{CurrentMode}", CurrentMode)
                .Replace("{ModeDescription}", ModeDescription);

                ev.Player.AddBroadcast(10, Message);

                ev.Player.SendConsoleMessage($"\n{Message.Replace("\n", "\n")}", "white");
                if (ModeDescriptionDetail == "")
                    ev.Player.SendConsoleMessage($"\n해당 모드에 대한 자세한 설명이 없습니다.", "white");

                else
                    ev.Player.SendConsoleMessage($"\n{ModeDescriptionDetail}", "white");
            }
            else
            {
                Server.ExecuteCommand($"/speak {ev.Player.Id} enable");

                List<RoleTypeId> Scps = new List<RoleTypeId>() 
                {
                    RoleTypeId.Scp173,
                    RoleTypeId.Scp049,
                    RoleTypeId.Scp0492,
                    RoleTypeId.Scp106,
                    RoleTypeId.Scp939,
                    RoleTypeId.Scp3114
                };

                List<RoleTypeId> Humans = new List<RoleTypeId>()
                {
                    RoleTypeId.ClassD,
                    RoleTypeId.Scientist,
                    RoleTypeId.FacilityGuard,
                    RoleTypeId.ChaosConscript,
                    RoleTypeId.NtfSpecialist,
                    RoleTypeId.Tutorial
                };

                List<RoleTypeId> SelectedRole()
                {
                    if (UnityEngine.Random.Range(1, 11) == 1)
                        return Scps;

                    else
                        return Humans;
                }

                ev.Player.Role.Set(Tools.GetRandomValue(SelectedRole()));
                ev.Player.ClearInventory();
                ev.Player.Position = GameObject.Find("LobbyStartPoint").transform.position;
                ev.Player.CameraTransform.Rotate(Vector3.left, 180);
                ev.Player.AddBroadcast(10, Notions.WelcomeMessage);

                string iv(int num)
                {
                    if (CurrentMode != null)
                        return CurrentMode;

                    else
                        return ModeVote.Keys.ToList()[num - 1];
                }

                while (!Round.IsStarted)
                {
                    if (Physics.Raycast(ev.Player.Position, Vector3.down, out RaycastHit hit, 5f, (LayerMask)1))
                    {
                        if (hit.transform.name == "Credit")
                        {
                            ev.Player.ShowHint(
"""
<size=50><b>[ ⭐ 랜덤게임모드(RGM) 크레딧 ⭐ ]</b></size>

<align=left><size=30>
<b><size=35><color=#F7FE2E>관리진</color></size></b>
Arloramäki(@alvar_noah) - 서버 소유자
Serendipity(@mercedes83) - 총괄 관리자
Normalperson(@normal._.person) - 관리자
bluefox2322(@bluefox2322) - 수습 관리자
pey paid for it(@0735_) - 수습 관리자
leejihyuk(@leejihyuk) - 수습 관리자

<b><size=35><color=#C8FE2E>개발진</color></size></b>
GoldenPig1205(@GoldenPig1205) - 메인 개발자

<b><size=35><color=#F79F81>후원자</color></size></b>
<size=20>[D.I.]Dotory001(@dotory001), 류세(@milkyway_0119), Lu(@1__neeko__1), 뇨호호(@yeeeee222), ㅠㅅㅠ(@tampast)</size>

<b><size=35><color=#F8E0F7>도움 주신 분들</color></size><b>
<size=20>Cocoa(@cocoa_1.19)</size>
</size></align>
\n\n\n\n\n\n\n\n
""", 1.2f);
                        }
                        else if (hit.transform.name == "Mode")
                        {
                            List<string> Modes = new List<string>();
                            
                            foreach (var mode in ModeList)
                            {
                                string modeName = mode.Key;
                                string color = mode.Value[0];
                                bool IsPrivate = mode.Value[3] == "private";

                                if (IsPrivate)
                                    Modes.Add($"<s><color=#{color}>{modeName}</color></s>");

                                else
                                    Modes.Add($"<color=#{color}>{modeName}</color>");
                            }

                            ev.Player.ShowHint($"\n\n\n\n\n\n<size=40><b>[ ⭐ 랜덤게임모드(RGM) 모드 목록 ⭐ ]</b></size>\n\n<size=25>{string.Join(", ", Modes)}</size>");
                        }
                        else
                        {
                            string SelectedMode;
                            string ModeColor;
                            string ModeDescription;

                            for (int i = 0; i < 3; i++)
                            {
                                if (ModeVote.ContainsKey(ModeVote.Keys.ToList()[i]) && ModeVote[ModeVote.Keys.ToList()[i]].Contains(ev.Player))
                                    ModeVote[ModeVote.Keys.ToList()[i]].Remove(ev.Player);
                            }

                            if (new List<string>() { "First", "Second", "Third" }.Contains(hit.collider.name))
                            {
                                if (hit.collider.name == "First")
                                {
                                    SelectedMode = ModeVote.Keys.ToList()[0];
                                    ModeVote[SelectedMode].Add(ev.Player);
                                }
                                else if (hit.collider.name == "Second")
                                {
                                    SelectedMode = ModeVote.Keys.ToList()[1];
                                    ModeVote[SelectedMode].Add(ev.Player);
                                }
                                else
                                {
                                    SelectedMode = ModeVote.Keys.ToList()[2];
                                    ModeVote[SelectedMode].Add(ev.Player);
                                }

                                ModeColor = ModeList[SelectedMode][0];
                                ModeDescription = ModeList[SelectedMode][1];
                            }
                            else
                            {
                                string FirstDesc()
                                {
                                    if (SelectMode == "RandomSelect")
                                        return "<b>[선택 모드 : 임의의 손길]</b> <color=#F8E0E0>랜덤한 모드가 선택됩니다. 과연 어떤 모드가 걸릴까요?</color>";

                                    else if (SelectMode == "SimpleSelect")
                                        return "<b>[선택 모드 : 간이 셀렉트]</b> <color=#F5F6CE>투표한 유저 중에서 모드가 자동으로 결정됩니다.</color>";

                                    else if (SelectMode == "MostVote")
                                        return "<b>[선택 모드 : 다수 결정자]</b> <color=#E0F2F7>원하는 모드의 번호가 할당된 플랫폼을 밟아 투표하세요.</color>";

                                    else
                                        return "<b>[버그로 추정됨 : 문의 요망]</b> 어떤 선택 모드도 선택되지 않았습니다. 뭔가 이상합니다.";
                                }

                                SelectedMode = "<i>참고</i>";
                                ModeColor = "ffffff";
                                ModeDescription = $"{FirstDesc()}\n<size=25>콘솔(` 또는 ~)을 열고 .help를 입력하여 사용 가능한 [RGM] 명령어 리스트를 확인할 수 있습니다.</size>";
                            }

                            string IdeaBy()
                            {
                                if (!ModeList.ContainsKey(SelectedMode) || ModeList[SelectedMode][4] == "")
                                    return "";
                                else
                                    return $" <size=20><color=white>Idea by {ModeList[SelectedMode][4]}</color></size>";
                            }

                            List<string> uc = UsersManager.UsersCache[ev.Player.UserId];

                            ev.Player.ShowHint(Notions.LobbyMessage
                                .Replace("{FirstMark}", ModeVote[iv(1)].Contains(ev.Player) ? "■" : "□")
                                .Replace("{SecondMark}", ModeVote[iv(2)].Contains(ev.Player) ? "■" : "□")
                                .Replace("{ThirdMark}", ModeVote[iv(3)].Contains(ev.Player) ? "■" : "□")
                                .Replace("{First}", iv(1)).Replace("{FirstVote}", ModeVote[iv(1)].Contains(ev.Player) ? $"<color=yellow>{ModeVote[iv(1)].Count()}</color>" : ModeVote[iv(1)].Count().ToString())
                                .Replace("{Second}", iv(2)).Replace("{SecondVote}", ModeVote[iv(2)].Contains(ev.Player) ? $"<color=yellow>{ModeVote[iv(2)].Count()}</color>" : ModeVote[iv(2)].Count().ToString())
                                .Replace("{Third}", iv(3)).Replace("{ThirdVote}", ModeVote[iv(3)].Contains(ev.Player) ? $"<color=yellow>{ModeVote[iv(3)].Count()}</color>" : ModeVote[iv(3)].Count().ToString())
                                .Replace("{ModeName}", $"{SelectedMode}{IdeaBy()}").Replace("{ModeColor}", $"{ModeColor}").Replace("{ModeDescription}", $"{ModeDescription}")
                                .Replace("{Lines}", $"{(ModeDescription.Contains("\n") ? "\n" : "\n\n")}").Replace("{Tip}", Tip)
                                .Replace("{Exp}", $"{uc[0]}")
                                .Replace("{RP}", $"{uc[1]}")
                                .Replace("{Cash}", $"{int.Parse(uc[2]).ToString("N0")}")
                                .Replace("{KillEffect}", $"{(uc[4] == "0" ? "-" : uc[4])}")
                                , 1.2f);
                        }
                    }

                    await Task.Delay(500);
                }
            }
        }

        public void OnLeft(Exiled.Events.EventArgs.Player.LeftEventArgs ev)
        {
            if (OnGround.ContainsKey(ev.Player))
                OnGround.Remove(ev.Player);

            if (!Round.IsStarted)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (ModeVote.ContainsKey(ModeVote.Keys.ToList()[i]) && ModeVote[ModeVote.Keys.ToList()[i]].Contains(ev.Player))
                        ModeVote[ModeVote.Keys.ToList()[i]].Remove(ev.Player);
                }
            }
        }

        public void OnSpawningRagdoll(Exiled.Events.EventArgs.Player.SpawningRagdollEventArgs ev)
        {
            if (!Round.IsStarted)
                ev.IsAllowed = false;
        }

        public async void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            if (ev.Player.IsAlive)
            {
                ev.Player.Scale = new Vector3(1, 1, 1);
                ev.Player.EnableEffect(EffectType.FogControl, 1);
            }

            if (ev.Reason == SpawnReason.RoundStart)
            {
                if (ev.Player.IsScp)
                {
                    if (UnityEngine.Random.Range(1, 21) == 1 && !IsScp3114Enabled)
                    {
                        ev.Player.Role.Set(RoleTypeId.Scp3114);

                        ev.Player.AddBroadcast(10, $"<color={ev.Player.Role.Color.ToHex()}>SCP-3114(5%, 정규)</color> 기믹이 적용되었습니다.");
                        IsScp3114Enabled = true;
                    }
                }
                else if (ev.Player.IsHuman)
                {
                    if (StartupRandom == 1) // 시작 카오스
                    {
                        if (ev.Player.Role.Type == RoleTypeId.FacilityGuard)
                        {
                            ev.Player.Role.Set(RoleTypeId.ChaosConscript);

                            ev.Player.AddBroadcast(10, $"<color={ev.Player.Role.Color.ToHex()}>시작 카오스(5%, 정규)</color> 기믹이 적용되었습니다.");
                        }
                    }
                    if (StartupRandom == 2) // 시작 NTF
                    {
                        if (ev.Player.Role.Type == RoleTypeId.FacilityGuard)
                        {
                            ev.Player.Role.Set(RoleTypeId.NtfPrivate);

                            ev.Player.AddBroadcast(10, $"<color={ev.Player.Role.Color.ToHex()}>시작 NTF(5%, 정규)</color> 기믹이 적용되었습니다.");
                        }
                    }

                    int rand = UnityEngine.Random.Range(1, 101); // 시작 좀?비
                    if (rand == 1)
                    {
                        ev.Player.Role.Set(RoleTypeId.Scp0492);
                        ev.Player.MaxHealth = 1000;
                        ev.Player.Health = ev.Player.MaxHealth;

                        ev.Player.AddBroadcast(10, $"<color={ev.Player.Role.Color.ToHex()}>시작 좀비(1%, 이스터에그)</color> 기믹이 적용되었습니다.");
                    }
                    else if (rand == 2)
                    {
                        ev.Player.Scale = new Vector3(-1, -1, -1);

                        ev.Player.AddBroadcast(10, $"<color={ev.Player.Role.Color.ToHex()}>뒤집기(1%, 이스터에그)</color> 기믹이 적용되었습니다.");
                    }
                }
            }
            
            if (ev.Player.Role.Type == RoleTypeId.Scp079)
            {
                ev.Player.MaxHealth = 12050;
                ev.Player.Health = ev.Player.MaxHealth;
            }

            if (ev.Player.IsAlive && Round.IsStarted && (ev.Reason == SpawnReason.RoundStart || ev.Reason == SpawnReason.Respawn))
            {
                GodModePlayers.Add(ev.Player);

                await Task.Delay(5000);

                if (GodModePlayers.Contains(ev.Player))
                    GodModePlayers.Remove(ev.Player);
            }
        }

        public void OnInteractingDoor(Exiled.Events.EventArgs.Player.InteractingDoorEventArgs ev)
        {
            if (ev.Player.IsScp && ev.Player.CurrentItem != null && ev.Door.Name.Contains("CHECKPOINT"))
                ev.Door.IsOpen = true;
        }

        public void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            if (!Round.IsStarted)
                ev.IsAllowed = false;

            else
            {
                if (GodModePlayers.Contains(ev.Player))
                {
                    if (ev.Attacker != null && ev.Attacker != ev.Player)
                        ev.IsAllowed = false;
                }
            }
        }

        public void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            if (!Round.IsStarted)
                ev.IsAllowed = false;

            else
            {
                if (GodModePlayers.Contains(ev.Player))
                {
                    if (ev.Attacker != null && ev.Attacker != ev.Player)
                        ev.IsAllowed = false;
                }
            }
        }

        public void OnStopping(Exiled.Events.EventArgs.Warhead.StoppingEventArgs ev)
        {
            if (AutoNuke)
                ev.IsAllowed = false;
        }

        public void OnDetonating(Exiled.Events.EventArgs.Warhead.DetonatingEventArgs ev)
        {
            Player.List.Where(x => x.Zone != ZoneType.Surface && x.IsAlive).ToList().ForEach(x => 
            {
                if (GodModePlayers.Contains(x))
                    GodModePlayers.Remove(x);

                x.Kill("핵폭발에 사망하였습니다."); 
            });
        }

        public void OnInteractingScp330(Exiled.Events.EventArgs.Scp330.InteractingScp330EventArgs ev)
        {
            if (UnityEngine.Random.Range(1, 21) == 1)
            {
                ev.IsAllowed = false;
                ev.Player.TryAddCandy(InventorySystem.Items.Usables.Scp330.CandyKindID.Pink);

                ev.Player.AddBroadcast(10, $"<color=#FF00FF>핑크 캔디(5%, 정규)</color> 기믹이 적용되었습니다.");
            }
        }

        public async void OnUsingScp244(Exiled.Events.EventArgs.Scp244.UsingScp244EventArgs ev)
        {
            await Task.Delay(60 * 1000);

            ev.Scp244.Health = 0;
            ev.Scp244.Destroy();
        }

        public async void OnOpeningScp244(Exiled.Events.EventArgs.Scp244.OpeningScp244EventArgs ev)
        {
            await Task.Delay(60 * 1000);

            ev.Pickup.Health = 0;
            ev.Pickup.Destroy();
        }

        public void OnRecontained(Exiled.Events.EventArgs.Scp079.RecontainedEventArgs ev)
        {
            ev.Player.Kill("재격리 버튼에 의해 격리되었습니다.");
        }

        public IEnumerator<float> GameStartButton()
        {
            int RemainingPress = 20;
            bool ButtonPressed = false;
            Transform redObject = null;

            while (!ButtonPressed)
            {
                if (!FreezeGameStart)
                {
                    bool pressing = false;

                    foreach (var player in Player.List)
                    {
                        if (Physics.Raycast(player.Position, Vector3.down, out RaycastHit hit, 1f, (LayerMask)1))
                        {
                            if (hit.transform.name == "GameStartRed")
                            {
                                if (Player.List.Count() > 1)
                                {
                                    if (RemainingPress <= 0)
                                        ButtonPressed = true;
                                }

                                redObject = hit.transform;
                                pressing = true;

                                RemainingPress -= 1;

                                redObject.position = new Vector3(redObject.position.x, redObject.position.y - 0.015f, redObject.transform.position.z);
                            }
                        }
                    }

                    if (!pressing)
                    {
                        if (RemainingPress < 20)
                        {
                            RemainingPress += 1;

                            redObject.position = new Vector3(redObject.transform.position.x, redObject.transform.position.y + 0.015f, redObject.transform.position.z);
                        }
                    }
                }

                yield return Timing.WaitForSeconds(0.1f);
            }

            Player.List.ToList().ForEach(x => x.Role.Set(RoleTypeId.Spectator));
            Round.Start();

            yield break;
        }

        public IEnumerator<float> RandomSelectMode()
        {
            while (!Round.IsStarted)
            {
                PickModes();

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public IEnumerator<float> ModeResetButton()
        {
            float RemainingPress = 20;
            bool ButtonPressed = false;
            Transform redObject = null;

            while (!ButtonPressed)
            {
                bool pressing = false;
                RaycastHit hit;
                int stack = 0;

                foreach (var player in Player.List)
                {
                    if (Physics.Raycast(player.Position, Vector3.down, out hit, 1f, (LayerMask)1))
                    {
                        if (hit.transform.name == "ModeResetRed")
                        {
                            stack += 1;
                            redObject = hit.transform;
                            pressing = true;
                        }
                    }
                }

                if (Player.List.Count() > 1)
                {
                    if (RemainingPress <= 0)
                        ButtonPressed = true;
                }

                if (pressing)
                {
                    RemainingPress -= 0.01f * stack;

                    redObject.position = new Vector3(redObject.position.x, redObject.position.y - 0.0001f * stack, redObject.transform.position.z);
                }

                yield return Timing.WaitForSeconds(0.1f);
            }

            PickModes();
            Server.ExecuteCommand($"/cassie_sl <mark=#ffff00aa><color=#000000><color=#ffffff>모드 투표 리스트</color>가 초기화되었습니다.</color></mark>");

            FreezeGameStart = true;

            yield return Timing.WaitForSeconds(10f);

            FreezeGameStart = false;

            yield break;
        }

        public IEnumerator<float> IsFallDown()
        {
            while (true)
            {
                foreach (var player in Player.List.Where(x => x.IsAlive))
                {
                    if (OnGround.ContainsKey(player) && !player.IsNoclipPermitted && player.Role.Type != RoleTypeId.Scp079)
                    {
                        if (FpcExtensionMethods.IsGrounded(player.ReferenceHub))
                            OnGround[player] = 5;
                        else
                        {
                            OnGround[player] -= 0.1f;

                            if (OnGround[player] <= 0)
                                player.Kill("공허에 빨려들어갔습니다. (5초 이상 낙하)");
                        }
                    }
                }

                yield return Timing.WaitForSeconds(0.1f);
            }
        }

        public IEnumerator<float> ChattingCooldown()
        {
            while (true)
            {
                ChatCooldown.Clear();

                yield return Timing.WaitForSeconds(2f);
            }
        }

        public IEnumerator<float> Ball()
        {
            while (!Round.IsStarted)
            {
                foreach (Player player in Player.List.Where(x => x.IsAlive))
                {
                    foreach (Transform Ball in Balls)
                    {
                        GameObject _ball = Ball.gameObject;

                        if (Vector3.Distance(_ball.transform.position, player.Position) < 2)
                        {
                            _ball.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rig);
                            rig.AddForce(player.GameObject.transform.forward + new Vector3(0, 0.01f, 0), ForceMode.Impulse);
                        }
                        
                        /*
                        else if (Vector3.Distance(_ball.transform.position, player.Position) > 45)
                            _ball.transform.position = new Vector3(player.Position.x, player.Position.y + 2, player.Position.z);
                        */
                    }
                }

                yield return Timing.WaitForOneFrame;
            }
        }
    }
}
