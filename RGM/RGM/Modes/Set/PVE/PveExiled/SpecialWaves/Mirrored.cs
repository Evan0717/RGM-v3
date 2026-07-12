using Exiled.API.Features;
using MEC;
using System.Collections.Generic;
using CustomPlayerEffects;

namespace RGM.Modes.PveExiledSystem.SpecialWaves
{
    public class Mirrored : SpecialWave
    {
        private CoroutineHandle runningSpecialWave;

        public override string SpecialWaveName { get; } = "<color=#dd11e0>반전</color>";
        public override string SoundtrackName { get; } = "SWave_Mirrored";

        public override void Enable(RoundHandler roundHandler, WaveConfig waveConfig, WaveConfig.WaveInfo waveInfo)
        {
            Map.ShowHint("거꾸로", 10);
            foreach(Player player in Player.List)
            {
                if (player.Role.Type == PlayerRoles.RoleTypeId.NtfSpecialist)
                {
                    player.EnableEffect<Slowness>(200, 0, false);
                }
            }

            runningSpecialWave = Timing.RunCoroutine(DoSpecialWave(roundHandler, waveConfig, waveInfo));
        }
        public override void Disable()
        {
            if (Ended) return;
            Ended = true;
            Timing.KillCoroutines(runningSpecialWave);
            foreach (Player player in Player.List)
            {
                if (player.Role.Type == PlayerRoles.RoleTypeId.NtfSpecialist)
                {
                    player.DisableEffect<Slowness>();
                }
            }
        }

        private IEnumerator<float> DoSpecialWave(RoundHandler roundHandler, WaveConfig waveConfig, WaveConfig.WaveInfo waveInfo)
        {
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