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
    public class Rifleman : Enemy
    {
        //float fireRate = 0.04f;//margin
        float updateDuration = 0.1f;
        float moveBackMinDist = 5f;

        bool shooting = false;
        bool reloading = false;

        CoroutineHandle followootine;
        CoroutineHandle targetSetRootine;
        CoroutineHandle enemyRootine;

        DummyAction? holdAction;
        DummyAction? releaseAction;
        DummyAction? reloadAction;

        InventorySystem.Items.Firearms.Firearm firearm;
        InventorySystem.Items.Firearms.Modules.MagazineModule magModule;
        public Rifleman(string enemyName, Vector3 spawnPos, int id, Dictionary<int, Enemy> container, WaveConfig waveConfig) : base(enemyName, spawnPos, id, container, waveConfig)
        {
            range = 26;
            range = range * range;
            moveBackMinDist = moveBackMinDist * moveBackMinDist;
            selfPlayer.IsSpectatable = false;
            selfPlayer.Role.Set(PlayerRoles.RoleTypeId.ChaosRifleman, SpawnReason.ForceClass);
            selfPlayer.EnableEffect<SpawnProtected>(3, true);
            selfPlayer.ClearInventory();
            selfPlayer.MaxHealth = 120 + waveConfig .MulCount* 8;//30명 -> 360HP
            selfPlayer.Health = 120 + waveConfig.MulCount * 8;
            fpc = selfPlayer.RoleManager.CurrentRole as IFpcRole;

            selfPlayer.Inventory.ServerAddItem(ItemType.ArmorCombat, ItemAddReason.AdminCommand);
            ItemBase item = selfPlayer.Inventory.ServerAddItem(ItemType.GunAK, ItemAddReason.AdminCommand);
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
                magModule.ServerModifyAmmo(30);
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
            if (!reloadAction.HasValue)
            {
                foreach (DummyAction a in DummyActionCollector.ServerGetActions(hub))
                {
                    if (a.Name.EndsWith("Reload->Click")) { reloadAction = a; break; }
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
            if (magModule.AmmoStored < 10)
            {
                Reload();
            }
        }
        public void Reload()
        {
            shooting = false;
            reloading = true;
            releaseAction.Value.Action();
            Timing.CallDelayed(0.1f, () =>
            {
                if (removed) return;
                selfPlayer.AddAmmo(AmmoType.Nato762, 1);
                reloadAction.Value.Action();
                Timing.CallDelayed(4, () => { if (removed) return; reloading = false; magModule.ServerModifyAmmo(30); }); ;
            });
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

                    if (reloading) continue;
                    if (magModule.AmmoStored <= 0)
                    {
                        Reload();
                        continue;
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
