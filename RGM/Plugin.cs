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

namespace RGM
{
    public class RGM : Plugin<Config>
    {
        public static RGM Instance;

        public static string WebhookURL;
        public static string BotAPIServer;

        public string CurrentMode = null;
        public Dictionary<string, List<string>> ModeList;
        public Dictionary<string, List<Player>> ModeVote = new Dictionary<string, List<Player>>();
        public Dictionary<Player, float> OnGround = new Dictionary<Player, float>();
        public Dictionary<Player, Room> CurrentRoom = new Dictionary<Player, Room>();

        public int StartupRandom = UnityEngine.Random.Range(1, 21);
        public bool AutoNuke = false;

        public static T GetRandomValue<T>(List<T> list)
        {
            System.Random random = new System.Random();
            int index = random.Next(0, list.Count);
            return list[index];
        }

        public override void OnEnabled()
        {
            Instance = this;
            base.OnEnabled();

            WebhookURL = Config.WebhookURL;
            BotAPIServer = Config.BotAPIServer;
            ModeList = Config.Modes;

            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Player.Left += OnLeft;
            Exiled.Events.Handlers.Player.SpawningRagdoll += OnSpawningRagdoll;
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            
            Exiled.Events.Handlers.Warhead.Stopping += OnStopping;

            Exiled.Events.Handlers.Scp330.InteractingScp330 += OnInteractingScp330;

            Timing.RunCoroutine(IsFallDown());

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

            Exiled.Events.Handlers.Warhead.Stopping -= OnStopping;

            Exiled.Events.Handlers.Scp330.InteractingScp330 -= OnInteractingScp330;

            base.OnDisabled();
            Instance = null;
        }

        public async void OnWaitingForPlayers()
        {
            Server.ExecuteCommand($"/mp load RGMLobby");

            var webhook = new Discord.Webhook();
            webhook.OnEnabled();

            var command = new Discord.Command();
            command.OnEnabled();

            for (int i=1; i<4; i++)
            {
                var StaticModeList = ModeList.Keys.Where(x => ModeList[x][3] != "private" && !ModeVote.ContainsKey(x)).ToList();
                var mode = StaticModeList[UnityEngine.Random.Range(0, StaticModeList.Count())];
                ModeVote.Add(mode, new List<Player>());
            }

            var First = GameObject.FindObjectsOfType<Transform>().Where(t => t.name == "First").ToList();
            var Second = GameObject.FindObjectsOfType<Transform>().Where(t => t.name == "Second").ToList();
            var Third = GameObject.FindObjectsOfType<Transform>().Where(t => t.name == "Third").ToList();

            List<List<Transform>> Pads = new List<List<Transform>>() { First, Second, Third };

            for (int i=0; i<3; i++)
            {
                foreach (var Pad in Pads[i])
                    Pad.GetComponent<PrimitiveObject>().Primitive.Color = ColorUtility.TryParseHtmlString("#" + ModeList[ModeVote.Keys.ToList()[i]][0], out Color color) ? color : Color.white;
            }

            var Numbers = GameObject.FindObjectsOfType<Transform>().Where(t => t.name == "Number").ToList();

            Color randomColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);

            foreach (var Number in Numbers)
                Number.GetComponent<PrimitiveObject>().Primitive.Color = randomColor;

            while (!Round.IsStarted)
            {
                if (Player.List.Count() > 1)
                    break;

                await Task.Delay(1000);
            }

            await Task.Delay(5000);

            while (!Round.IsStarted)
            {
                if (Player.List.Count() > 1 && Round.LobbyWaitingTime < 1)
                {
                    Player.List.ToList().ForEach(x => x.Role.Set(RoleTypeId.Spectator));
                    Round.Start();
                }

                await Task.Delay(1000);
            }
        }

