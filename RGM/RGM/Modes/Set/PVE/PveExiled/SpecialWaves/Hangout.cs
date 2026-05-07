using Exiled.API.Features;
using MEC;
using System.Collections.Generic;
using UnityEngine;
using CustomPlayerEffects;
using NetworkManagerUtils.Dummies;

namespace RGM.Modes.PveExiledSystem.SpecialWaves
{
    public class Hangout : SpecialWave
    {
        private CoroutineHandle runningSpecialWave;

        public override string SpecialWaveName { get; } = "<color=#00FFF6>지상 전투</color>";
        public override string SoundtrackName { get; } = "SWave_Hangout";

        RoundHandler RoundHandler;
        List<Vector3> EnemySpawnPoints = new List<Vector3>();
        public override void Enable(RoundHandler roundHandler, WaveConfig waveConfig, WaveConfig.WaveInfo waveInfo)
        {
            RoundHandler = roundHandler;
            Map.ShowHint("지상에서 전투가 이루어집니다.", 10);
            foreach(Vector3 pos in roundHandler.enemySpawnPoints)
            {
                EnemySpawnPoints.Add(pos);
            }
            Vector3 surfaceSpawn = Room.Get(Exiled.API.Enums.RoomType.Surface).Position + Vector3.up;

            roundHandler.enemySpawnPoints.Clear();
            roundHandler.enemySpawnPoints.Add(surfaceSpawn);

            ReferenceHub dummy = DummyUtils.SpawnDummy("spawn");
            Player dummyPlayer = Player.Get(dummy);
            dummyPlayer.Role.Set(PlayerRoles.RoleTypeId.NtfSpecialist, Exiled.API.Enums.SpawnReason.ForceClass, PlayerRoles.RoleSpawnFlags.UseSpawnpoint);

            foreach(Player player in Player.List)
            {
                if (player.Role.Type == PlayerRoles.RoleTypeId.NtfSpecialist)
                {
                    player.Position = dummyPlayer.Position + Vector3.up;
                }
            }

            runningSpecialWave = Timing.RunCoroutine(DoSpecialWave(roundHandler, waveConfig, waveInfo));
        }
        public override void Disable()
        {
            if (Ended) return;
            Ended = true;
            Timing.KillCoroutines(runningSpecialWave);
            RoundHandler.enemySpawnPoints.Clear();
            foreach (Vector3 pos in EnemySpawnPoints)
            {
                RoundHandler.enemySpawnPoints.Add(pos);
            }
            foreach (Player player in Player.List)
            {
                if (player.Role.Type == PlayerRoles.RoleTypeId.NtfSpecialist)
                {
                    player.Position = RoundHandler.playerSpawnPoint;
                    player.EnableEffect<HeavyFooted>(255, 0, false);
                }
            }
        }

        private IEnumerator<float> DoSpecialWave(RoundHandler roundHandler, WaveConfig waveConfig, WaveConfig.WaveInfo waveInfo)
        {
            yield return Timing.WaitForSeconds(1);
            DummyUtils.DestroyAllDummies();
            List<string> spawnQueue = new List<string>();
            int maxEnemy = (int)(waveInfo.MaxEnemyCount + waveConfig.MulCount * waveInfo.MaxEnemyPerPlayer);
            int minEnemy = (int)(waveInfo.MinEnemyCount + waveConfig.MulCount * waveInfo.MinEnemyPerPlayer);
            if (minEnemy >= maxEnemy) minEnemy = maxEnemy - 2;
            foreach (WaveConfig.EnemySpawnInfo spawnInfo in waveInfo.EnemySpawnInfos)//적 스폰
            {
                for (int i = 0; i < (int)(spawnInfo.Amount + waveConfig.MulCount * spawnInfo.EnemyPerPlayer); i++)
                {
                    spawnQueue.Add(spawnInfo.EnemyName);
                }
            }
            while (true)
            {
                if (spawnQueue.Count <= 0) break;
                string random = spawnQueue.RandomItem();
                spawnQueue.Remove(random);
                roundHandler.SpawnEnemy(random);
                if (roundHandler.enemies.Count >= maxEnemy)
                {
                    while (roundHandler.enemies.Count > minEnemy && roundHandler.GetAlivePlayerCount() > 0) yield return Timing.WaitForSeconds(5);
                    if (roundHandler.GetAlivePlayerCount() <= 0) { yield return Timing.WaitForSeconds(1); break; }
                }
                else yield return Timing.WaitForSeconds(0.8f);
            }
            while (roundHandler.enemies.Count > 0 && roundHandler.GetAlivePlayerCount() > 0) yield return Timing.WaitForSeconds(5);//ㄱㄷ
            Disable();
        }
    }
}