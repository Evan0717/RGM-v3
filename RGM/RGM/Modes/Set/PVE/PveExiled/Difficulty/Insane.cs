using System.Collections.Generic;
using PlayerEventArgs = Exiled.Events.EventArgs.Player;

namespace RGM.Modes.PveExiledSystem.Difficulty
{
    public class Insane : WaveConfig
    {
        public override bool IsSpecial { get; } = false;
        public override int Difficulty { get; } = 2;
        public override string DifficultyName { get; } = "<color=\"red\">지옥</color>";

        public override void OnHurting(PlayerEventArgs.HurtingEventArgs ev)
        {
            if (ev.Player == null) return;
            if (ev.Attacker == null) return;
            if (!ev.Attacker.IsNPC) return;
            if (ev.Player.IsNPC) { ev.IsAllowed = false; return; }
            if (ev.Attacker.CurrentItem == null)
            {
                if (ev.Attacker.Role.Type == PlayerRoles.RoleTypeId.Scp939) ev.DamageHandler.Damage *= 1.5f;
                return;
            }

            switch (ev.Attacker.CurrentItem.Type)
            {
                case ItemType.SCP1509: ev.DamageHandler.Damage *= 0.5f; break;
                case ItemType.MicroHID: ev.DamageHandler.Damage *= 0.3f; break;
                case ItemType.GunLogicer: ev.DamageHandler.Damage *= 0.9f; break;
                case ItemType.ParticleDisruptor: ev.DamageHandler.Damage *= 0.5f; break;

                case ItemType.GunCrossvec: ev.DamageHandler.Damage *= 0.5f; break;
                case ItemType.GunA7: ev.DamageHandler.Damage *= 0.35f; break;
                case ItemType.GunAK: ev.DamageHandler.Damage *= 0.6f; break;
                default: ev.DamageHandler.Damage *= 0.6f; break;
            }
        }

        /*
       ClassD: 3, 30
       Gunner: 2, 24
       Scout: 2, 24
       Cloaker: 1, 4
       Zombie: 2, 16
       Rifleman: 1, 15
       Tranquilizer: 1, 6
       Pyromancer: 1, 5
       Striker: 1, 3
       Juggernaut: 1, 4
       Demolisher: 0, 0
       Sniper: 1, 7
       Assassin: 1, 1
        */

