using Exiled.API.Features.Items;
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
    public class Juggernaut : Enemy
    {
        //float fireRate = 0.04f;//margin
        float updateDuration = 0.1f;
        float moveBackMinDist = 5f;

        float chargeCool = 10;
        float shootingMinTime = 7;

        bool shooting = false;
        bool attackStateChangeable = true;

        CoroutineHandle followootine;
        CoroutineHandle targetSetRootine;
        CoroutineHandle enemyRootine;

        DummyAction? holdAction;
        DummyAction? releaseAction;
        DummyAction? aimAction;
        DummyAction? unaimAction;

        InventorySystem.Items.Firearms.Firearm firearm;
        InventorySystem.Items.Firearms.Modules.MagazineModule magModule;
        public Juggernaut(string enemyName, Vector3 spawnPos, int id, Dictionary<int, Enemy> container, WaveConfig waveConfig) : base(enemyName, spawnPos, id, container, waveConfig)
        {
            range = 50;
            range = range * range;
            moveBackMinDist = moveBackMinDist * moveBackMinDist;
            chargeCool = 10 - waveConfig.Difficulty * 3;
            shootingMinTime = 7 + waveConfig.Difficulty * 4;
            getCloserPlayerCycle = 2 - waveConfig.Difficulty*0.5f;
            selfPlayer.Role.Set(PlayerRoles.RoleTypeId.ChaosConscript, SpawnReason.ForceClass);
            selfPlayer.EnableEffect<Slowness>(30, -1, false);
            selfPlayer.EnableEffect<SpawnProtected>(3, true);
            selfPlayer.ClearInventory();
            selfPlayer.MaxHealth = 300 + waveConfig.MulCount *9;//30명 -> 570HP
            selfPlayer.Health = 300 + waveConfig.MulCount * 9;
            fpc = selfPlayer.RoleManager.CurrentRole as IFpcRole;

            selfPlayer.Inventory.ServerAddItem(ItemType.ArmorHeavy, ItemAddReason.AdminCommand);
            Firearm item = Firearm.Create(FirearmType.Logicer);
            item.Give(selfPlayer);
            selfPlayer.Inventory.ServerSelectItem(item.Serial);
            firearm = item.Base;
            firearm.TryGetModule<MagazineModule>(out magModule);
            item.Inaccuracy = 0;

            selfPlayer.Position = spawnPos;
            Timing.CallDelayed(0.5f, () => {
                if (removed) return;
                NullActionCheck();
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
            if (!aimAction.HasValue)
            {
                foreach (DummyAction a in DummyActionCollector.ServerGetActions(hub))
                {
                    if (a.Name.EndsWith("Zoom->Hold")) { aimAction = a; break; }
                }
            }
            if (!unaimAction.HasValue)
            {
                foreach (DummyAction a in DummyActionCollector.ServerGetActions(hub))
                {
                    if (a.Name.EndsWith("Zoom->Click")) { unaimAction = a; break; }
                }
            }
        }
        public void ReleaseTrigger()
        {
            if (shooting)
            {
                shooting = false;
                releaseAction.Value.Action();
                unaimAction.Value.Action();
                attackStateChangeable = false;
                Timing.CallDelayed(chargeCool, () => attackStateChangeable = true);
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
                if (followEnabled && lookDirection.sqrMagnitude < moveBackMinDist)
                {
                    followEnabled = false;
                    Timing.CallDelayed(1, () => followEnabled = true);
                }
                if (!attackStateChangeable) { magModule.ServerModifyAmmo(50); continue; }
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
                    magModule.ServerModifyAmmo(50);

                    if (shooting) continue;

                    shooting = true;
                    attackStateChangeable = false;
                    NullActionCheck();
                    aimAction.Value.Action();
                    Timing.CallDelayed(3, () => {
                        if (removed) return;
                        holdAction.Value.Action();
                        Timing.CallDelayed(shootingMinTime, () => { if (removed) return; attackStateChangeable = true; });
                    });
                }
            }
        }
    }
}
