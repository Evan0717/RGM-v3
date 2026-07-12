using Exiled.API.Features;
using MEC;
using System.Collections.Generic;
using InventorySystem;

namespace RGM.Modes.PveExiledSystem.SpecialWaves
{
    public class Blackout : SpecialWave
    {
        private CoroutineHandle runningSpecialWave;

        public override string SpecialWaveName { get; } = "<color=#000000>블랙아웃</color>";
        public override string SoundtrackName { get; } = "SWave_Blackout";

        RoundHandler RoundHandler;

        public override void Enable(RoundHandler roundHandler, WaveConfig waveConfig, WaveConfig.WaveInfo waveInfo)
        {
            RoundHandler = roundHandler;
            Map.ShowHint("아무것도 보이지 않습니다.", 10);
            foreach(Player player in Player.List)
            {
                if (player.Role.Type == PlayerRoles.RoleTypeId.NtfSpecialist)
                {
                    player.Inventory.ServerAddItem(ItemType.Flashlight, InventorySystem.Items.ItemAddReason.AdminCommand);
                }
            }
            Map.TurnOffAllLights(1000);

            runningSpecialWave = Timing.RunCoroutine(DoSpecialWave(roundHandler, waveConfig, waveInfo));
        }
        public override void Disable()
        {
            if (Ended) return;
            Ended = true;
            Timing.KillCoroutines(runningSpecialWave);
            Map.TurnOffAllLights(1);
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