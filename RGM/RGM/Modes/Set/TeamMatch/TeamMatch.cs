using System.Collections.Generic;
using System.Linq;
using Decals;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using UnityEngine;
using static RGM.Variables.Variable;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.TeamMatch)]
    class TeamMatch : Mode
    {
        public override string Name => "팀 데스매치";
        public override string Description => "팀원과 함께 상대 팀을 무찌르고, 승리하세요.";
        public override string Detail =>
"""
랜덤한 맵에서, 팀 승리를 위해 상대 팀을 무찌르세요.
목표 점수에 먼저 도달한 팀이 승리합니다.

목표 점수는 라운드 시작 시, 현재 서버 인원을 기준으로 결정됩니다.

<b>[Map Credit]</b>
@vasileii, @sleeplessbutter
""";
        public override string Color => "CEECF5";

        public static TeamMatch Instance;

        private List<Player> _teamA = new();
        private List<Player> _teamB = new();
        private List<Vector3> _teamASpawns = new();
        private List<Vector3> _teamBSpawns = new();

        // Schematic: Spot A/B | YAML player_spawnpoints: Spawn_ClassD_*/Spawn_Scientist_*
        private static readonly string[] TeamASpawnKeys = { "Spot A", "Spawn_ClassD" };
        private static readonly string[] TeamBSpawnKeys = { "Spot B", "Spawn_Scientist" };

        private static readonly List<string> TdmMaps =
        [
            /*"Battle",
            "Battle_Xmas2025",*/
            "Battle_Ezone"
        ];

        private CoroutineHandle _onModeStarted;
        private CoroutineHandle _cleanupDecals;
        private CoroutineHandle _scoreHint;
        private int _targetScore;
        private int _teamAScore;
        private int _teamBScore;
        private bool _isMatchEnded;
        private bool _isModeActive;
        private int _modeId;

        public override void OnEnabled()
        {
            _modeId++;
            _teamAScore = 0;
            _teamBScore = 0;
            _isMatchEnded = false;
            _isModeActive = true;
            Round.IsLocked = true;
            Respawn.PauseWaves();

            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.SpawnedRagdoll += OnSpawnedRagdoll;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.DroppingAmmo += OnDroppingAmmo;
            Exiled.Events.Handlers.Player.Shot += OnShot;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
            _cleanupDecals = Timing.RunCoroutine(CleanDecals());
            _scoreHint = Timing.RunCoroutine(ScoreHintCoroutine());
        }

        public override void OnDisabled()
        {
            _isModeActive = false;

            Exiled.Events.Handlers.Player.Died -= OnDied;
            Exiled.Events.Handlers.Player.SpawnedRagdoll -= OnSpawnedRagdoll;
            Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;
            Exiled.Events.Handlers.Player.DroppingAmmo -= OnDroppingAmmo;
            Exiled.Events.Handlers.Player.Shot -= OnShot;

            Timing.KillCoroutines(_onModeStarted);
            Timing.KillCoroutines(_cleanupDecals);
            Timing.KillCoroutines(_scoreHint);

            foreach (var door in Door.List)
                door.Unlock();

            _teamASpawns.Clear();
            _teamBSpawns.Clear();
        }

        public IEnumerator<float> OnModeStarted()
        {
            foreach (var door in Door.List)
            {
                door.IsOpen = true;
                door.Lock(DoorLockType.AdminCommand);
            }

            Tools.LoadMap(TdmMaps.GetRandomValue());

            yield return Timing.WaitForSeconds(1f);

            _teamASpawns = Tools.GetSpawnPositions(TeamASpawnKeys);
            _teamBSpawns = Tools.GetSpawnPositions(TeamBSpawnKeys);

            if (_teamASpawns.Count == 0 || _teamBSpawns.Count == 0)
                Log.Error($"[TeamMatch] 스폰 포인트를 찾지 못했습니다. A={_teamASpawns.Count}, B={_teamBSpawns.Count}");

            var players = PlayerManager.List.ToList();
            players.ShuffleList();

            _targetScore = players.Count * 4 > 100 ? 100 : players.Count * 4; 
            int halfCount = players.Count / 2;

            _teamA = players.Take(halfCount).ToList();
            _teamB = players.Skip(halfCount).ToList();

            foreach (var player in _teamA)
            {
                player.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.None);
                yield return Timing.WaitForOneFrame;
                player.ClearInventory();
                player.Position = _teamASpawns.GetRandomValue();
                foreach (var item in Items())
                    player.AddItem(item);
            }

            foreach (var player in _teamB)
            {
                player.Role.Set(RoleTypeId.Scientist, RoleSpawnFlags.None);
                yield return Timing.WaitForOneFrame;
                player.ClearInventory();
                player.Position = _teamBSpawns.GetRandomValue();
                foreach (var item in Items())
                    player.AddItem(item);
            }

            foreach (var player in PlayerManager.List)
                player.AddBroadcast(10, $"<size=36><b>목표 점수: <color=#D23265>{_targetScore}</color></b></size>");
        }

        public List<ItemType> Items()
        {
            List<ItemType> Guns = new List<ItemType> {
                ItemType.GunE11SR,
                ItemType.GunFSP9,
                ItemType.GunRevolver,
                ItemType.GunCrossvec,
                ItemType.GunLogicer,
                ItemType.GunFRMG0,
                ItemType.GunAK,
                ItemType.GunShotgun

            };
            List<ItemType> CDItems = new List<ItemType> {
                ItemType.Medkit,
                ItemType.Painkillers,
                ItemType.Radio
            };
            List<ItemType> Items = new List<ItemType>();

            Items.Add(Guns.GetRandomValue());
            Items.AddRange(CDItems.Where(item => Random.Range(1, 3) == 1));
            Items.Add(ItemType.ArmorLight);

            return Items;
        }

        public void OnDied(DiedEventArgs ev)
        {
            if (_isMatchEnded || (!_teamA.Contains(ev.Player) && !_teamB.Contains(ev.Player)))
                return;

            Exiled.API.Features.Map.CleanAllItems();

            if (_teamA.Contains(ev.Attacker) && _teamB.Contains(ev.Player))
            {
                _teamAScore++;
                CheckWinner(_teamA);
            }
            else if (_teamB.Contains(ev.Attacker) && _teamA.Contains(ev.Player))
            {
                _teamBScore++;
                CheckWinner(_teamB);
            }

            if (!_isMatchEnded)
                Timing.RunCoroutine(RespawnPlayer(ev.Player, _modeId));
        }

        public void OnSpawnedRagdoll(SpawnedRagdollEventArgs ev)
        {
            ev.Ragdoll?.Destroy();
        }

        IEnumerator<float> RespawnPlayer(Player player, int modeId)
        {
            yield return Timing.WaitForSeconds(5f);

            if (!_isModeActive || modeId != _modeId || _isMatchEnded || !Round.IsLocked || player.IsAlive)
                yield break;

            if (_teamA.Contains(player))
            {
                player.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.None);
                yield return Timing.WaitForOneFrame;
                player.Position = _teamASpawns.GetRandomValue();
                player.ApplyGodMode(3);
            }
            else if (_teamB.Contains(player))
            {
                player.Role.Set(RoleTypeId.Scientist, RoleSpawnFlags.None);
                yield return Timing.WaitForOneFrame;
                player.Position = _teamBSpawns.GetRandomValue();
                player.ApplyGodMode(3);
            }
            else
            {
                yield break;
            }

            player.ClearInventory();
            foreach (var item in Items())
                player.AddItem(item);
        }

        IEnumerator<float> CleanDecals()
        {
            while (!_isMatchEnded)
            {
                yield return Timing.WaitForSeconds(30f);

                Exiled.API.Features.Map.Clean(DecalPoolType.Blood);
                Exiled.API.Features.Map.Clean(DecalPoolType.Bullet);
            }
        }

        IEnumerator<float> ScoreHintCoroutine()
        {
            while (_isModeActive && !_isMatchEnded)
            {
                string scoreText = $"<size=30><b><color=#FF5533>{_teamAScore}</color> : <color=#FFD700>{_teamBScore}</color></b></size>";

                foreach (var player in PlayerManager.List)
                    player.AddHint("팀 데스매치 점수", scoreText, 1.2f);

                yield return Timing.WaitForSeconds(1f);
            }
        }

        void CheckWinner(List<Player> winningTeam)
        {
            if (_teamAScore < _targetScore && _teamBScore < _targetScore)
                return;

            _isMatchEnded = true;

            var losingTeam = ReferenceEquals(winningTeam, _teamA) ? _teamB : _teamA;
            foreach (var player in losingTeam.Where(x => x.IsAlive).ToList())
                player.Kill("패배하였습니다...");

            Round.IsLocked = false;
            Timing.RunCoroutine(Tools.SetWinner(winningTeam, 3));
        }

        public void OnDroppingItem(DroppingItemEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        public void OnDroppingAmmo(DroppingAmmoEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        public void OnShot(ShotEventArgs ev)
        {
            ev.Player.AddAmmo(ev.Firearm.AmmoType, 1);
        }
    }
}
