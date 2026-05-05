using System.Collections.Generic;
using PlayerEventArgs = Exiled.Events.EventArgs.Player;

namespace RGM.Modes.PveExiledSystem.Difficulty
{
    public class Special : WaveConfig
    {
        public override bool IsSpecial { get; } = false;
        public override int Difficulty { get; } = 0;
        public override string DifficultyName { get; } = "<color=\"orange\">보통</color>";

        public override void OnHurting(PlayerEventArgs.HurtingEventArgs ev)
        {
            if (ev.Player == null) return;
            if (ev.Attacker == null) return;
            if (!ev.Attacker.IsNPC) return;
            if (ev.Player.IsNPC) { ev.IsAllowed = false; return; }
            if (ev.Attacker.CurrentItem == null)
            {
                if (ev.Attacker.Role.Type == PlayerRoles.RoleTypeId.Scp939) ev.DamageHandler.Damage *= 1.2f;
                if (ev.Attacker.Role.Type == PlayerRoles.RoleTypeId.Scp0492) ev.DamageHandler.Damage *= 0.625f;
                return;
            }

            switch (ev.Attacker.CurrentItem.Type)
            {
                case ItemType.SCP1509: ev.DamageHandler.Damage *= 0.4f; break;
                case ItemType.MicroHID: ev.DamageHandler.Damage *= 0.1f; break;
                case ItemType.GunLogicer: ev.DamageHandler.Damage *= 0.4f; break;
                case ItemType.ParticleDisruptor: ev.DamageHandler.Damage *= 0.3f; break;

                case ItemType.GunCrossvec: ev.DamageHandler.Damage *= 0.3f; break;
                case ItemType.GunA7: ev.DamageHandler.Damage *= 0.15f; break;
                case ItemType.GunAK: ev.DamageHandler.Damage *= 0.3f; break;
                default: ev.DamageHandler.Damage *= 0.2f; break;
            }
        }

        /*
       ClassD: 5, 50
       Gunner: 3, 40
       Scout: 3, 35
       Cloaker: 1, 8
       Zombie: 4, 24
       Rifleman: 2, 24
       Tranquilizer: 2, 16
       Pyromancer: 1, 6
       Striker: 1, 4
       Juggernaut: 1, 5
       Demolisher: 1, 4
       Sniper: 2, 12
       Assassin: 1, 1
        */

