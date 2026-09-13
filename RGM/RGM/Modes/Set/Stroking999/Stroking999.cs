using AFK;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using System.Collections.Generic;
using System.Linq; // PlayersReport
using UnityEngine;
using static RGM.Variables.Variable;

namespace RGM.Modes
{
    [Mode(ModeCategory.Private, ModeInfo.Set, ModeType.Stroking999)]
    public class Stroking999 : Mode
    {
        public override string Name => "999쓰다듬기(임시)";
        public override string Description => "Scp999를 최대한 많이 쓰다듬어 승리하세요.";
        public override string Color => "fb9517";
        public override string Author => "Ragdoll";

        public override string Detail =>
            """
            가운데에 있는 Scp999를 보고 alt를 누르면 점수가 올라갑니다.
            """;

        private const float RaycastDistance = 5f;
        private int TimeLimit;
        private const string TargetObjectName = "Scp999"; // 목표 오브젝트 이름

        private static readonly int RaycastMask = 1 << 0;
        private readonly Dictionary<Player, int> _scores = new();
        private bool gameEnded = false;

        private int _time;
        private CoroutineHandle _timerHandle;

        public override void OnEnabled()
        {
            Server.FriendlyFire = true;
            Round.IsLocked = true;
            Respawn.PauseWaves();
            AFKManager._kickTime = 120500;

            TimeLimit = Random.Range(20, 56);

            foreach (var player in PlayerManager.List)
            {
                _scores[player] = 0;
            }

            Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;
            Exiled.Events.Handlers.Player.Left += OnPlayerLeft;

            _timerHandle = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.TogglingNoClip -= OnTogglingNoClip;
            Exiled.Events.Handlers.Player.Left -= OnPlayerLeft;
            Timing.KillCoroutines(_timerHandle);

            _scores.Clear();
        }

        private void OnPlayerLeft(LeftEventArgs ev)
        {
            _scores.Remove(ev.Player);
        }

        private void OnTogglingNoClip(TogglingNoClipEventArgs ev)
        {
            Player player = ev.Player;
            if (!_scores.ContainsKey(player))
                return;

            Transform cam = player.ReferenceHub.PlayerCameraReference;
            Vector3 origin = cam.position;

            if (!Physics.Raycast(origin, cam.forward, out RaycastHit hit, RaycastDistance, RaycastMask, QueryTriggerInteraction.Collide))
                return;

            if (!IsTargetObject(hit.collider.gameObject))
                return;

            if (gameEnded)
            {
                Tools.PlaySound(player.Transform, "scp-999", 2);
                return;
            }
            _scores[player]++;
            player.ShowHitMarker();

            PlayersReport[player.UserId].Damage = _scores[player];
        }

        private static bool IsTargetObject(GameObject obj)
        {
            return obj.name == TargetObjectName;
        }

        private IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(1f);
            Tools.LoadMap("Stroking999");

            yield return Timing.WaitForSeconds(0.1f);
            foreach (var player in PlayerManager.List)
            {
                player.Role.Set(RoleTypeId.Tutorial);
                player.Position = Tools.GetObjectList("SpawnSpot").GetRandomValue().position;
            }

            for (int countdown = 10; countdown > 0; countdown--)
            {
                foreach (var player in _scores.Keys)
                    player.AddBroadcast(1, $"<size=25>게임이 {countdown}초 후 시작됩니다!</size>\nalt를 눌러 999를 쓰다듬을 수 있습니다.");

                yield return Timing.WaitForSeconds(1);
            }

            while (_time < TimeLimit)
            {
                _time++;

                Player leader = null;
                int leaderScore = 0;

                foreach (var entry in _scores)
                {
                    if (entry.Value > leaderScore)
                    {
                        leaderScore = entry.Value;
                        leader = entry.Key;
                    }
                }

                string leaderText = leader != null
                    ? $"현재 1위: {leader.Nickname} ({leaderScore}회)"
                    : "현재 1위: 아직 기록 없음";

                foreach (var player in _scores.Keys)
                {
                    int remain = TimeLimit - _time;
                    player.AddBroadcast(1,
                        $"<size=20>{remain}초 남음 | 클릭 수: {_scores[player]}회\n{leaderText}</size>");
                }

                yield return Timing.WaitForSeconds(1);
            }

            AnnounceWinner();
        }

        private void AnnounceWinner()
        {
            gameEnded = true;
            int maxScore = int.MinValue;
            var tops = new List<Player>();

            foreach (var entry in _scores)
            {
                if (entry.Value > maxScore)
                {
                    maxScore = entry.Value;
                    tops.Clear();
                    tops.Add(entry.Key);
                }
                else if (entry.Value == maxScore)
                {
                    tops.Add(entry.Key);
                }
            }

            foreach (var player in _scores.Keys.Where(p => !tops.Contains(p)))
            {
                player.Kill("당신은 999에게 선택받지 못했습니다.");
            }

            Round.IsLocked = false;

            if (tops.Count == 1)
                Timing.RunCoroutine(Tools.SetWinner(tops, 5));
            else if (tops.Count > 1)
                Timing.RunCoroutine(Tools.SetWinner(tops, 3));
        }
    }
}