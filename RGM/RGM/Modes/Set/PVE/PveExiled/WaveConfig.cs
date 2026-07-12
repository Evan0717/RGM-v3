using System.Collections.Generic;
using PlayerEventArgs = Exiled.Events.EventArgs.Player;
using MapEventArgs = Exiled.Events.EventArgs.Map;
using ServerEventArgs = Exiled.Events.EventArgs.Server;

namespace RGM.Modes.PveExiledSystem
{
    public abstract class WaveConfig
    {
        public abstract bool IsSpecial { get; }
        public abstract int Difficulty { get; }
        public abstract string DifficultyName { get; }

        public int MulCount { get; set; } = 1;

        public virtual void OnThrownProjectile(PlayerEventArgs.ThrownProjectileEventArgs ev) { }

        public abstract void OnHurting(PlayerEventArgs.HurtingEventArgs ev);
        public void OnPickupAdded(MapEventArgs.PickupAddedEventArgs ev)
        {
            if (ev.Pickup.PreviousOwner == null) return;
            if (!ev.Pickup.PreviousOwner.IsNPC) return;
            ev.Pickup.Destroy();
        }
        public void OnPlacingBulletHole(MapEventArgs.PlacingBulletHoleEventArgs ev) { ev.IsAllowed = false; }
        public void OnRespawningTeam(ServerEventArgs.RespawningTeamEventArgs ev) { ev.IsAllowed = false; }

        public void OnAnnouncingScpTermination(MapEventArgs.AnnouncingScpTerminationEventArgs ev) { ev.IsAllowed = false; }

        public void OnChangingRole(PlayerEventArgs.ChangingRoleEventArgs ev)
        {
            if (ev.Reason == Exiled.API.Enums.SpawnReason.Resurrected)
            {
                ev.IsAllowed = false;
                return;
            }
        }

        public abstract WaveInfo[] Waves { get; }

        //타입
        public class EnemySpawnInfo
        {
            public string EnemyName { get; }
            public float Amount { get; }
            public float EnemyPerPlayer { get; }
            public EnemySpawnInfo(string enemyType, float amount = 1, float maxAmount = 1)
            {
                EnemyName = enemyType;
                Amount = amount;
                if (amount > maxAmount)
                {
                    EnemyPerPlayer = 0;
                }
                else
                {
                    EnemyPerPlayer = (maxAmount - amount) / 29f;
                }
            }
        }
        public class SupplySpawnInfo
        {
            public ItemType Type { get; }
            public float Amount { get; }
            public float ItemPerPlayer { get; }

            public SupplySpawnInfo(ItemType type, float amount, float maxAmount = 1)
            {
                Type = type;
                Amount = amount;
                if (amount > maxAmount)
                {
                    ItemPerPlayer = 0;
                }
                else
                {
                    ItemPerPlayer = (maxAmount - amount) / 29f;
                }
            }
        }
        public class SupplyGiveInfo
        {
            public ItemType Type { get; }

            public SupplyGiveInfo(ItemType type)
            {
                Type = type;
            }
        }
        public class WaveInfo
        {
            public List<EnemySpawnInfo> EnemySpawnInfos { get; }
            public int IntermissionTime { get; }
            public List<SupplySpawnInfo> SupplySpawnInfos { get; }
            public List<ItemType> SupplyGiveInfos { get; }

            public float MaxEnemyCount { get; }
            public float MaxEnemyPerPlayer { get; }
            public float MinEnemyCount { get; }
            public float MinEnemyPerPlayer { get; }

            public string BCtext { get; }

            public WaveInfo(
                List<EnemySpawnInfo> enemySpawnInfos,
                int intermissionTime,
                List<SupplySpawnInfo> supplySpawnInfos,
                string bcText,
                List<ItemType> supplyGiveInfos = null,
                float maxEnemyCount = 20,
                float maxMaxEnemyCount = 20,
                float minEnemyCount = 10,
                float maxMinEnemyCount = 10)
            {
                IntermissionTime = intermissionTime;
                SupplySpawnInfos = supplySpawnInfos;
                EnemySpawnInfos = enemySpawnInfos;
                BCtext = bcText;
                SupplyGiveInfos = supplyGiveInfos;

                MaxEnemyCount = maxEnemyCount;
                if (maxEnemyCount > maxMaxEnemyCount) MaxEnemyPerPlayer = 0;
                else MaxEnemyPerPlayer = (maxMaxEnemyCount - maxEnemyCount) / 29f;

                MinEnemyCount = minEnemyCount;
                if (minEnemyCount > maxMinEnemyCount) MinEnemyPerPlayer = 0;
                else MinEnemyPerPlayer = (maxMinEnemyCount - minEnemyCount) / 29f;
            }
        }
    }
}