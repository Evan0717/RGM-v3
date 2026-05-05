using System.Collections.Generic;
using PlayerEventArgs = Exiled.Events.EventArgs.Player;

namespace RGM.Modes.PveExiledSystem.Difficulty
{
    public class Hard : WaveConfig
    {
        public override bool IsSpecial { get; } = false;
        public override int Difficulty { get; } = 1;
        public override string DifficultyName { get; } = "<color=\"green\">어려움</color>";

        public override void OnHurting(PlayerEventArgs.HurtingEventArgs ev)
        {
            if (ev.Player == null) return;
            if (ev.Attacker == null) return;
            if (!ev.Attacker.IsNPC) return;
            if (ev.Player.IsNPC) { ev.IsAllowed = false; return; }
            if (ev.Attacker.CurrentItem == null)
            {
                if (ev.Attacker.Role.Type == PlayerRoles.RoleTypeId.Scp939) ev.DamageHandler.Damage *= 1.2f;
                if (ev.Attacker.Role.Type == PlayerRoles.RoleTypeId.Scp0492) ev.DamageHandler.Damage *= 0.75f;
                return;
            }

            switch (ev.Attacker.CurrentItem.Type)
            {
                case ItemType.SCP1509: ev.DamageHandler.Damage *= 0.5f; break;
                case ItemType.MicroHID: ev.DamageHandler.Damage *= 0.2f; break;
                case ItemType.GunLogicer: ev.DamageHandler.Damage *= 0.6f; break;
                case ItemType.ParticleDisruptor: ev.DamageHandler.Damage *= 0.5f; break;

                case ItemType.GunCrossvec: ev.DamageHandler.Damage *= 0.3f; break;
                case ItemType.GunA7: ev.DamageHandler.Damage *= 0.2f; break;
                case ItemType.GunAK: ev.DamageHandler.Damage *= 0.4f; break;
                default: ev.DamageHandler.Damage *= 0.4f; break;
            }
        }

        /*
       ClassD: 3, 40
       Gunner: 2, 30
       Scout: 2, 35
       Cloaker: 1, 6
       Zombie: 3, 20
       Rifleman: 2, 24
       Tranquilizer: 2, 10
       Pyromancer: 1, 5
       Striker: 1, 4
       Juggernaut: 1, 5
       Demolisher: 1, 3
       Sniper: 2, 10
       Assassin: 1, 1
        */

