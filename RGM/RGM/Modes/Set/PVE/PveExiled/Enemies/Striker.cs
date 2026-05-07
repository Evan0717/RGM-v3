using MEC;
using PlayerRoles.FirstPersonControl;
using UnityEngine;
using InventorySystem;
using InventorySystem.Items;
using System.Collections.Generic;
using Exiled.API.Enums;
using CustomPlayerEffects;
using PlayerRoles;
using Exiled.API.Features.Toys;

namespace RGM.Modes.PveExiledSystem.Enemies
{
    public class Striker : Enemy
    {
        float fireRate = 0.6f;
        float updateDuration = 0.1f;
        bool canShoot = true;
        float moveBackMinDist = 8f;

        float Damage = 5;
        int ChainCount = 5;
        float ChainDamage = 10;
        float ChainRangeSqr = 25;

        AudioPlayer audioPlayer;

        CoroutineHandle followootine;
        CoroutineHandle targetSetRootine;
        CoroutineHandle enemyRootine;

        List<Exiled.API.Features.Toys.Primitive> primitives = new List<Exiled.API.Features.Toys.Primitive>();

        public Striker(string enemyName, Vector3 spawnPos, int id, Dictionary<int, Enemy> container, WaveConfig waveConfig) : base(enemyName, spawnPos, id, container, waveConfig)
        {
            Damage = 6 + waveConfig.Difficulty * 2;
            ChainDamage = 5 + waveConfig.Difficulty;
            ChainCount = 4 + waveConfig.Difficulty;
            range = 15;
            range = range * range;
            moveBackMinDist = moveBackMinDist * moveBackMinDist;
            selfPlayer.Role.Set(PlayerRoles.RoleTypeId.ChaosRepressor, SpawnReason.ForceClass);
            selfPlayer.EnableEffect<MovementBoost>(30, -1, false);
            selfPlayer.EnableEffect<SpawnProtected>(3, true);
            selfPlayer.ClearInventory();
            selfPlayer.MaxHealth = 300 + waveConfig.MulCount*10;//30명 -> 600HP
            selfPlayer.Health = 300 + waveConfig.MulCount * 10;
            fpc = selfPlayer.RoleManager.CurrentRole as IFpcRole;

            audioPlayer = AudioPlayer.CreateOrGet($"Enemy {selfPlayer.Id}", onIntialCreation: (p) =>
            {
                p.transform.parent = selfPlayer.GameObject.transform;
                Speaker speaker = p.AddSpeaker("Main", isSpatial: true, minDistance: 5f, maxDistance: 20f);
                speaker.transform.parent = selfPlayer.GameObject.transform;
                speaker.transform.localPosition = Vector3.zero;
            });

            ItemBase item = selfPlayer.Inventory.ServerAddItem(ItemType.MicroHID, ItemAddReason.AdminCommand);
            selfPlayer.Inventory.ServerSelectItem(item.ItemSerial);

            selfPlayer.Position = spawnPos;
            Timing.CallDelayed(0.5f, () => {
                if (removed) return;
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

            if (audioPlayer != null)
            {
                audioPlayer.RemoveAllClips();
                audioPlayer.RemoveSpeaker("Main");
                audioPlayer.Destroy();
            }//destroyAudioplayer

            foreach(Primitive primitive in primitives)
            {
                if (primitive == null) continue;
                primitive.Destroy();
            }
            primitives.Clear();

            base.RemoveEnemy();//인벤클리어&이벤트끊기&리스트제거
        }

        private void ConnectUnitChain(Vector3 pos1, Vector3 pos2)
        {
            Vector3 gap = (pos1 - pos2);
            if (gap.sqrMagnitude == 0) return;
            Primitive primitive = Primitive.Create(primitiveType: PrimitiveType.Cube, position: (pos1 + pos2) / 2, scale: new Vector3(0.05f, 0.05f, gap.magnitude), spawn: false);
            primitives.Add(primitive);
            primitive.Collidable = false;
            primitive.Color = new Color(1, 0.25f, 0.25f, -10);
            primitive.Rotation = Quaternion.LookRotation(gap);
            primitive.Spawn();
            Timing.CallDelayed(0.2f, () =>
            {
                if (removed) return;
                primitives.Remove(primitive);
                primitive.Destroy();
            });
        }

        private void MakeChain(Vector3 pos1, Vector3 pos2)
        {
            Vector3 positionGap = (pos2 - pos1);
            if (positionGap.sqrMagnitude == 0) return;
            float magnitude = positionGap.magnitude;

            int madiCount = (int)(positionGap.magnitude);
            if (madiCount <= 1)
            {
                ConnectUnitChain(pos1, pos2);
            }
            else
            {
                Vector3 unit = positionGap.normalized;
                float madiLength = magnitude / (float)madiCount;
                Vector3 origin = pos1;
                Vector3 target;
                for (int i = 1; i <= madiCount; i++)
                {
                    target = pos1 + unit * i * madiLength + new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f );
                    ConnectUnitChain(origin, target);
                    origin = target;
                }
                ConnectUnitChain(origin, pos2);

            }
        }

        private void Attack(List<Exiled.API.Features.Player> players)
        {
            audioPlayer.AddClip("Seige_Striker1");
            audioPlayer.AddClip("Seige_Striker2");

            MakeChain(selfPlayer.Position, players[0].Position);
            for (int i = 1; i < players.Count; i++)
            {
                MakeChain(players[i - 1].Position, players[i].Position);
            }
            players[0].Hurt(amount: Damage, damageType: DamageType.Tesla, attacker: selfPlayer);
            for (int i=1; i<players.Count; i++) players[i].Hurt(amount: ChainDamage, damageType: DamageType.Tesla, attacker: selfPlayer);
        }

        private void ChainPlayers(Exiled.API.Features.Player Originplayer)
        {
            List<Exiled.API.Features.Player > players = new List<Exiled.API.Features.Player>() { Originplayer };
            for (int i=0; i<ChainCount; i++)
            {
                foreach (Exiled.API.Features.Player player in Exiled.API.Features.Player.List)
                {
                    if (!IsAttackablePlayer(player) || players.Contains(player)) continue;
                    Vector3 lookDirection = (players[players.Count - 1].Position - player.Position);
                    if (lookDirection.sqrMagnitude > ChainRangeSqr) continue;
                    if (lookDirection.sqrMagnitude == 0) { players.Add(player); break; }

                    if (player.CurrentRoom == players[players.Count-1].CurrentRoom || !Physics.Raycast(selfPlayer.Position, lookDirection.normalized, out RaycastHit _, maxDistance: lookDirection.magnitude, layerMask: mask, queryTriggerInteraction: QueryTriggerInteraction.Ignore))
                    {
                        players.Add(player);
                        break;
                    }
                }
                if (players.Count-1 <= i) break;
            }
            Attack(players);
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
                    ChainPlayers(targetPlayer);

                    Timing.CallDelayed(fireRate, () => canShoot = true);
                    if (followEnabled && lookDirection.sqrMagnitude < moveBackMinDist)
                    {
                        followEnabled = false;
                        Timing.CallDelayed(1, () => { if (removed) return; followEnabled = true; });
                    }
                }
            }
        }

        protected bool IsAttackablePlayer(Exiled.API.Features.Player player)
        {
            if (player == null) return false;
            if (player.UserId == "ID_Dedicated" || player.UserId == "ID_Dummy" || player.IsNPC)
            {
                if (player.Nickname != "Tester") return false;
            }
            if (player.Role.Type != RoleTypeId.NtfSpecialist) return false;
            return true;
        }
    }
}
