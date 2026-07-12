using MEC;
using PlayerRoles.FirstPersonControl;
using UnityEngine;
using InventorySystem;
using InventorySystem.Items;
using System.Collections.Generic;
using NetworkManagerUtils.Dummies;
using Exiled.API.Enums;
using CustomPlayerEffects;
using RelativePositioning;

namespace RGM.Modes.PveExiledSystem.Enemies
{
    public class Pyromancer : Enemy
    {
        //float fireRate = 0.04f;//margin
        float updateDuration = 0.1f;
        float moveBackMinDist = 5f;
        float absShotRange = 10;

        bool shooting = false;

        CoroutineHandle followootine;
        CoroutineHandle targetSetRootine;
        CoroutineHandle enemyRootine;

        DummyAction? holdAction;
        DummyAction? releaseAction;

        InventorySystem.Items.MicroHID.MicroHIDItem hid;
        public Pyromancer(string enemyName, Vector3 spawnPos, int id, Dictionary<int, Enemy> container, WaveConfig waveConfig) : base(enemyName, spawnPos, id, container, waveConfig)
        {
            range = 15;
            range = range * range;
            moveBackMinDist = moveBackMinDist * moveBackMinDist;
            selfPlayer.Role.Set(PlayerRoles.RoleTypeId.ChaosRepressor, SpawnReason.ForceClass);
            selfPlayer.EnableEffect<SpawnProtected>(5, true);
            selfPlayer.ClearInventory();
            selfPlayer.MaxHealth = 250 + waveConfig.MulCount *12;//35명 -> 670HP
            selfPlayer.Health = 250 + waveConfig.MulCount * 12;
            fpc = selfPlayer.RoleManager.CurrentRole as IFpcRole;

            selfPlayer.Inventory.ServerAddItem(ItemType.ArmorHeavy, ItemAddReason.AdminCommand);
            ItemBase item = selfPlayer.Inventory.ServerAddItem(ItemType.MicroHID, ItemAddReason.AdminCommand);
            selfPlayer.Inventory.ServerSelectItem(item.ItemSerial);
            hid = item as InventorySystem.Items.MicroHID.MicroHIDItem;

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
        }
        public void ReleaseTrigger()
        {
            if (shooting)
            {
                absShotRange -= 2;
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
                    if (shootCast && lookDirection.magnitude >absShotRange)
                    {
                        ReleaseTrigger();
                        continue;
                    }
                    hid.EnergyManager.ServerSetEnergy(hid.ItemSerial, 100);
                    if (!shootCast && followEnabled && lookDirection.sqrMagnitude < moveBackMinDist)
                    {
                        followEnabled = false;
                        Timing.CallDelayed(1, () => { if (removed) return; followEnabled = true; });
                    }

                    if (shooting) continue;

                    shooting = true;
                    absShotRange += 2;
                    NullActionCheck();
                    holdAction.Value.Action();
                }
            }
        }

        protected override void FollowAndLook()
        {
            Vector3 direction = targetPosition - selfPlayer.Position;
            direction.y = 0;
            if (fpc == null)
            {
            }
            if (direction.sqrMagnitude > 0)
            {
                if (followEnabled) fpc.FpcModule.Motor.ReceivedPosition = new RelativePosition(selfPlayer.Position + direction.normalized);
                else fpc.FpcModule.Motor.ReceivedPosition = new RelativePosition(selfPlayer.Position - hub.transform.forward);
            }
            if (direction.sqrMagnitude < 0.25 || direction.sqrMagnitude > 100)
            {
                finished = true;
            }

            if (targetPlayer == null) return;
            Vector3 lookDirection = targetPlayer.Position - selfPlayer.Position + Vector3.down*0.5f;
            if (lookDirection.sqrMagnitude > 0)
            {
                fpc.FpcModule.MouseLook.LookAtDirection(lookDirection.normalized);
            }
        }
    }
}
