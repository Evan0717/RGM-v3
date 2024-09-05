using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace RGM
{
    public class RGM : Plugin<Config>
    {
        public static RGM Instance;

        public static string WebhookURL;

        public string CurrentMode = null;
        public Dictionary<string, List<string>> ModeList;
        public Dictionary<Player, float> OnGround = new Dictionary<Player, float>();
        public Dictionary<Player, Room> CurrentRoom = new Dictionary<Player, Room>();

        public override void OnEnabled()
        {
            Instance = this;

            WebhookURL = Config.WebhookURL;
            ModeList = Config.Modes;

            // + EventHandlers / Round
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            // + EventHandlers / Player
            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Player.Spawning += OnSpawned;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
        }

        public override void OnDisabled()
        {
            // - EventHandlers / Round
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            // - EventHandlers / Player
            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Player.Spawning -= OnSpawned;
            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;

            Instance = null;
        }

        public void OnWaitingForPlayers()
        {
            Server.ExecuteCommand("rnr");

            Logger.Discord discord = new Logger.Discord();
            discord.OnEnabled();
        }

        // EventArgs / Round
        public void OnRoundStarted()
        {
            if (CurrentMode == null)
                CurrentMode = ModeList.Keys.Where(x => ModeList[x][3] != "private").ToList()[UnityEngine.Random.Range(0, ModeList.Count)];

            string ModeColor = ModeList[CurrentMode][0];
            string ModeDescription = ModeList[CurrentMode][1];
            string ModeFileName = ModeList[CurrentMode][2];

            Log.Info($"이번 라운드의 모드 : [{CurrentMode}]");

            foreach (var player in Player.List)
            {
                player.ClearBroadcasts();
                player.Broadcast(10, Config.StartModeDescription
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
        }

        public void OnRoundEnded(Exiled.Events.EventArgs.Server.RoundEndedEventArgs ev)
        {

        }

        // EventArgs / Player
        public async void OnVerified(Exiled.Events.EventArgs.Player.VerifiedEventArgs ev)
        {
            if (Round.IsStarted)
            {
                ev.Player.Broadcast(10, Config.LateJoinModeDescription
                    .Replace("{ModeColor}", ModeList[CurrentMode][0])
                    .Replace("{CurrentMode}", CurrentMode)
                    );
            }
            else
            {
                ev.Player.Broadcast(10, Config.WelcomeMessage);

                while (!Round.IsStarted)
                {
                    ev.Player.ShowHint(Config.LobbyMessage, 1.2f);
                    await Task.Delay(1000);
                }
            }
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawningEventArgs ev)
        {
            ev.Player.EnableEffect(Exiled.API.Enums.EffectType.FogControl);
        }

        public void OnLeft(Exiled.Events.EventArgs.Player.LeftEventArgs ev)
        {
            if (OnGround.ContainsKey(ev.Player))
                OnGround.Remove(ev.Player);
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            ev.Player.EnableEffect(Exiled.API.Enums.EffectType.FogControl);

            if (ev.Player.IsScp && ev.Reason == Exiled.API.Enums.SpawnReason.RoundStart)
            {
                if (UnityEngine.Random.Range(1, 14) == 1)
                    ev.Player.Role.Set(PlayerRoles.RoleTypeId.Scp3114);
            }
        }

        public void OnInteractingDoor(Exiled.Events.EventArgs.Player.InteractingDoorEventArgs ev)
        {
            if (ev.Player.IsScp && ev.Player.CurrentItem != null && ev.Door.IsPartOfCheckpoint)
                ev.Door.IsOpen = true;
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
                    if (player.IsAlive && OnGround.ContainsKey(player) && !player.IsNoclipPermitted && player.Role.Type != PlayerRoles.RoleTypeId.Scp079)
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

        public IEnumerator<float> BlockAFK()
        {
            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (CurrentRoom.ContainsKey(player))
                    {
                        if (CurrentRoom[player] == player.CurrentRoom && player.CurrentRoom.Name != "Outside" && player.IsAlive)
                        {
                            player.ShowHint($"<color=red><i><b>당신은 2분 동안 한 방에 있었습니다!!!</b></i></color>", 15);
                            player.EnableEffect(Exiled.API.Enums.EffectType.SeveredHands);

                            CurrentRoom[player] = player.CurrentRoom;
                        }
                        else
                        {
                            if (player.IsAlive)
                                CurrentRoom[player] = player.CurrentRoom;
                        }
                    }
                    else
                    {
                        if (player.IsAlive)
                            CurrentRoom.Add(player, player.CurrentRoom);
                    }
                }

                yield return Timing.WaitForSeconds(120);
            }
        }
    }
}
