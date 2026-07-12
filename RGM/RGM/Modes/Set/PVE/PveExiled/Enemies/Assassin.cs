using MEC;
using PlayerRoles.FirstPersonControl;
using UnityEngine;
using System.Collections.Generic;
using NetworkManagerUtils.Dummies;
using Exiled.API.Enums;
using CustomPlayerEffects;

namespace RGM.Modes.PveExiledSystem.Enemies
{
    public class Assassin : Enemy
    {
        float fireRate = 0.68f;
        float updateDuration = 0.1f;
        bool canShoot = true;
        float moveBackMinDist = 0.8f;
        float selfMaxHealth;

        float detectRange = 12f;

        CoroutineHandle followootine;
        CoroutineHandle targetSetRootine;
        CoroutineHandle enemyRootine;

        DummyAction? shootAction;

        public Assassin(string enemyName, Vector3 spawnPos, int id, Dictionary<int, Enemy> container, WaveConfig waveConfig) : base(enemyName, spawnPos, id, container, waveConfig)
        {
            range = 1.6f;
            pathCompCheckTime = 0.15f;
            range = range * range;
            moveBackMinDist = moveBackMinDist * moveBackMinDist;
            selfPlayer.Role.Set(PlayerRoles.RoleTypeId.Scp939, SpawnReason.ForceClass, PlayerRoles.RoleSpawnFlags.All);
            foreach(Exiled.API.Features.Player player in Exiled.API.Features.Player.List)
            {
                if (player != null && player.Role.Type == PlayerRoles.RoleTypeId.NtfSpecialist)
                {
                    player.EnableEffect<AmnesiaVision>(0, false);
                }
            }
            selfPlayer.EnableEffect<SpawnProtected>(1, true);
            selfPlayer.MaxHumeShield = 0;
            selfMaxHealth = 200 + waveConfig.MulCount * 10;
            selfPlayer.MaxHealth = 200 + selfMaxHealth;//30명 -> 500HP
            selfPlayer.Health = 200 + selfMaxHealth;
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

            foreach (Exiled.API.Features.Player player in Exiled.API.Features.Player.List)
            {
                if (player != null && player.Role.Type == PlayerRoles.RoleTypeId.NtfSpecialist)
                {
                    player.DisableEffect<AmnesiaVision>();
                }
            }

            base.RemoveEnemy();//인벤클리어&이벤트끊기&리스트제거
        }

        private void LoadAction()
        {
            selfPlayer.Role.Set(PlayerRoles.RoleTypeId.Scp939, SpawnReason.ForceClass, PlayerRoles.RoleSpawnFlags.None);
            selfPlayer.EnableEffect<SpawnProtected>(1, true);
            selfPlayer.MaxHumeShield = 0;
            selfPlayer.MaxHealth = selfMaxHealth;//30명 -> 500HP
            selfPlayer.Health = selfMaxHealth;
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
                    if (lookDirection.sqrMagnitude > detectRange) continue;
                    if (!targetPlayer.IsEffectActive<Scp1344Detected>())
                    {
                        targetPlayer.EnableEffect<Scp1344Detected>(duration: 5, addDurationIfActive: true);
                        targetPlayer.ShowHint("누군가 당신을 노리고 있습니다..", 5);
                    }
                    if (lookDirection.sqrMagnitude > range) continue;

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
