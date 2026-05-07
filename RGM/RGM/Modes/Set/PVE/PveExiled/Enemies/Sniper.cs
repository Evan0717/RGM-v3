using Exiled.API.Features.Items;
using MEC;
using PlayerRoles.FirstPersonControl;
using UnityEngine;
using InventorySystem.Items.Firearms.Modules;
using System.Collections.Generic;
using NetworkManagerUtils.Dummies;
using Exiled.API.Enums;
using CustomPlayerEffects;

namespace RGM.Modes.PveExiledSystem.Enemies
{
    public class Sniper : Enemy
    {
        float fireRate = 10f;
        float aimTime = 5f;
        float updateDuration = 0.1f;
        float moveBackMinDist = 45;

        bool canShoot = true;
        bool aiming = false;
        float aimStartTime = 0;

        CoroutineHandle followootine;
        CoroutineHandle targetSetRootine;
        CoroutineHandle enemyRootine;

        DummyAction? shootAction;
        DummyAction? zoomAction;
        DummyAction? unzoomAction;

        InventorySystem.Items.Firearms.Firearm firearm;
        InventorySystem.Items.Firearms.Modules.MagazineModule magModule;
        public Sniper(string enemyName, Vector3 spawnPos, int id, Dictionary<int, Enemy> container, WaveConfig waveConfig) : base(enemyName, spawnPos, id, container, waveConfig)
        {
            range = 60;
            range = range * range;
            moveBackMinDist = moveBackMinDist * moveBackMinDist;
            aimTime = 7f - waveConfig.Difficulty * 3f;
            selfPlayer.Role.Set(PlayerRoles.RoleTypeId.ChaosRepressor, SpawnReason.ForceClass);
            selfPlayer.EnableEffect<SilentWalk>(200, -1, false);
            selfPlayer.EnableEffect<SpawnProtected>(5, true);
            if (waveConfig.Difficulty == 2) { selfPlayer.EnableEffect<Scp1853>(4, true); fireRate = 7; }
            selfPlayer.ClearInventory();
            selfPlayer.MaxHealth = 200 + waveConfig.MulCount *5;//30명 -> 350HP
            selfPlayer.Health = 200 + waveConfig.MulCount * 5;
            fpc = selfPlayer.RoleManager.CurrentRole as IFpcRole;

            Firearm item = Firearm.Create(FirearmType.ParticleDisruptor);
            item.Give(selfPlayer);
            selfPlayer.Inventory.ServerSelectItem(item.Serial);
            firearm = item.Base;
            firearm.TryGetModule<MagazineModule>(out magModule);

            selfPlayer.Position = spawnPos;
            Timing.CallDelayed(2f, () => {
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
        private void NullActionCheck()
        {
            if (!shootAction.HasValue)
            {
                foreach (DummyAction a in DummyActionCollector.ServerGetActions(hub))
                {
                    if (a.Name.EndsWith("Shoot->Click")) { shootAction = a; break; }
                }
            }
            if (!zoomAction.HasValue)
            {
                foreach (DummyAction a in DummyActionCollector.ServerGetActions(hub))
                {
                    if (a.Name.EndsWith("Zoom->Hold")) { zoomAction = a; break; }
                }
            }
            if (!unzoomAction.HasValue)
            {
                foreach (DummyAction a in DummyActionCollector.ServerGetActions(hub))
                {
                    if (a.Name.EndsWith("Zoom->Click")) { unzoomAction = a; break; }
                }
            }
        }
        private void doUnzoom()
        {
            if (!aiming) return;
            aiming = false;
            unzoomAction.Value.Action();
            selfPlayer.DisableEffect<Slowness>();
            canMove = true;
        }
        private IEnumerator<float> EnemyFunction()
        {
            while (!removed)
            {
                yield return Timing.WaitForSeconds(updateDuration);
                FollowAndLook();
                if (targetPlayer == null || targetPlayer.Role.Type != PlayerRoles.RoleTypeId.NtfSpecialist)
                {
                    doUnzoom();
                    continue;
                }

                Vector3 lookDirection = targetPlayer.Position - selfPlayer.Position;
                if (lookDirection.sqrMagnitude > 0)
                {
                    if (lookDirection.sqrMagnitude > range)
                    {
                        doUnzoom();
                        continue;
                    }
                    bool shootCast = Physics.Raycast(selfPlayer.Position, lookDirection.normalized, out RaycastHit _, maxDistance: lookDirection.magnitude, layerMask: mask, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
                    if (shootCast)
                    {
                        doUnzoom();
                        continue;
                    }
                    if (followEnabled && lookDirection.sqrMagnitude < moveBackMinDist)
                    {
                        followEnabled = false;
                        Timing.CallDelayed(1, () => { if (removed) return; followEnabled = true; });
                    }
                    if (!canShoot) continue;
                    if (!aiming)
                    {
                        canMove = false;
                        selfPlayer.EnableEffect<Slowness>(80, 0, false);
                        aiming = true;
                        aimStartTime = Time.time;
                        NullActionCheck();
                        zoomAction.Value.Action();

                        continue;
                    }
                    else if (Time.time - aimStartTime < aimTime)
                    {
                        if (!targetPlayer.IsEffectActive<Scanned>())
                        {
                            targetPlayer.EnableEffect<Scanned>(duration: 10, addDurationIfActive: true);
                            targetPlayer.ShowHint("소름끼치는 시선이 느껴집니다..", 5);
                        }
                        continue;
                    }

                    canShoot = false;

                    shootAction.Value.Action();
                    magModule.ServerModifyAmmo(5);
                    Timing.CallDelayed(4, () => { if (removed) return; doUnzoom(); });
                    Timing.CallDelayed(fireRate, () => { if (removed) return; canShoot = true; });
                }
            }
        }
    }
}
