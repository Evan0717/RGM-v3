using Exiled.API.Features;
using MEC;
using System.Collections.Generic;
using CustomPlayerEffects;
using System.Linq;

namespace RGM.Modes.PveExiledSystem.SpecialWaves
{
    public class Spirit : SpecialWave
    {
        private CoroutineHandle runningSpecialWave;

        public override string SpecialWaveName { get; } = "<color=#00FFF6>스피릿</color>";
        public override string SoundtrackName { get; } = "SWave_Spirit";

        public override void Enable(RoundHandler roundHandler, WaveConfig waveConfig, WaveConfig.WaveInfo waveInfo)
        {
            Map.ShowHint("분노한 영혼들이 공격해옵니다", 10);

            runningSpecialWave = Timing.RunCoroutine(DoSpecialWave(roundHandler, waveConfig, waveInfo));
        }
        public override void Disable()
        {
            if (Ended) return;
            Ended = true;
            Timing.KillCoroutines(runningSpecialWave);
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
                Timing.CallDelayed(0.3f, () => {
                    Enemy enemy = roundHandler.enemies.Last().Value;
                    if (enemy != null && enemy.selfPlayer != null && !enemy.selfPlayer.IsScp && enemy.selfPlayer.Nickname != "Cloaker")
                    {
                        enemy.selfPlayer.EnableEffect<Fade>(255, -1, false);
                        enemy.selfPlayer.Health = enemy.selfPlayer.MaxHealth * 0.1f;
                    }
                });
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