        public override WaveInfo[] Waves { get; } = new WaveInfo[15]//웨이브 구조
        {
            new WaveInfo(
                    intermissionTime: 10,
                    bcText: "웨이브 1",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                        new SupplySpawnInfo(ItemType.SCP1344, 1, 5),
                        new SupplySpawnInfo(ItemType.GunE11SR, 1, 3.8f),
                        new SupplySpawnInfo(ItemType.ArmorHeavy, 1, 5),
                        new SupplySpawnInfo(ItemType.Ammo556x45, 2, 7),
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("Gunner", 1, 12),
                        new EnemySpawnInfo("Zombie", 0.5f, 6),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Radio},
                    maxEnemyCount:5,
                    maxMaxEnemyCount:15,
                    minEnemyCount:1,
                    maxMinEnemyCount:6
            ),
            new WaveInfo(
                    intermissionTime: 10,
                    bcText: "웨이브 2",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("Gunner", 1, 14),
                        new EnemySpawnInfo("Zombie", 1, 6),
                        new EnemySpawnInfo("Cloaker", 0.5f, 2),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Medkit},
                    maxEnemyCount:5,
                    maxMaxEnemyCount:15,
                    minEnemyCount:1,
                    maxMinEnemyCount:6
            ),
            new WaveInfo(
                    intermissionTime: 10,
                    bcText: "웨이브 3",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                        new SupplySpawnInfo(ItemType.SCP1344, 0.8f, 5),
                        new SupplySpawnInfo(ItemType.AntiSCP207, 1, 5),
                        new SupplySpawnInfo(ItemType.SCP500, 2, 12),
                        new SupplySpawnInfo(ItemType.Medkit, 2, 8),
                        new SupplySpawnInfo(ItemType.SCP268, 1, 6),
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("Gunner", 1, 16),
                        new EnemySpawnInfo("Zombie", 1, 6),
                        new EnemySpawnInfo("Cloaker", 0.5f, 2),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Medkit},
                    maxEnemyCount:5,
                    maxMaxEnemyCount:15,
                    minEnemyCount:1,
                    maxMinEnemyCount:6
            ),
            new WaveInfo(
                    intermissionTime: 10,
                    bcText: "웨이브 4",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("Gunner", 1, 16),
                        new EnemySpawnInfo("Scout", 0.5f, 8),
                        new EnemySpawnInfo("Zombie", 1, 6),
                        new EnemySpawnInfo("Cloaker", 1, 2),
                        new EnemySpawnInfo("Pyromancer", 0.5f, 1.5f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Medkit},
                    maxEnemyCount:5,
                    maxMaxEnemyCount:15,
                    minEnemyCount:1,
                    maxMinEnemyCount:6
            ),
            new WaveInfo(
                    intermissionTime: 20,
                    bcText: "<color=\"red\">웨이브 5</color>",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("Gunner", 1, 12),
                        new EnemySpawnInfo("Scout", 1, 6),
                        new EnemySpawnInfo("Zombie", 1, 6),
                        new EnemySpawnInfo("Cloaker", 0.5f, 2),
                        new EnemySpawnInfo("Tranquilizer", 1, 3.5f),
                        new EnemySpawnInfo("Pyromancer", 1, 2.8f),
                        new EnemySpawnInfo("Striker", 1, 2.8f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Medkit},
                    maxEnemyCount:5,
                    maxMaxEnemyCount:15,
                    minEnemyCount:1,
                    maxMinEnemyCount:6
            ),
            new WaveInfo(
                    intermissionTime: 15,
                    bcText: "웨이브 6",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                        new SupplySpawnInfo(ItemType.SCP1853, 1, 8),
                        new SupplySpawnInfo(ItemType.Jailbird, 1, 4.8f),
                        new SupplySpawnInfo(ItemType.SCP500, 2, 12),
                        new SupplySpawnInfo(ItemType.SCP1853, 2, 8),
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("Gunner", 1, 14),
                        new EnemySpawnInfo("Scout", 1, 8),
                        new EnemySpawnInfo("Rifleman", 0.5f, 4),
                        new EnemySpawnInfo("Zombie", 1, 8),
                        new EnemySpawnInfo("Cloaker", 1, 2.8f),
                        new EnemySpawnInfo("Tranquilizer", 1, 3),
                        new EnemySpawnInfo("Pyromancer", 0.5f, 1.8f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Medkit, ItemType.Radio, ItemType.Ammo556x45, ItemType.Ammo556x45, ItemType.GunCrossvec},
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
                        new EnemySpawnInfo("Gunner", 1, 14),
                        new EnemySpawnInfo("Scout", 1, 8),
                        new EnemySpawnInfo("Rifleman", 0.5f, 8),
                        new EnemySpawnInfo("Zombie", 1, 10),
                        new EnemySpawnInfo("Cloaker", 1, 2.8f),
                        new EnemySpawnInfo("Tranquilizer", 1, 3),
                        new EnemySpawnInfo("Sniper", 0.5f, 2.8f),
                        new EnemySpawnInfo("Pyromancer", 0.5f, 1.8f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Medkit},
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
                        new EnemySpawnInfo("Gunner", 1, 14),
                        new EnemySpawnInfo("Scout", 1, 10),
                        new EnemySpawnInfo("Rifleman", 1, 8),
                        new EnemySpawnInfo("Zombie", 1, 12),
                        new EnemySpawnInfo("Cloaker", 1, 2.8f),
                        new EnemySpawnInfo("Tranquilizer", 1, 3),
                        new EnemySpawnInfo("Sniper", 1, 5),
                        new EnemySpawnInfo("Pyromancer", 0.5f, 1.8f),
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
                        new SupplySpawnInfo(ItemType.GunSCP127, 1, 5),
                        new SupplySpawnInfo(ItemType.ArmorHeavy, 0.8f, 5),
                        new SupplySpawnInfo(ItemType.SCP500, 2, 12),
                        new SupplySpawnInfo(ItemType.Medkit, 2, 8),
                        new SupplySpawnInfo(ItemType.SCP268, 0.8f, 6),
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("Gunner", 1, 12),
                        new EnemySpawnInfo("Scout", 2, 16),
                        new EnemySpawnInfo("Rifleman", 1, 8),
                        new EnemySpawnInfo("Zombie", 1, 14),
                        new EnemySpawnInfo("Cloaker", 1, 2.8f),
                        new EnemySpawnInfo("Tranquilizer", 1, 3),
                        new EnemySpawnInfo("Sniper", 1, 5),
                        new EnemySpawnInfo("Pyromancer", 0.5f, 1.8f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Medkit, ItemType.GunE11SR, ItemType.Ammo556x45, ItemType.Ammo556x45},
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
                        new EnemySpawnInfo("Gunner", 1, 10),
                        new EnemySpawnInfo("Scout", 1, 12),
                        new EnemySpawnInfo("Rifleman", 1, 8),
                        new EnemySpawnInfo("Zombie", 1, 12),
                        new EnemySpawnInfo("Cloaker", 0.5f, 2),
                        new EnemySpawnInfo("Tranquilizer", 1, 2.8f),
                        new EnemySpawnInfo("Sniper", 0.5f, 2.8f),
                        new EnemySpawnInfo("Pyromancer", 1, 2.8f),
                        new EnemySpawnInfo("Juggernaut", 1, 4.8f),
                        new EnemySpawnInfo("Assassin", 1, 1),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Medkit},
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
                        new EnemySpawnInfo("Gunner", 1, 6),
                        new EnemySpawnInfo("Scout", 2, 20),
                        new EnemySpawnInfo("Rifleman", 1, 10),
                        new EnemySpawnInfo("Zombie", 2, 14),
                        new EnemySpawnInfo("Cloaker", 0.5f, 2),
                        new EnemySpawnInfo("Tranquilizer", 1, 4),
                        new EnemySpawnInfo("Sniper", 1, 5),
                        new EnemySpawnInfo("Pyromancer", 1, 2.8f),
                        new EnemySpawnInfo("Assassin", 0.5f, 1.8f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Medkit},
                    maxEnemyCount:5,
                    maxMaxEnemyCount:24,
                    minEnemyCount:1,
                    maxMinEnemyCount:14
            ),
            new WaveInfo(
                    intermissionTime: 15,
                    bcText: "웨이브 12",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                        new SupplySpawnInfo(ItemType.GunLogicer, 1, 3),
                        new SupplySpawnInfo(ItemType.GunFRMG0, 1, 4),
                        new SupplySpawnInfo(ItemType.AntiSCP207, 1, 3.8f),
                        new SupplySpawnInfo(ItemType.GunSCP127, 0.8f, 3.8f),
                        new SupplySpawnInfo(ItemType.SCP500, 2, 12),
                        new SupplySpawnInfo(ItemType.Medkit, 2, 8),
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("Gunner", 1, 6),
                        new EnemySpawnInfo("Scout", 2, 20),
                        new EnemySpawnInfo("Rifleman", 1, 12),
                        new EnemySpawnInfo("Zombie", 2, 14),
                        new EnemySpawnInfo("Cloaker", 0.5f, 2),
                        new EnemySpawnInfo("Tranquilizer", 1, 4),
                        new EnemySpawnInfo("Sniper", 2, 7),
                        new EnemySpawnInfo("Pyromancer", 1, 2.8f),
                        new EnemySpawnInfo("Assassin", 0.5f, 1.8f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Medkit, ItemType.Ammo556x45, ItemType.Ammo556x45, ItemType.Ammo762x39, ItemType.Ammo762x39},
                    maxEnemyCount:5,
                    maxMaxEnemyCount:24,
                    minEnemyCount:1,
                    maxMinEnemyCount:14
            ),
            new WaveInfo(
                    intermissionTime: 15,
                    bcText: "웨이브 13",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("Gunner", 1, 6),
                        new EnemySpawnInfo("Scout", 2, 20),
                        new EnemySpawnInfo("Rifleman", 1, 14),
                        new EnemySpawnInfo("Zombie", 2, 14),
                        new EnemySpawnInfo("Cloaker", 0.5f, 2),
                        new EnemySpawnInfo("Tranquilizer", 1, 4),
                        new EnemySpawnInfo("Sniper", 2, 7),
                        new EnemySpawnInfo("Pyromancer", 1, 2.8f),
                        new EnemySpawnInfo("Juggernaut", 0.5f, 1.8f),
                        new EnemySpawnInfo("Assassin", 0.5f, 1.8f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Medkit, ItemType.Ammo556x45, ItemType.Ammo556x45},
                    maxEnemyCount:5,
                    maxMaxEnemyCount:24,
                    minEnemyCount:1,
                    maxMinEnemyCount:14
            ),
            new WaveInfo(
                    intermissionTime: 15,
                    bcText: "웨이브 14",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("Gunner", 1, 6),
                        new EnemySpawnInfo("Scout", 2, 20),
                        new EnemySpawnInfo("Rifleman", 1, 16),
                        new EnemySpawnInfo("Zombie", 2, 14),
                        new EnemySpawnInfo("Cloaker", 0.5f, 2),
                        new EnemySpawnInfo("Tranquilizer", 1, 4),
                        new EnemySpawnInfo("Sniper", 2, 7),
                        new EnemySpawnInfo("Pyromancer", 1, 2.8f),
                        new EnemySpawnInfo("Juggernaut", 1, 2.8f),
                        new EnemySpawnInfo("Assassin", 0.5f, 1.8f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Medkit, ItemType.Ammo556x45, ItemType.Ammo556x45},
                    maxEnemyCount:5,
                    maxMaxEnemyCount:24,
                    minEnemyCount:1,
                    maxMinEnemyCount:14
            ),
            new WaveInfo(
                    intermissionTime: 30,
                    bcText: "<color=\"red\">마지막 웨이브</color>",
                    supplySpawnInfos: new List<SupplySpawnInfo>
                    {
                        new SupplySpawnInfo(ItemType.SCP1853, 1, 8),
                        new SupplySpawnInfo(ItemType.Medkit, 2, 10),
                        new SupplySpawnInfo(ItemType.Jailbird, 0.8f, 1.8f),
                        new SupplySpawnInfo(ItemType.SCP1344, 1, 5),
                        new SupplySpawnInfo(ItemType.ArmorHeavy, 1, 8),
                    },
                    enemySpawnInfos: new List<EnemySpawnInfo>
                    {
                        new EnemySpawnInfo("Gunner", 1, 15),
                        new EnemySpawnInfo("Scout", 2, 22),
                        new EnemySpawnInfo("Rifleman", 2, 14),
                        new EnemySpawnInfo("Zombie", 2, 16),
                        new EnemySpawnInfo("Cloaker", 0.5f, 3),
                        new EnemySpawnInfo("Tranquilizer", 1, 5),
                        new EnemySpawnInfo("Sniper", 3, 8),
                        new EnemySpawnInfo("Pyromancer", 1, 3.8f),
                        new EnemySpawnInfo("Juggernaut", 1, 2.8f),
                        new EnemySpawnInfo("Assassin", 1, 1),
                        new EnemySpawnInfo("Striker", 1, 3.6f),
                    },
                    supplyGiveInfos: new List<ItemType>(){ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Ammo9x19, ItemType.Medkit, ItemType.Ammo556x45, ItemType.Ammo556x45, ItemType.Ammo762x39, ItemType.Ammo762x39, ItemType.Radio},
                    maxEnemyCount:5,
                    maxMaxEnemyCount:24,
                    minEnemyCount:1,
                    maxMinEnemyCount:14
            ),
        };
    }
}