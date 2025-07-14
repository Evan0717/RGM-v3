using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using ProjectMER.Commands.Modifying.Position;
using ProjectMER.Events.Arguments;
using ProjectMER.Events.Handlers;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using RemoteAdmin;
using RGM.API.Components;
using RGM.API.Features;
using RGM.Modes.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static RGM.Variables.ServerManagers;

namespace RGM.Modes.Sets.AddScp.Scps
{
    public static class Scp457
    {
        static bool abilityCooldown = false;

        public static void OnEnabled()
        {
            CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(new SetScp457());
        }

        public static Player Create(Player player)
        {
            player.Role.Set(RoleTypeId.Scp0492, RoleSpawnFlags.AssignInventory);
            player.EnableEffect(EffectType.SilentWalk, 10);
            player.EnableEffect(EffectType.Slowness, 10);
            player.MaxHealth = 1500;
            player.Health = player.MaxHealth;
            Timing.CallDelayed(1, () =>
            {
                player.Scale = new Vector3(0.3f, 0.3f, 0.3f);
            });
            player.AddHint("SCP-457 설명", "<size=25>당신은 <color=red>SCP-457</color>(<color=#f4fe48>Euclid</color>/<color=#fe4848>Potential Keter</color>)입니다.</size>\n<size=20>진영은 <color=red>SCP</color> 소속입니다.</size>", 20);
            SchematicObject schematic = ObjectSpawner.SpawnSchematic("SCP_457", player.Position, player.Rotation, new Vector3(0.4f, 0.4f, 0.4f));
            schematic.transform.parent = player.Transform;

            IEnumerator<float> main()
            {
                while (true)
                {
                    foreach (var pickup in Pickup.List)
                    {
                        if (Vector3.Distance(player.Position, pickup.Position) < 3)
                        {
                            if (player.Scale.x < 1.15f)
                                player.Scale += new Vector3(0.002f * pickup.Weight, 0.002f * pickup.Weight, 0.002f * pickup.Weight);
                            player.MaxHealth += pickup.Weight * 1.2f;
                            player.Health += pickup.Weight * 1.2f;

                            pickup.Destroy();
                        }
                    }

                    yield return Timing.WaitForSeconds(1);
                }
            }

            IEnumerator<float> makeSound()
            {
                while (true)
                {
                    var audio = Tools.PlaySound(player.Transform, "scp-457-burn");

                    yield return Timing.WaitForSeconds((float)audio.Duration.TotalSeconds);
                }
            }

            IEnumerator<float> burn2()
            {
                while (true)
                {
                    foreach (var p in Player.List.Where(x => !x.IsScp && x != player && Vector3.Distance(x.Position, player.Position) < 4))
                    {
                        p.EnableEffect(EffectType.Burned, 1, 0.1f);
                        p.Hit(player, player.Scale.x * 10);
                    }

                    yield return Timing.WaitForSeconds(0.1f);
                }
            }

            IEnumerator<float> attack()
            {
                while (true)
                {
                    abilityCooldown = false;

                    while (!abilityCooldown)
                    {
                        player.AddHint("SCP-457 공격 가능", "<size=20>[ALT]키를 눌러 원거리 공격</size>", 1);

                        yield return Timing.WaitForSeconds(1);
                    }

                    yield return Timing.WaitForSeconds(60);
                }
            }

            var main_c = Timing.RunCoroutine(main());
            var makeSound_c = Timing.RunCoroutine(makeSound());
            var burn2_c = Timing.RunCoroutine(burn2());
            var attack_c = Timing.RunCoroutine(attack());

            void OnHurting(HurtingEventArgs ev)
            {
                if (ev.Player != player || ev.Attacker == null)
                    return;

                if (ev.Attacker.IsScp)
                {
                    ev.IsAllowed = false;
                }
            }

            IEnumerator<float> OnTogglingNoClip(TogglingNoClipEventArgs ev)
            {
                if (ev.Player != player || abilityCooldown)
                    yield break;

                Throwable throwable = ev.Player.ThrowGrenade(ProjectileType.FragGrenade);

                abilityCooldown = true;

                yield return Timing.WaitForSeconds(0.3f);

                if (throwable.Projectile is ExplosionGrenadeProjectile grenade)
                {
                    while (!grenade.IsAlreadyDetonated)
                    {
                        if (Physics.OverlapSphere(grenade.Position, 0.3f).Count() > 4)
                        {
                            grenade.Base.Network_syncTargetTime = 0.1f;

                            Vector3 pos = grenade.Position;

                            IEnumerator<float> lava()
                            {
                                for (int i = 0; i < 10; i++)
                                {
                                    foreach (var p in Player.List.Where(x => !x.IsScp && x != player && Vector3.Distance(x.Position, pos) < 4))
                                    {
                                        p.EnableEffect(EffectType.Burned, 1, 0.1f);
                                        p.Hit(player, player.Scale.x * 10);
                                    }

                                    yield return Timing.WaitForSeconds(1);
                                }
                            }

                            Timing.RunCoroutine(lava());
                        }

                        yield return Timing.WaitForOneFrame;
                    }
                }
            }

            void OnDied(DiedEventArgs ev)
            {
                if (ev.Attacker == null || ev.Attacker != player)
                    return;

                if (ev.Player.Scale.x < 1.15f)
                    ev.Attacker.Scale += new Vector3(0.03f, 0.03f, 0.03f);
                ev.Attacker.MaxHealth += 25;
                ev.Attacker.Health += 25;
            }

            void OnDying(DyingEventArgs ev)
            {
                if (ev.Player != player)
                    return;

                Vector3 pos = ev.Player.Position;

                Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                {
                    if (ev.Player.IsDead)
                    {
                        if (ev.Player == player)
                        {
                            var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, player);
                            g.FuseTime = 0.1f;
                            g.SpawnActive(pos, player);

                            Timing.KillCoroutines(main_c);
                            Timing.KillCoroutines(makeSound_c);
                            Timing.KillCoroutines(burn2_c);
                            Timing.KillCoroutines(attack_c);

                            schematic.Destroy();

                            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
                            Exiled.Events.Handlers.Player.TogglingNoClip -= OnTogglingNoClip;
                            Exiled.Events.Handlers.Player.Died -= OnDied;
                            Exiled.Events.Handlers.Player.Dying -= OnDying;
                        }
                    }
                });
            }

            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;
            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            return player;
        }
    }
}
