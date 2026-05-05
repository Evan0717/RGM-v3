using Exiled.API.Features;
using MEC;
using System.Collections.Generic;

namespace RGM.Modes.PveExiledSystem.SpecialWaves
{
    public class Upgraded : SpecialWave
    {
        private CoroutineHandle runningSpecialWave;

        public override string SpecialWaveName { get; } = "<color=#ff22dd>강화</color>";
        public override string SoundtrackName { get; } = "SWave_Upgraded";

        public override void Enable(RoundHandler roundHandler, WaveConfig waveConfig, WaveConfig.WaveInfo waveInfo)
        {
            Map.ShowHint("적들이 상위 직업으로 업그레이드 됩니다.", 10);

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
                string enemyName = "Gunner";
                switch (spawnInfo.EnemyName)
                {
                    case "Gunner": enemyName = "Scout"; break;
                    case "ClassD": enemyName = "Gunner"; break;
                    case "Scout": enemyName = "Rifleman"; break;
                    case "Cloaker": enemyName = "Demolisher"; break;
                    case "Pyromancer": enemyName = "Striker"; break;
                    case "Juggernaut": enemyName = "Juggernaut"; break;
                    case "Demolisher": enemyName = "Demolisher"; break;
                    case "Tranquilizer": enemyName = "Sniper"; break;
                    case "Rifleman": enemyName = "Rifleman"; break;
                    case "Zombie": enemyName = "Rifleman"; break;
                    case "Assassin": enemyName = "Assassin"; break;
                    case "Striker": enemyName = "Juggernaut"; break;
                    case "Sniper": enemyName = "Sniper"; break;
                    default: enemyName = "Gunner"; break;
                }
                for (int i = 0; i < (int)((spawnInfo.Amount + waveConfig.MulCount * spawnInfo.EnemyPerPlayer)*0.75f); i++)
                {
                    spawnQueue.Add(enemyName);
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