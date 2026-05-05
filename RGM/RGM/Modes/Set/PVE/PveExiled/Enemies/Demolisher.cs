using MEC;
using PlayerRoles.FirstPersonControl;
using UnityEngine;
using InventorySystem;
using InventorySystem.Items;
using System.Collections.Generic;
using NetworkManagerUtils.Dummies;
using Exiled.API.Enums;
using CustomPlayerEffects;

namespace RGM.Modes.PveExiledSystem.Enemies
{
    public class Demolisher : Enemy
    {
        float raycastIgnoreRange = 8;
        float updateDuration = 0.1f;
        bool canUse = true;
        float moveBackMinDist = 4f;

        CoroutineHandle followootine;
        CoroutineHandle targetSetRootine;
        CoroutineHandle enemyRootine;

        DummyAction? useAction;
        public Demolisher(string enemyName, Vector3 spawnPos, int id, Dictionary<int, Enemy> container, WaveConfig waveConfig) : base(enemyName, spawnPos, id, container, waveConfig)
        {
            range = 10;
            range = range*range;
            moveBackMinDist = moveBackMinDist * moveBackMinDist;
            selfPlayer.Role.Set(PlayerRoles.RoleTypeId.ClassD, SpawnReason.ForceClass);
            selfPlayer.EnableEffect<MovementBoost>(35, -1, false);
            selfPlayer.EnableEffect<Scp207>(1, -1, false);
            selfPlayer.ClearInventory();
            selfPlayer.MaxHealth = 100 + waveConfig.MulCount* 5;//30명 -> 250HP
            selfPlayer.Health = 100 + waveConfig.MulCount * 5;
            fpc = selfPlayer.RoleManager.CurrentRole as IFpcRole;

            pathCompCheckTime = 0.2f;

            selfPlayer.Inventory.ServerAddItem(ItemType.ArmorHeavy, ItemAddReason.AdminCommand);
            ItemBase item = selfPlayer.Inventory.ServerAddItem(ItemType.AntiSCP207, ItemAddReason.AdminCommand);
            selfPlayer.Inventory.ServerSelectItem(item.ItemSerial);

            selfPlayer.Position = spawnPos;
            Timing.CallDelayed(0.5f, () => {
                foreach (DummyAction a in DummyActionCollector.ServerGetActions(hub))
                {
                    if (a.Name.EndsWith("UsableItem->Start")) { useAction = a; break; }
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

                if (!canUse) continue;
                if (targetPlayer == null) continue;

                Vector3 lookDirection = targetPlayer.Position - selfPlayer.Position;
                if (lookDirection.sqrMagnitude > 0)
                {
                    if (lookDirection.sqrMagnitude > range) continue;
                    bool shootCast = Physics.Raycast(selfPlayer.Position, lookDirection.normalized, out RaycastHit _, maxDistance: lookDirection.magnitude, layerMask: mask, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
                    if (shootCast && lookDirection.magnitude > raycastIgnoreRange) continue;

                    if (!useAction.HasValue)
                    {
                        foreach (DummyAction a in DummyActionCollector.ServerGetActions(hub))
                        {
                            if (a.Name.EndsWith("UsableItem->Start")) { useAction = a; break; }
                        }
                        continue;
                    }
                    useAction.Value.Action();
                    Timing.CallDelayed(5, () => { if (removed) return; selfPlayer.EnableEffect<CustomPlayerEffects.PitDeath>(1); });
                    canUse = false;
                }
            }
        }
    }
}
