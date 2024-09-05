using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace Plugin
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance;

        public string CurrentMode = null;
        public Dictionary<string, List<string>> ModeList = new Dictionary<string, List<string>>() // 모드 이름 : 색상, 설명, 파일 이름
        {
            { "한 방", new List<string>() { "FAAC58", "피격당하는 즉시 죽습니다.", "OnePunch", "public", "" } },
            { "츄파츕스", new List<string>() { "A9E2F3", "모두가 제일버드를 가지고 시작합니다.", "Jailbird", "public", "" } },
            { "SCP 러쉬", new List<string>() { "FE2E2E", "모든 SCP가 한 개체로 통일됩니다.", "SCPRUSH", "public", "" } }
        };

        public override void OnEnabled()
        {
            Instance = this;

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
        }

        // EventArgs / Round
        public void OnRoundStarted()
        {
            if (CurrentMode == null)
                CurrentMode = ModeList.Keys.Where(x => ModeList[x][3] != "private").ToList()[UnityEngine.Random.Range(0, ModeList.Count)];

            string ModeColor = ModeList[CurrentMode][0];
            string ModeDescription = ModeList[CurrentMode][1];
            string ModeFileName = ModeList[CurrentMode][2];

            foreach (var player in Player.List)
            {
                player.ClearBroadcasts();
                player.Broadcast(10, $"<size=30>[<b><color=#{ModeColor}>{CurrentMode}</color></b>]</size>\n<size=25>{ModeDescription}</size>");
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
                ev.Player.Broadcast(10, $"<size=20>현재 진행중인 모드</size>\n<size=25><b>[<color=#{ModeList[CurrentMode][0]}>{CurrentMode}</color>]</b></size>");
            }
            else
            {
                ev.Player.Broadcast(10, $"<size=25><b>랜덤게임모드</b>에 오신 것을 환영합니다!</size>");

                while (!Round.IsStarted)
                {
                    ev.Player.ShowHint("\n\n\n\n\n\n\n<size=200><b>?</b></size>\n<size=20>\"이번 라운드는 어떤 모드가 걸릴까요?\"</size>\n", 1.2f);
                    await Task.Delay(1000);
                }
            }
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawningEventArgs ev)
        {
            ev.Player.EnableEffect(Exiled.API.Enums.EffectType.FogControl);
        }

        public void OnInteractingDoor(Exiled.Events.EventArgs.Player.InteractingDoorEventArgs ev)
        {
            if (ev.Player.IsScp && ev.Player.CurrentItem != null && ev.Door.IsPartOfCheckpoint)
                ev.Door.IsOpen = true;
        }
    }
}
