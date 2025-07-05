using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using ProjectMER.Events.Arguments;
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
    public static class Scp999
    {
        static bool cuteCooldown = false;

        public static void OnEnabled()
        {
            CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(new SetScp999());
        }

        public static Player CreateScp999(Player player)
        {
            player.Role.Set(RoleTypeId.Tutorial, RoleSpawnFlags.AssignInventory);
            player.EnableEffect(EffectType.Ghostly, 1);
            player.EnableEffect(EffectType.SilentWalk, 10);
            player.EnableEffect(EffectType.Asphyxiated, 1);
            player.MaxHealth = 999;
            player.Health = player.MaxHealth;
            Timing.CallDelayed(1, () =>
            {
                player.Scale = new Vector3(0.3f, 0.3f, 0.3f);
            });
            player.AddHint("SCP-999 설명", "<size=25>당신은 <color=red>SCP-999</color>(<color=#a4fc16>Safe</color>)입니다.</size>\n<size=20>모두에게 중립이며, 그들에게 버프를 줄 수 있습니다.</size>", 20);
            SchematicObject schematic = ObjectSpawner.SpawnSchematic("SCP_999", new Vector3(player.Position.x, player.Position.y - 0.1f, player.Position.z), player.Rotation, new Vector3(3, 3, 3));
            schematic.transform.parent = player.Transform;

            IEnumerator<float> main()
            {
                while (true)
                {
                    try
                    {
                        if (Tools.TryGetLookPlayer(player, 15, out Player target, out RaycastHit? hit))
                        {
                            player.DisableEffect(EffectType.Slowness);
                            player.EnableEffect(EffectType.MovementBoost, 20);

                            target.Heal(0.1f);
                        }
                        else
                        {
                            player.DisableEffect(EffectType.MovementBoost);
                            player.EnableEffect(EffectType.Slowness, 20);
                        }
                    }
                    catch
                    {
                    }

                    player.Heal(0.2f);

                    yield return Timing.WaitForOneFrame;
                }
            }

            IEnumerator<float> makeSound()
            {
                while (true)
                {
                    var audio = Tools.PlaySound(schematic.transform, "scp-999-moving");

                    yield return Timing.WaitForSeconds((float)audio.Duration.TotalSeconds);
                }
            }

            IEnumerator<float> cute()
            {
                while (true)
                {
                    cuteCooldown = false;

                    while (!cuteCooldown)
                    {
                        player.AddHint("SCP-999 cute", "<size=20>[ALT]키를 눌러 애교 부리기</size>", 1);

                        yield return Timing.WaitForSeconds(1);
                    }

                    yield return Timing.WaitForSeconds(Random.Range(3, 101));
                }
            }

            var main_c = Timing.RunCoroutine(main());
            var makeSound_c = Timing.RunCoroutine(makeSound());
            var cute_c = Timing.RunCoroutine(cute());

            void OnTogglingNoClip(TogglingNoClipEventArgs ev)
            {
                if (ev.Player != player)
                    return;

                if (!cuteCooldown)
                {
                    schematic.AnimationController.Stop();
                    string name = $"Normal{Random.Range(1, 3)}";
                    schematic.AnimationController.Play(name);
                    Tools.PlaySound(schematic.transform, "scp-999", 2);

                    foreach (var p in Player.List.Where(x => Vector3.Distance(player.Position, x.Position) < 10))
                    {
                        p.AddEffect(EffectType.MovementBoost, 15, 7);
                        p.AddEffect(EffectType.Invigorated, 1, 7);
                    }

                    cuteCooldown = true;
                }
            }

            void OnSearchingPickup(SearchingPickupEventArgs ev)
            {
                if (ev.Player != player)
                    return;

                if (ev.Pickup.Type.IsWeapon())
                {
                    ev.IsAllowed = false;

                    player.AddHint("SCP-999 무기 금지", "<size=20><color=red>SCP-999</color>은(는) 무기를 쥘 수 없습니다.</size>", 3);
                }
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
                            Timing.KillCoroutines(main_c);
                            Timing.KillCoroutines(makeSound_c);
                            Timing.KillCoroutines(cute_c);

                            schematic.Destroy();

                            SchematicObject dead = ObjectSpawner.SpawnSchematic("SCP_999_Dead", new Vector3(pos.x, pos.y - 0.1f, pos.z), player.Rotation);
                            Tools.PlaySound(dead.transform, "scp-999-dead", 2);

                            Exiled.Events.Handlers.Player.SearchingPickup -= OnSearchingPickup;
                            Exiled.Events.Handlers.Player.TogglingNoClip -= OnTogglingNoClip;
                            Exiled.Events.Handlers.Player.Dying -= OnDying;
                        }
                    }
                });
            }

            Exiled.Events.Handlers.Player.SearchingPickup += OnSearchingPickup;
            Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            return player;
        }
    }
}
