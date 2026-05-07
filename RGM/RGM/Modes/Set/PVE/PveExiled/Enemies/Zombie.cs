using MEC;
using PlayerRoles.FirstPersonControl;
using UnityEngine;
using System.Collections.Generic;
using NetworkManagerUtils.Dummies;
using Exiled.API.Enums;
using CustomPlayerEffects;

namespace RGM.Modes.PveExiledSystem.Enemies
{
    public class Zombie : Enemy
    {
        float fireRate = 2f;
        float updateDuration = 0.1f;
        bool canShoot = true;
        float moveBackMinDist = 0.8f;
        float selfMaxhealth;

        CoroutineHandle followootine;
        CoroutineHandle targetSetRootine;
        CoroutineHandle enemyRootine;

        DummyAction? shootAction;

        public Zombie(string enemyName, Vector3 spawnPos, int id, Dictionary<int, Enemy> container, WaveConfig waveConfig) : base(enemyName, spawnPos, id, container, waveConfig)
        {
            range = 1.6f;
            range = range * range;
            moveBackMinDist = moveBackMinDist * moveBackMinDist;
            pathCompCheckTime = 0.15f;
            selfPlayer.IsSpectatable = false;
            selfPlayer.Role.Set(PlayerRoles.RoleTypeId.Scp0492, SpawnReason.ForceClass, PlayerRoles.RoleSpawnFlags.All);
            selfPlayer.EnableEffect<SpawnProtected>(1, true);
            selfMaxhealth = 150 + waveConfig.MulCount * 5;//30명 -> 300HP
            selfPlayer.MaxHealth = selfMaxhealth;//30명 -> 300HP
            selfPlayer.Health = selfMaxhealth;
            fpc = selfPlayer.RoleManager.CurrentRole as IFpcRole;

            selfPlayer.Position = spawnPos;
            Timing.CallDelayed(0.5f, () => {
                if (removed) return;
                LoadAction();
                Timing.CallDelayed(1f, () =>
                {
                    if (removed) return;
                    targetSetRootine = Timing.RunCoroutine(RerollTarget());
                    followootine = Timing.RunCoroutine(FollowLoop());
                    enemyRootine = Timing.RunCoroutine(EnemyFunction());
                });
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

        private void LoadAction()
        {
            selfPlayer.Role.Set(PlayerRoles.RoleTypeId.Scp0492, SpawnReason.ForceClass, PlayerRoles.RoleSpawnFlags.None);
            selfPlayer.EnableEffect<SpawnProtected>(1, true);
            selfPlayer.MaxHealth = selfMaxhealth;
            selfPlayer.Health = selfMaxhealth;
            Timing.CallDelayed(0.5f, () =>
            {
                if (removed) return;
                foreach (var a in DummyActionCollector.ServerGetActions(hub))
                {
                    if (a.Name.EndsWith("Shoot->Click")) { shootAction = a; break; }
                }
            });
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
                        LoadAction();
                        continue;
                    }
                    shootAction.Value.Action();

                    Timing.CallDelayed(fireRate, () => canShoot = true);
                    if (followEnabled && lookDirection.sqrMagnitude < moveBackMinDist)
                    {
                        followEnabled = false;
                        Timing.CallDelayed(0.5f, () => { if (removed) return; followEnabled = true; });
                    }
                }
            }
        }
    }
}