        // EventArgs / Round
        public async void OnRoundStarted()
        {
            foreach (var player in Player.List)
            {
                Server.ExecuteCommand($"/speak {player.Id} disable");
            }

            if (CurrentMode == null)
            {
                var maxLength = ModeVote.Values.Max(list => list.Count);
                var longestKeys = ModeVote.Keys.Where(key => ModeVote[key].Count == maxLength).ToList();
                var randomKey = longestKeys[UnityEngine.Random.Range(0, longestKeys.Count)];
                CurrentMode = randomKey;
            }

            string ModeColor = ModeList[CurrentMode][0];
            string ModeDescription = ModeList[CurrentMode][1];
            string ModeFileName = ModeList[CurrentMode][2];

            Log.Info($"이번 라운드의 모드 : [{CurrentMode}]");

            foreach (var player in Player.List)
            {
                player.ClearPlayerBroadcasts();
                player.AddBroadcast(10, Config.StartModeDescription
                .Replace("{ModeColor}", ModeColor)
                .Replace("{CurrentMode}", CurrentMode)
                .Replace("{ModeDescription}", ModeDescription)
                );

                player.SendConsoleMessage($"\n[ {CurrentMode} ]\n" +
                    $"------------------------------------------------------------------------" +
                    $"\n{ModeDescription}\n" +
                    $"------------------------------------------------------------------------", "white");
            }

            var modeType = Type.GetType($"RGM.Modes.{ModeFileName}");
            if (modeType != null)
            {
                var modeInstance = Activator.CreateInstance(modeType);
                var onEnabledMethod = modeType.GetMethod("OnEnabled");
                onEnabledMethod?.Invoke(modeInstance, null);
            }

            await Task.Delay(25 * 60 * 1000);

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
                Player.List.ToList().ForEach(x => x.Health -= (x.MaxHealth / 100));
                await Task.Delay(1000);
            }
        }

        public async void OnRoundEnded(Exiled.Events.EventArgs.Server.RoundEndedEventArgs ev)
        {
            await Task.Delay(9000);

            Server.ExecuteCommand("sr");
        }

        // EventArgs / Player
        public async void OnVerified(Exiled.Events.EventArgs.Player.VerifiedEventArgs ev)
        {
            OnGround.Add(ev.Player, 5);

            if (Round.IsStarted)
            {
                ev.Player.AddBroadcast(10, Config.LateJoinModeDescription
                    .Replace("{ModeColor}", ModeList[CurrentMode][0])
                    .Replace("{CurrentMode}", CurrentMode)
                    );

                string ModeDescription = ModeList[CurrentMode][1];
                ev.Player.SendConsoleMessage($"\n[ {CurrentMode} ]\n" +
                    $"------------------------------------------------------------------------" +
                    $"\n{ModeDescription}\n" +
                    $"------------------------------------------------------------------------", "white");
            }
            else
            {
                foreach (var player in Player.List)
                {
                    Server.ExecuteCommand($"/speak {player.Id} enable");
                }

                List<RoleTypeId> Humans = new List<RoleTypeId>()
                {
                    RoleTypeId.ClassD,
                    RoleTypeId.Scientist,
                    RoleTypeId.FacilityGuard,
                    RoleTypeId.ChaosConscript,
                    RoleTypeId.NtfSpecialist,
                    RoleTypeId.Tutorial
                };

                ev.Player.Role.Set(Humans[UnityEngine.Random.Range(0, Humans.Count())]);
                ev.Player.ClearInventory();
                ev.Player.Position = new Vector3(47.103f, 1007.963f, -6.374592f);
                MultiBroadcast.API.MultiBroadcast.AddPlayerBroadcast(ev.Player, 10, Config.WelcomeMessage);

                string iv(int num)
                {
                    return ModeVote.Keys.ToList()[num - 1];
                }

                while (!Round.IsStarted)
                {
                    if (Physics.Raycast(ev.Player.Position, Vector3.down, out RaycastHit hit, 2f, (LayerMask)1))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (ModeVote.ContainsKey(ModeVote.Keys.ToList()[i]) && ModeVote[ModeVote.Keys.ToList()[i]].Contains(ev.Player))
                                ModeVote[ModeVote.Keys.ToList()[i]].Remove(ev.Player);
                        }

                        if (hit.collider.name == "First")
                            ModeVote[ModeVote.Keys.ToList()[0]].Add(ev.Player);

                        else if (hit.collider.name == "Second")
                            ModeVote[ModeVote.Keys.ToList()[1]].Add(ev.Player);

                        else if (hit.collider.name == "Third")
                            ModeVote[ModeVote.Keys.ToList()[2]].Add(ev.Player);
                    }

                    ev.Player.ShowHint(Config.LobbyMessage
                        .Replace("{First}", iv(1)).Replace("{FirstVote}", ModeVote[iv(1)].Contains(ev.Player) ? $"<color=yellow>{ModeVote[iv(1)].Count()}</color>" : ModeVote[iv(1)].Count().ToString())
                        .Replace("{Second}", iv(2)).Replace("{SecondVote}", ModeVote[iv(2)].Contains(ev.Player) ? $"<color=yellow>{ModeVote[iv(2)].Count()}</color>" : ModeVote[iv(2)].Count().ToString())
                        .Replace("{Third}", iv(3)).Replace("{ThirdVote}", ModeVote[iv(3)].Contains(ev.Player) ? $"<color=yellow>{ModeVote[iv(3)].Count()}</color>" : ModeVote[iv(3)].Count().ToString())
                        .Replace("{ModeDescription}", "TIP. 콘솔(`)창을 열고 [.help] 명령어를 입력해보세요."), 1.2f);


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
            {
                ev.IsAllowed = false;
            }
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            ev.Player.EnableEffect(Exiled.API.Enums.EffectType.FogControl);

            if (ev.Reason == Exiled.API.Enums.SpawnReason.RoundStart)
            {
                if (ev.Player.IsScp)
                {
                    if (UnityEngine.Random.Range(1, 14) == 1)
                        ev.Player.Role.Set(RoleTypeId.Scp3114);
                }
                else if (ev.Player.IsHuman)
                {
                    if (StartupRandom == 1) // 시작 카오스
                    {
                        if (ev.Player.Role.Type == RoleTypeId.FacilityGuard)
                            ev.Player.Role.Set(RoleTypeId.ChaosConscript);
                    }
                    if (StartupRandom == 2) // 시작 NTF
                    {
                        if (ev.Player.Role.Type == RoleTypeId.FacilityGuard)
                            ev.Player.Role.Set(RoleTypeId.NtfSpecialist);
                    }

                    int rand = UnityEngine.Random.Range(1, 101); // 시작 좀?비
                    if (rand == 1)
                    {
                        ev.Player.Role.Set(RoleTypeId.Scp0492);
                        ev.Player.MaxHealth = 1000;
                        ev.Player.Health = ev.Player.MaxHealth;
                    }
                }
            }
        }

        public void OnInteractingDoor(Exiled.Events.EventArgs.Player.InteractingDoorEventArgs ev)
        {
            if (ev.Player.IsScp && ev.Player.CurrentItem != null && ev.Door.Name.Contains("CHECKPOINT"))
                ev.Door.IsOpen = true;
        }

        public void OnStopping(Exiled.Events.EventArgs.Warhead.StoppingEventArgs ev)
        {
            if (AutoNuke)
                ev.IsAllowed = false;
        }

        public void OnInteractingScp330(Exiled.Events.EventArgs.Scp330.InteractingScp330EventArgs ev)
        {
            if (UnityEngine.Random.Range(1, 14) == 1)
            {
                ev.IsAllowed = false;
                ev.Player.TryAddCandy(InventorySystem.Items.Usables.Scp330.CandyKindID.Pink);
            }
        }

        public IEnumerator<float> IsFallDown()
        {
            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (player.IsAlive && OnGround.ContainsKey(player) && !player.IsNoclipPermitted && player.Role.Type != RoleTypeId.Scp079)
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
    }
}
