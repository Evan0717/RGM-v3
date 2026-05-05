using Exiled.API.Features;
using MEC;
using System.Collections.Generic;
using PlayerEventArgs = Exiled.Events.EventArgs.Player;
using PlayerEvents = Exiled.Events.Handlers.Player;
using System.Linq;

namespace RGM.Modes.PveExiledSystem.SpecialWaves
{
    public class DemolisherRush : SpecialWave
    {
        private CoroutineHandle runningSpecialWave;
        private float damagerMultiplier = 1;

        public override string SpecialWaveName { get; } = "<color=\"orange\">데몰리셔 러쉬</color>";
        public override string SoundtrackName { get; } = "SWave_DemRush";

        public override void Enable(RoundHandler roundHandler, WaveConfig waveConfig, WaveConfig.WaveInfo waveInfo)
        {
            runningSpecialWave = Timing.RunCoroutine(DoSpecialWave(roundHandler, waveConfig, waveInfo));
            PlayerEvents.Hurting += OnHurting;
            Map.ShowHint("폭발과 죽음", 10);
        }
        public override void Disable()
        {
            if (Ended) return;
            Ended = true;
            PlayerEvents.Hurting -= OnHurting;
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
                for (int i = 0; i < (int)(spawnInfo.Amount + waveConfig.MulCount * spawnInfo.EnemyPerPlayer * 1.5f); i++)
                {
                    spawnQueue.Add("Demolisher");
                }
            }

            damagerMultiplier = 0.1f * (1 + waveConfig.Difficulty*0.4f);

            while (true)
            {
                if (spawnQueue.Count <= 0) break;
                string random = spawnQueue.Last();
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

        private void OnHurting(PlayerEventArgs.HurtingEventArgs ev)
        {
            if (ev.Player == null) return;
            if (ev.Attacker == null) return;
            if (!ev.Attacker.IsNPC) return;
            if (ev.Player.IsNPC) { ev.IsAllowed = false; return; }
            ev.DamageHandler.Damage *= damagerMultiplier;
        }
    }
}