        public override WaveInfo[] Waves { get; } = new WaveInfo[15]//웨이브 구조
        {
            new WaveInfo(
                    intermissionTime: 10,
                    bcText: "웨이브 1",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                        new SupplySpawnInfo(ItemType.GunFSP9, 1, 5),
                        new SupplySpawnInfo(ItemType.ArmorLight, 1, 5),
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("ClassD", 2, 30),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Adrenaline},
                    maxEnemyCount:6,
                    maxMaxEnemyCount:20,
                    minEnemyCount:1,
                    maxMinEnemyCount:10
            ),
            new WaveInfo(
                    intermissionTime: 10,
                    bcText: "웨이브 2",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("ClassD", 3, 40),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Painkillers},
                    maxEnemyCount:6,
                    maxMaxEnemyCount:20,
                    minEnemyCount:1,
                    maxMinEnemyCount:10
            ),
            new WaveInfo(
                    intermissionTime: 10,
                    bcText: "웨이브 3",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                        new SupplySpawnInfo(ItemType.Medkit, 1, 5),
                        new SupplySpawnInfo(ItemType.ArmorLight, 0.8f, 4),
                        new SupplySpawnInfo(ItemType.SCP1344, 1, 6),
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("ClassD", 1, 25),
                        new EnemySpawnInfo("Scout", 1, 10),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Painkillers},
                    maxEnemyCount:6,
                    maxMaxEnemyCount:20,
                    minEnemyCount:1,
                    maxMinEnemyCount:10
            ),
            new WaveInfo(
                    intermissionTime: 10,
                    bcText: "웨이브 4",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("ClassD", 2, 25),
                        new EnemySpawnInfo("Scout", 1, 15),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Painkillers},
                    maxEnemyCount:6,
                    maxMaxEnemyCount:20,
                    minEnemyCount:1,
                    maxMinEnemyCount:10
            ),
            new WaveInfo(
                    intermissionTime: 20,
                    bcText: "<color=\"red\">웨이브 5</color>",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("ClassD", 1, 16),
                        new EnemySpawnInfo("Scout", 1, 12),
                        new EnemySpawnInfo("Zombie", 2, 16),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Painkillers},
                    maxEnemyCount:6,
                    maxMaxEnemyCount:20,
                    minEnemyCount:1,
                    maxMinEnemyCount:10
            ),
            new WaveInfo(
                    intermissionTime: 15,
                    bcText: "웨이브 6",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                        new SupplySpawnInfo(ItemType.GunCrossvec, 1, 10),
                        new SupplySpawnInfo(ItemType.Medkit, 2, 8),
                        new SupplySpawnInfo(ItemType.Adrenaline, 1, 6),
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("ClassD", 2, 25),
                        new EnemySpawnInfo("Scout", 1, 20),
                        new EnemySpawnInfo("Zombie", 1, 10),
                        new EnemySpawnInfo("Tranquilizer", 1, 4.5f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Painkillers},
                    maxEnemyCount:6,
                    maxMaxEnemyCount:20,
                    minEnemyCount:1,
                    maxMinEnemyCount:10
            ),
            new WaveInfo(
                    intermissionTime: 15,
                    bcText: "웨이브 7",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("ClassD", 2, 25),
                        new EnemySpawnInfo("Scout", 2, 22),
                        new EnemySpawnInfo("Zombie", 1, 10),
                        new EnemySpawnInfo("Tranquilizer", 1, 6),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Painkillers},
                    maxEnemyCount:6,
                    maxMaxEnemyCount:20,
                    minEnemyCount:1,
                    maxMinEnemyCount:10
            ),
            new WaveInfo(
                    intermissionTime: 15,
                    bcText: "웨이브 8",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("ClassD", 2, 25),
                        new EnemySpawnInfo("Scout", 2, 22),
                        new EnemySpawnInfo("Zombie", 1, 12),
                        new EnemySpawnInfo("Tranquilizer", 2, 6),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Painkillers},
                    maxEnemyCount:6,
                    maxMaxEnemyCount:20,
                    minEnemyCount:1,
                    maxMinEnemyCount:10
            ),
            new WaveInfo(
                    intermissionTime: 15,
                    bcText: "웨이브 9",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                        new SupplySpawnInfo(ItemType.SCP500, 1, 5),
                        new SupplySpawnInfo(ItemType.ArmorLight, 0.8f, 4),
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("ClassD", 2, 25),
                        new EnemySpawnInfo("Scout", 2, 24),
                        new EnemySpawnInfo("Zombie", 2, 15),
                        new EnemySpawnInfo("Tranquilizer", 2, 8),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Painkillers},
                    maxEnemyCount:6,
                    maxMaxEnemyCount:20,
                    minEnemyCount:1,
                    maxMinEnemyCount:10
            ),
            new WaveInfo(
                    intermissionTime: 20,
                    bcText: "<color=\"red\">웨이브 10</color>",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                        new SupplySpawnInfo(ItemType.SCP268, 1, 7),
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("ClassD", 1, 10),
                        new EnemySpawnInfo("Scout", 1, 20),
                        new EnemySpawnInfo("Zombie", 1, 10),
                        new EnemySpawnInfo("Pyromancer", 1, 4.8f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Painkillers},
                    maxEnemyCount:6,
                    maxMaxEnemyCount:20,
                    minEnemyCount:1,
                    maxMinEnemyCount:10
            ),
            new WaveInfo(
                    intermissionTime: 15,
                    bcText: "웨이브 11",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("ClassD", 1, 20),
                        new EnemySpawnInfo("Scout", 1, 26),
                        new EnemySpawnInfo("Zombie", 2, 15),
                        new EnemySpawnInfo("Tranquilizer", 2, 10),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Painkillers},
                    maxEnemyCount:6,
                    maxMaxEnemyCount:20,
                    minEnemyCount:1,
                    maxMinEnemyCount:10
            ),
            new WaveInfo(
                    intermissionTime: 15,
                    bcText: "웨이브 12",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                        new SupplySpawnInfo(ItemType.SCP1344, 1, 4),
                        new SupplySpawnInfo(ItemType.ArmorCombat, 1, 5),
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("ClassD", 1, 20),
                        new EnemySpawnInfo("Scout", 2, 26),
                        new EnemySpawnInfo("Zombie", 2, 15),
                        new EnemySpawnInfo("Cloaker", 0.5f, 4.8f),
                        new EnemySpawnInfo("Tranquilizer", 2, 10),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Painkillers},
                    maxEnemyCount:6,
                    maxMaxEnemyCount:20,
                    minEnemyCount:1,
                    maxMinEnemyCount:10
            ),
            new WaveInfo(
                    intermissionTime: 15,
                    bcText: "웨이브 13",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("ClassD", 1, 20),
                        new EnemySpawnInfo("Scout", 2, 28),
                        new EnemySpawnInfo("Zombie", 2, 15),
                        new EnemySpawnInfo("Cloaker", 1, 6),
                        new EnemySpawnInfo("Tranquilizer", 3, 12),
                        new EnemySpawnInfo("Pyromancer", 0.5f, 2.8f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Painkillers},
                    maxEnemyCount:6,
                    maxMaxEnemyCount:20,
                    minEnemyCount:1,
                    maxMinEnemyCount:10
            ),
            new WaveInfo(
                    intermissionTime: 15,
                    bcText: "웨이브 14",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("ClassD", 1, 20),
                        new EnemySpawnInfo("Scout", 3, 35),
                        new EnemySpawnInfo("Zombie", 2, 15),
                        new EnemySpawnInfo("Cloaker", 1, 6),
                        new EnemySpawnInfo("Tranquilizer", 3, 12),
                        new EnemySpawnInfo("Pyromancer", 1, 2.8f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Painkillers},
                    maxEnemyCount:6,
                    maxMaxEnemyCount:20,
                    minEnemyCount:1,
                    maxMinEnemyCount:10
            ),
            new WaveInfo(
                    intermissionTime: 30,
                    bcText: "<color=\"red\">마지막 웨이브</color>",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                        new SupplySpawnInfo(ItemType.GunE11SR, 1, 3),
                        new SupplySpawnInfo(ItemType.ArmorHeavy, 1, 2.8f),
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("ClassD", 1, 20),
                        new EnemySpawnInfo("Scout", 2, 20),
                        new EnemySpawnInfo("Zombie", 4, 24),
                        new EnemySpawnInfo("Pyromancer", 1, 3.8f),
                        new EnemySpawnInfo("Juggernaut", 1, 3.8f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Painkillers},
                    maxEnemyCount:6,
                    maxMaxEnemyCount:20,
                    minEnemyCount:1,
                    maxMinEnemyCount:10
            ),
        };
    }
}