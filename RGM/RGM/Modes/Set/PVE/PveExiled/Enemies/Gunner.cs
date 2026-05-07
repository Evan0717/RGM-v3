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
    public class Gunner : Enemy
    {
        //float fireRate = 0.04f;//margin
        float updateDuration = 0.1f;
        float moveBackMinDist = 5f;

        bool shooting = false;

        CoroutineHandle followootine;
        CoroutineHandle targetSetRootine;
        CoroutineHandle enemyRootine;

        DummyAction? holdAction;
        DummyAction? releaseAction;

        InventorySystem.Items.Firearms.Firearm firearm;
        InventorySystem.Items.Firearms.Modules.MagazineModule magModule;
        public Gunner(string enemyName, Vector3 spawnPos, int id, Dictionary<int, Enemy> container, WaveConfig waveConfig) : base(enemyName, spawnPos, id, container, waveConfig)
        {
            range = 24;
            range = range * range;
            moveBackMinDist = moveBackMinDist * moveBackMinDist;
            selfPlayer.Role.Set(PlayerRoles.RoleTypeId.ClassD, SpawnReason.ForceClass);
            selfPlayer.EnableEffect<SpawnProtected>(3, true);
            selfPlayer.ClearInventory();
            selfPlayer.MaxHealth = 100 + waveConfig .MulCount* 5;//30명 -> 250HP
            selfPlayer.Health = 100 + waveConfig.MulCount * 5;
            fpc = selfPlayer.RoleManager.CurrentRole as IFpcRole;

            ItemBase item = selfPlayer.Inventory.ServerAddItem(ItemType.GunA7, ItemAddReason.AdminCommand);
            selfPlayer.Inventory.ServerSelectItem(item.ItemSerial);
            firearm = item as InventorySystem.Items.Firearms.Firearm;
            firearm.TryGetModule<MagazineModule>(out magModule);

            selfPlayer.Position = spawnPos;
            Timing.CallDelayed(0.5f, () => {
                if (removed) return;
                NullActionCheck();
                targetSetRootine = Timing.RunCoroutine(RerollTarget());
                followootine = Timing.RunCoroutine(FollowLoop());
                enemyRootine = Timing.RunCoroutine(EnemyFunction());
                magModule.ServerModifyAmmo(50);
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
        public void NullActionCheck()
        {
            if (!holdAction.HasValue)
            {
                foreach (DummyAction a in DummyActionCollector.ServerGetActions(hub))
                {
                    if (a.Name.EndsWith("Shoot->Hold")) { holdAction = a; break; }
                }
            }
            if (!releaseAction.HasValue)
            {
                foreach (DummyAction a in DummyActionCollector.ServerGetActions(hub))
                {
                    if (a.Name.EndsWith("Shoot->Click")) { releaseAction = a; break; }
                }
            }
        }
        public void ReleaseTrigger()
        {
            if (shooting)
            {
                shooting = false;
                releaseAction.Value.Action();
            }
        }

        private IEnumerator<float> EnemyFunction()
        {
            while (!removed)
            {
                yield return Timing.WaitForSeconds(updateDuration);
                FollowAndLook();
                if (targetPlayer == null) continue;

                //Shoot판단
                Vector3 lookDirection = targetPlayer.Position - selfPlayer.Position;
                if (lookDirection.sqrMagnitude > 0)
                {
                    if (lookDirection.sqrMagnitude > range)//사거리 밖
                    {
                        ReleaseTrigger();
                        continue;
                    }
                    bool shootCast = Physics.Raycast(selfPlayer.Position, lookDirection.normalized, out RaycastHit _, maxDistance: lookDirection.magnitude, layerMask: mask, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
                    if (shootCast)
                    {
                        ReleaseTrigger();
                        continue;
                    }
                    if (followEnabled && lookDirection.sqrMagnitude < moveBackMinDist)
                    {
                        followEnabled = false;
                        Timing.CallDelayed(1, () => { if (removed) return; followEnabled = true; });
                    }
                    if (magModule.AmmoStored <= 20)
                    {
                        magModule.ServerModifyAmmo(50);
                    }
                    if (shooting) continue;

                    shooting = true;
                    NullActionCheck();
                    holdAction.Value.Action();
                }
            }
        }
    }
}
