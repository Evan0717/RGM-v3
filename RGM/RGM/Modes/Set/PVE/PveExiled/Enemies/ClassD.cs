using MEC;
using PlayerRoles.FirstPersonControl;
using UnityEngine;
using InventorySystem.Items.Firearms.Modules;
using InventorySystem;
using InventorySystem.Items;
using System.Collections.Generic;
using NetworkManagerUtils.Dummies;
using Exiled.API.Enums;
using CustomPlayerEffects;

namespace RGM.Modes.PveExiledSystem.Enemies
{
    public class ClassD : Enemy
    {
        float fireRate = 0.5f;
        float updateDuration = 0.1f;
        bool canShoot = true;
        float moveBackMinDist = 4f;

        CoroutineHandle followootine;
        CoroutineHandle targetSetRootine;
        CoroutineHandle enemyRootine;

        DummyAction? shootAction;

        InventorySystem.Items.Firearms.Firearm firearm;
        InventorySystem.Items.Firearms.Modules.MagazineModule magModule;
        public ClassD(string enemyName, Vector3 spawnPos, int id, Dictionary<int, Enemy> container, WaveConfig waveConfig) : base(enemyName, spawnPos, id, container, waveConfig)
        {
            range = 20;
            range = range * range;
            moveBackMinDist = moveBackMinDist * moveBackMinDist;
            selfPlayer.Role.Set(PlayerRoles.RoleTypeId.ClassD, SpawnReason.ForceClass);
            selfPlayer.IsSpectatable = false;
            selfPlayer.EnableEffect<MovementBoost>(30, -1, false);
            selfPlayer.EnableEffect<SilentWalk>(200, -1, false);
            selfPlayer.EnableEffect<SpawnProtected>(3, true);
            selfPlayer.ClearInventory();
            selfPlayer.MaxHealth = 80 + waveConfig.MulCount*5;//30명 -> 220HP
            selfPlayer.Health = 80 + waveConfig.MulCount * 5;
            fpc = selfPlayer.RoleManager.CurrentRole as IFpcRole;

            ItemBase item = selfPlayer.Inventory.ServerAddItem(ItemType.GunCOM18, ItemAddReason.AdminCommand);
            selfPlayer.Inventory.ServerSelectItem(item.ItemSerial);
            firearm = item as InventorySystem.Items.Firearms.Firearm;
            firearm.TryGetModule<MagazineModule>(out magModule);

            selfPlayer.Position = spawnPos;
            Timing.CallDelayed(0.5f, () => {
                if (removed) return;
                foreach (DummyAction a in DummyActionCollector.ServerGetActions(hub))
                {
                    if (a.Name.EndsWith("Shoot->Click")) { shootAction = a; break; }
                }
                targetSetRootine = Timing.RunCoroutine(RerollTarget());
                followootine = Timing.RunCoroutine(FollowLoop());
                enemyRootine = Timing.RunCoroutine(EnemyFunction());
            });
        }
        public override void RemoveEnemy()
        {
            if (removed) return;
            removed = true;
            Timing.KillCoroutines(targetSetRootine);
            Timing.KillCoroutines(followootine);
            Timing.KillCoroutines(enemyRootine);

            base.RemoveEnemy();//인벤클리어&이벤트끊기&리스트제거
        }

        private IEnumerator<float> EnemyFunction()
        {
            while (!removed)
            {
                yield return Timing.WaitForSeconds(updateDuration);
                FollowAndLook();
                if (!canShoot) continue;
                if (targetPlayer == null) continue;

                Vector3 lookDirection = targetPlayer.Position - selfPlayer.Position;
                if (lookDirection.sqrMagnitude > 0)
                {
                    if (lookDirection.sqrMagnitude > range) continue;
                    bool shootCast = Physics.Raycast(selfPlayer.Position, lookDirection.normalized, out RaycastHit _, maxDistance: lookDirection.magnitude, layerMask: mask, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
                    if (shootCast) continue;

                    canShoot = false;

                    //Server.ExecuteCommand($"/dummy action {selfPlayer.Id}. GunCOM18_(#{firearm.ItemSerial}) Shoot->Click");
                    if (!shootAction.HasValue)
                    {
                        foreach (var a in DummyActionCollector.ServerGetActions(hub))
                        {
                            if (a.Name.EndsWith("Shoot->Click")) { shootAction = a; break; }
                        }
                        continue;
                    }
                    shootAction.Value.Action();

                    magModule.ServerModifyAmmo(1);
                    Timing.CallDelayed(fireRate, () => canShoot = true);
                    if (followEnabled && lookDirection.sqrMagnitude < moveBackMinDist)
                    {
                        followEnabled = false;
                        Timing.CallDelayed(1, () => { if (removed) return; followEnabled = true; });
                    }
                }
            }
        }
    }
}
