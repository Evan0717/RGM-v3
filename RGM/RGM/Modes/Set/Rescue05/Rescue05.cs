using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;

using PlayerRoles;
using UnityEngine;
using Exiled.API.Enums;
using RGM.API.Features;

using static RGM.Variables.Variable;
using Exiled.Events.EventArgs.Server;
using Exiled.API.Features.Waves;
using Respawning;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.Rescue05)]
    class Rescue05 : Mode
    {
        public override string Name => "05 평의회 구출 작전";
        public override string Description => "05 평의회를 구출하려는 자들과 사살하려는 자들의 싸움입니다.";
        public override string Detail =>
"""
<color=#000000><b>05 평의회</b></color>가 탈출에 성공할 경우,
<color=#2E9AFE>MTF</color> 진영이 승리합니다.

<color=#000000><b>05 평의회</b></color>가 사살될 경우,
<color=#088A08>혼돈의 반란</color> 진영이 승리합니다.

<i>* 게임 시작 10분 뒤 <color=red>자동핵</color>이 작동됩니다.</i>
""";
        public override string Color => "0040FF";

        public static Rescue05 Instance;

        Player Level05;
        Player Assassin;

        CoroutineHandle _onModeStarted;
        CoroutineHandle _autoWarhead;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            Exiled.Events.Handlers.Player.Escaping += OnEscaping;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.Handcuffing += OnHandcuffing;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
            _autoWarhead = Timing.RunCoroutine(AutoWarhead());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            Exiled.Events.Handlers.Player.Escaping -= OnEscaping;
            Exiled.Events.Handlers.Player.Dying -= OnDying;
            Exiled.Events.Handlers.Player.Handcuffing -= OnHandcuffing;

            Timing.KillCoroutines(_onModeStarted);
            Timing.KillCoroutines(_autoWarhead);
        }

        public IEnumerator<float> OnModeStarted()
        {
            Level05 = Tools.GetRandomValue(PlayerManager.List.ToList());
            Assassin = Tools.GetRandomValue(PlayerManager.List.Where(x => x != Level05).ToList());

            List<ItemType> Level05Items = new List<ItemType>() 
            {
                ItemType.KeycardO5,
                ItemType.GrenadeFlash,
                ItemType.Flashlight,
                ItemType.GunCOM15,
                ItemType.Medkit,
                ItemType.Painkillers,
                ItemType.SCP500,
                ItemType.Radio
            };

            Level05.Role.Set(RoleTypeId.Scientist);
            Level05.ClearInventory();
            foreach (ItemType item in Level05Items) Level05.AddItem(item);
            Level05.AddBroadcast(10, $"<size=25>당신은 <color=#000000><b>05 평의회</b></color>, <color=#A4A4A4>시설 경비</color>들의 도움을 받아 시설에서 탈출하십시오.</size>");

            List<ItemType> AssanssinItems = new List<ItemType>()
            {
                ItemType.GunCOM15,
                ItemType.Ammo9x19
            };

            Assassin.Role.Set(RoleTypeId.ClassD);
            foreach (ItemType item in AssanssinItems) Assassin.AddItem(item);
            Assassin.AddBroadcast(10, $"<size=25>당신은 <color=#FE9A2E><b>암살자</b></color>, <color=#04B404>혼돈의 반란</color>들과 함께 <color=#000000><b>05 평의회</b></color>를 사살하십시오.</size>");

            for (int i = 0; i < PlayerManager.List.Count / 1.5f; i++)
            {
                Player player = Tools.GetRandomValue(PlayerManager.List.Where(x => x != Level05 && x != Assassin).ToList());
                player.Role.Set(RoleTypeId.ChaosRifleman);

                player.AddBroadcast(10, $"<size=25>당신은 <color=#04B404><b>혼돈의 반란</b></color>, <color=#FE9A2E><b>암살자</b></color>에 협조하여 <color=#000000><b>05 평의회</b></color>를 사살하십시오.</size>");
            }

            foreach (var player in PlayerManager.List.Where(x => x != Level05 && x != Assassin && !x.IsCHI))
            {
                player.Role.Set(RoleTypeId.FacilityGuard);
                player.Role.Set(RoleTypeId.NtfPrivate, RoleSpawnFlags.AssignInventory);

                player.AddBroadcast(10, $"<size=25>당신은 <color=#A4A4A4>시설 경비</color>, <color=#000000><b>05 평의회</b></color>를 안전하게 탈출시키십시오.</size>");
            }

            yield break;
        }

        public IEnumerator<float> AutoWarhead()
        {
            yield return Timing.WaitForSeconds(9 * 60);

            Exiled.API.Features.Cassie.MessageTranslated("", $"1분 뒤 <color=red>자동핵</color>이 작동됩니다.");

            yield return Timing.WaitForSeconds(1 * 60);

            DeadmanSwitch.StartWarhead();
        }

        public void OnEscaping(Exiled.Events.EventArgs.Player.EscapingEventArgs ev)
        {
            if (ev.Player == Level05 && ev.Player.Role.Type == RoleTypeId.Scientist)
            {
                Round.IsLocked = false;
                Timing.RunCoroutine(Tools.SetWinner(PlayerManager.List.Where(x => x.LeadingTeam == LeadingTeam.FacilityForces).ToList(), 1));

                foreach (var player in PlayerManager.List)
                {
                    player.AddBroadcast(20, $"<size=30><b><color=#000000>05 평의회</color>({Level05.DisplayNickname})</b>가 탈출하여 <u><i>강화제 제작 방법</i>을 재단에 넘기는 데 성공하였습니다.</u></size>");

                    if (player.IsCHI || player.Role.Type == RoleTypeId.ClassD)
                        player.Role.Set(RoleTypeId.Tutorial, RoleSpawnFlags.None);
                }
            }
        }

        public void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            if (ev.Player == Level05 && ev.Player.Role.Type == RoleTypeId.Scientist)
            {
                Round.IsLocked = false;
                Timing.RunCoroutine(Tools.SetWinner(PlayerManager.List.Where(x => x.LeadingTeam == LeadingTeam.ChaosInsurgency).ToList(), 1));

                foreach (var player in PlayerManager.List)
                {
                    player.AddBroadcast(20, $"<size=30><b><color=#000000>05 평의회</color>({Level05.DisplayNickname})</b>가 <color=red>사살당하였습니다.</color></size>");

                    if (player.IsNTF)
                        player.Role.Set(RoleTypeId.Tutorial, RoleSpawnFlags.None);
                }
            }
        }

        public void OnHandcuffing(Exiled.Events.EventArgs.Player.HandcuffingEventArgs ev)
        {
            if (ev.Target == Level05 && ev.Target.Role.Type == RoleTypeId.Scientist)
                ev.IsAllowed = false;
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            IEnumerable<Player> players = PlayerManager.List.Where(x => x.IsAlive && !x.IsNPC);

            if (players.Count() == 1)
                Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 5));

            else if (players.Count() > 1)
                Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 1));
        }
    }
}