        public override WaveInfo[] Waves { get; } = new WaveInfo[15]//웨이브 구조
        {
            new WaveInfo(
                    intermissionTime: 10,
                    bcText: "웨이브 1",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                        new SupplySpawnInfo(ItemType.GunCrossvec, 1, 5),
                        new SupplySpawnInfo(ItemType.ArmorLight, 1, 5),
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("ClassD", 2, 30),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Radio},
                    maxEnemyCount:5,
                    maxMaxEnemyCount:15,
                    minEnemyCount:1,
                    maxMinEnemyCount:8
            ),
            new WaveInfo(
                    intermissionTime: 10,
                    bcText: "웨이브 2",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("ClassD", 1, 20),
                        new EnemySpawnInfo("Scout", 1, 10),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Painkillers},
                    maxEnemyCount:5,
                    maxMaxEnemyCount:15,
                    minEnemyCount:1,
                    maxMinEnemyCount:8
            ),
            new WaveInfo(
                    intermissionTime: 10,
                    bcText: "웨이브 3",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                        new SupplySpawnInfo(ItemType.SCP1344, 1, 8),
                        new SupplySpawnInfo(ItemType.GunCrossvec, 0.8f, 4),
                        new SupplySpawnInfo(ItemType.Medkit, 2, 10),
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("ClassD", 1, 20),
                        new EnemySpawnInfo("Scout", 1, 15),
                        new EnemySpawnInfo("Cloaker", 0.5f, 2.8f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Painkillers},
                    maxEnemyCount:5,
                    maxMaxEnemyCount:15,
                    minEnemyCount:1,
                    maxMinEnemyCount:8
            ),
            new WaveInfo(
                    intermissionTime: 10,
                    bcText: "웨이브 4",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("ClassD", 2, 20),
                        new EnemySpawnInfo("Scout", 1, 20),
                        new EnemySpawnInfo("Cloaker", 1, 2.8f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Painkillers},
                    maxEnemyCount:5,
                    maxMaxEnemyCount:15,
                    minEnemyCount:1,
                    maxMinEnemyCount:8
            ),
            new WaveInfo(
                    intermissionTime: 20,
                    bcText: "<color=\"red\">웨이브 5</color>",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                        new SupplySpawnInfo(ItemType.SCP268, 1, 8),
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("ClassD", 1, 10),
                        new EnemySpawnInfo("Scout", 1, 12),
                        new EnemySpawnInfo("Zombie", 1, 10),
                        new EnemySpawnInfo("Pyromancer", 1, 4.8f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Painkillers},
                    maxEnemyCount:5,
                    maxMaxEnemyCount:15,
                    minEnemyCount:1,
                    maxMinEnemyCount:8
            ),
            new WaveInfo(
                    intermissionTime: 15,
                    bcText: "웨이브 6",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                        new SupplySpawnInfo(ItemType.SCP1853, 1, 4),
                        new SupplySpawnInfo(ItemType.Medkit, 2, 10),
                        new SupplySpawnInfo(ItemType.Adrenaline, 2, 6),
                        new SupplySpawnInfo(ItemType.ArmorCombat, 1, 5),
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("ClassD", 1, 15),
                        new EnemySpawnInfo("Scout", 1, 20),
                        new EnemySpawnInfo("Zombie", 2, 12),
                        new EnemySpawnInfo("Cloaker", 0.5f, 2.8f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Painkillers, ItemType.Radio},
                    maxEnemyCount:5,
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
                        new EnemySpawnInfo("ClassD", 1, 15),
                        new EnemySpawnInfo("Scout", 1, 24),
                        new EnemySpawnInfo("Rifleman", 0.5f, 5),
                        new EnemySpawnInfo("Zombie", 2, 12),
                        new EnemySpawnInfo("Cloaker", 0.5f, 2.8f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Painkillers, ItemType.Radio},
                    maxEnemyCount:5,
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
                        new EnemySpawnInfo("ClassD", 1, 15),
                        new EnemySpawnInfo("Scout", 1, 24),
                        new EnemySpawnInfo("Rifleman", 1, 5),
                        new EnemySpawnInfo("Zombie", 2, 12),
                        new EnemySpawnInfo("Cloaker", 0.5f, 2.8f),
                        new EnemySpawnInfo("Tranquilizer", 1, 6),
                        new EnemySpawnInfo("Pyromancer", 0.5f, 2),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Medkit},
                    maxEnemyCount:5,
                    maxMaxEnemyCount:20,
                    minEnemyCount:1,
                    maxMinEnemyCount:10
            ),
            new WaveInfo(
                    intermissionTime: 15,
                    bcText: "웨이브 9",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                        new SupplySpawnInfo(ItemType.GunE11SR, 1, 4),
                        new SupplySpawnInfo(ItemType.ArmorHeavy, 1, 4),
                        new SupplySpawnInfo(ItemType.SCP500, 1, 8),
                        new SupplySpawnInfo(ItemType.Medkit, 2, 10),
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("ClassD", 1, 15),
                        new EnemySpawnInfo("Scout", 1, 24),
                        new EnemySpawnInfo("Rifleman", 1, 5),
                        new EnemySpawnInfo("Zombie", 2, 12),
                        new EnemySpawnInfo("Cloaker", 1, 4),
                        new EnemySpawnInfo("Tranquilizer", 1, 6),
                        new EnemySpawnInfo("Pyromancer", 1, 2),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Medkit},
                    maxEnemyCount:5,
                    maxMaxEnemyCount:20,
                    minEnemyCount:1,
                    maxMinEnemyCount:10
            ),
            new WaveInfo(
                    intermissionTime: 20,
                    bcText: "<color=\"red\">웨이브 10</color>",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("Scout", 1, 16),
                        new EnemySpawnInfo("Rifleman", 1, 10),
                        new EnemySpawnInfo("Zombie", 1, 8),
                        new EnemySpawnInfo("Tranquilizer", 1, 6),
                        new EnemySpawnInfo("Pyromancer", 1, 4.8f),
                        new EnemySpawnInfo("Juggernaut", 1, 3.8f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Medkit},
                    maxEnemyCount:5,
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
                        new EnemySpawnInfo("Scout", 1, 24),
                        new EnemySpawnInfo("Rifleman", 1, 12),
                        new EnemySpawnInfo("Zombie", 2, 12),
                        new EnemySpawnInfo("Cloaker", 0.5f, 4),
                        new EnemySpawnInfo("Tranquilizer", 2, 8),
                        new EnemySpawnInfo("Pyromancer", 1, 3.8f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Medkit},
                    maxEnemyCount:5,
                    maxMaxEnemyCount:26,
                    minEnemyCount:1,
                    maxMinEnemyCount:16
            ),
            new WaveInfo(
                    intermissionTime: 15,
                    bcText: "웨이브 12",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                        new SupplySpawnInfo(ItemType.SCP268, 0.9f, 6),
                        new SupplySpawnInfo(ItemType.Ammo556x45, 2, 6),
                        new SupplySpawnInfo(ItemType.Medkit, 3, 12),
                        new SupplySpawnInfo(ItemType.Adrenaline, 2, 8),
                        new SupplySpawnInfo(ItemType.GunFRMG0, 1, 3),
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("Scout", 1, 24),
                        new EnemySpawnInfo("Rifleman", 1, 14),
                        new EnemySpawnInfo("Zombie", 2, 12),
                        new EnemySpawnInfo("Cloaker", 1, 4),
                        new EnemySpawnInfo("Tranquilizer", 2, 8),
                        new EnemySpawnInfo("Pyromancer", 1, 3.8f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Medkit},
                    maxEnemyCount:5,
                    maxMaxEnemyCount:26,
                    minEnemyCount:1,
                    maxMinEnemyCount:16
            ),
            new WaveInfo(
                    intermissionTime: 15,
                    bcText: "웨이브 13",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                        new SupplySpawnInfo(ItemType.Ammo556x45, 2, 6),
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("Scout", 2, 24),
                        new EnemySpawnInfo("Rifleman", 1, 14),
                        new EnemySpawnInfo("Zombie", 2, 15),
                        new EnemySpawnInfo("Cloaker", 1, 6),
                        new EnemySpawnInfo("Tranquilizer", 2, 8),
                        new EnemySpawnInfo("Demolisher", 0.5f, 2.8f),
                        new EnemySpawnInfo("Pyromancer", 1, 3.8f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Medkit},
                    maxEnemyCount:5,
                    maxMaxEnemyCount:26,
                    minEnemyCount:1,
                    maxMinEnemyCount:16
            ),
            new WaveInfo(
                    intermissionTime: 15,
                    bcText: "웨이브 14",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                        new SupplySpawnInfo(ItemType.Ammo556x45, 2, 6),
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("Scout", 2, 24),
                        new EnemySpawnInfo("Rifleman", 2, 20),
                        new EnemySpawnInfo("Zombie", 2, 15),
                        new EnemySpawnInfo("Cloaker", 1, 6),
                        new EnemySpawnInfo("Tranquilizer", 2, 8),
                        new EnemySpawnInfo("Demolisher", 1, 3.8f),
                        new EnemySpawnInfo("Pyromancer", 0.5f, 2.8f),
                        new EnemySpawnInfo("Juggernaut", 0.5f, 2.8f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Medkit},
                    maxEnemyCount:5,
                    maxMaxEnemyCount:26,
                    minEnemyCount:1,
                    maxMinEnemyCount:16
            ),
            new WaveInfo(
                    intermissionTime: 30,
                    bcText: "<color=\"red\">마지막 웨이브</color>",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                        new SupplySpawnInfo(ItemType.GunLogicer, 1, 3),
                        new SupplySpawnInfo(ItemType.Ammo762x39, 2, 6),
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("ClassD", 1, 20),
                        new EnemySpawnInfo("Scout", 2, 20),
                        new EnemySpawnInfo("Rifleman", 2, 20),
                        new EnemySpawnInfo("Zombie", 2, 15),
                        new EnemySpawnInfo("Tranquilizer", 2, 6),
                        new EnemySpawnInfo("Pyromancer", 1, 5.8f),
                        new EnemySpawnInfo("Juggernaut", 1, 3.8f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Medkit, ItemType.Radio},
                    maxEnemyCount:5,
                    maxMaxEnemyCount:26,
                    minEnemyCount:1,
                    maxMinEnemyCount:16
            ),
        };
    }
}