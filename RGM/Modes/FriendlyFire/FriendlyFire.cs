using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using UnityEngine;
using MEC;
using HarmonyLib;
using PlayerRoles;

namespace RGM.Modes
{
    class FriendlyFire
    {
        public static FriendlyFire Instance;

        public List<Player> HumanMeleeCooldown = new List<Player>();
        public int Scp106AttackTeamCoolDown = 0;
        public List<Player> Scp106Stacks = new List<Player>();

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;

            Exiled.Events.Handlers.Scp173.Blinking += OnBlinking;

            Exiled.Events.Handlers.Scp939.Lunging += OnLunging;

            Harmony harmony = new Harmony($"FriendlyFire - {DateTime.Now.Ticks}");
            harmony.Patch(AccessTools.Method(typeof(HitboxIdentity), nameof(HitboxIdentity.IsEnemy), [typeof(Team), typeof(Team)]), 
                postfix: new HarmonyMethod(AccessTools.Method(typeof(HitboxPatchPostfix), nameof(HitboxPatchPostfix.Postfix))));
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(0.5f);

            foreach (var player in Player.List)
            {
                Spawned(player);
            }
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            if (new List<RoleTypeId>() { RoleTypeId.Scp173, RoleTypeId.Scp106 }.Contains(player.Role.Type) || player.IsHuman)
                player.ShowHint($"<size=20><b><i>tip.</i></b> [ALT] 키를 눌러 같은 진영에게 피해를 입힐 수 있습니다.</size>", 10);

            else if (player.Role.Type == RoleTypeId.Scp939)
                player.ShowHint($"<size=20><b><i>tip.</i></b> 런지를 사용하는 도중 근접한 SCP를 쳐다보면 해당 개체에 피해를 입힐 수 있습니다.</size>", 10);
        }

        public async void OnTogglingNoClip(Exiled.Events.EventArgs.Player.TogglingNoClipEventArgs ev)
        {
            if (ev.Player.IsHuman && !ev.Player.IsCuffed)
            {
                if (Physics.Raycast(ev.Player.ReferenceHub.PlayerCameraReference.position + ev.Player.ReferenceHub.PlayerCameraReference.forward * 0.2f, ev.Player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 2f, InventorySystem.Items.Firearms.Modules.StandardHitregBase.HitregMask) &&
                    hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
                {
                    var player = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>());

                    if (ev.Player != player && !HumanMeleeCooldown.Contains(ev.Player))
                    {
                        Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 0.7f);
                        player.Hurt(12.05f, "무지성으로 구타당해 죽었습니다.");

                        HumanMeleeCooldown.Add(ev.Player);
                        await Task.Delay(1000);
                        HumanMeleeCooldown.Remove(ev.Player);
                    }
                }
            }

            else if (ev.Player.Role.Type == RoleTypeId.Scp173)
            {
                if (Physics.Raycast(ev.Player.ReferenceHub.PlayerCameraReference.position + ev.Player.ReferenceHub.PlayerCameraReference.forward * 0.2f, ev.Player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 2f, InventorySystem.Items.Firearms.Modules.StandardHitregBase.HitregMask) &&
                    hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
                {
                    if (ev.Player.Role is Exiled.API.Features.Roles.Scp173Role scp173)
                    {
                        if (scp173.BlinkCooldown == 0f)
                        {
                            var player = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>());

                            if (ev.Player != player && player.IsScp)
                            {
                                Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 1.2f);
                                Server.ExecuteCommand($"/cassie <color=#ff0000>{ev.Player.Role.Name}</color>이(가) <color=#ff0000>{player.Role.Name}</color>의 뒷통수를 쳤습니다.");
                                player.Hurt(-1, Exiled.API.Enums.DamageType.Scp173);
                            }
                        }
                    }
                }
            }

            else if (ev.Player.Role.Type == RoleTypeId.Scp106)
            {
                if (Physics.Raycast(ev.Player.ReferenceHub.PlayerCameraReference.position + ev.Player.ReferenceHub.PlayerCameraReference.forward * 0.2f, ev.Player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 2f, InventorySystem.Items.Firearms.Modules.StandardHitregBase.HitregMask) &&
                    hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
                {
                    var player = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>());

                    if (ev.Player != player && player.IsScp && Scp106AttackTeamCoolDown <= 0)
                    {
                        Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 0.9f);
                        player.Hurt(40, Exiled.API.Enums.DamageType.Scp106);
                        Scp106AttackTeamCoolDown = 1;

                        if (Scp106Stacks.Contains(player))
                        {
                            player.EnableEffect(Exiled.API.Enums.EffectType.PocketCorroding);

                            Scp106AttackTeamCoolDown = 0;
                        }
                        else
                        {
                            Scp106Stacks.Add(player);
                            await Task.Delay(1000);
                            Scp106AttackTeamCoolDown = 0;
                            await Task.Delay(9000);
                            Scp106Stacks.Remove(player);
                        }
                    }
                }
            }
        }

        public async void OnBlinking(Exiled.Events.EventArgs.Scp173.BlinkingEventArgs ev)
        {
            await Task.Delay(20);

            if (Physics.Raycast(ev.Player.ReferenceHub.PlayerCameraReference.position + ev.Player.ReferenceHub.PlayerCameraReference.forward * 0.2f, ev.Player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 2f, InventorySystem.Items.Firearms.Modules.StandardHitregBase.HitregMask) &&
            hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
            {
                var player = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>());

                if (ev.Player != player && player.IsScp)
                {
                    Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 1.5f);
                    Server.ExecuteCommand($"/cassie {ev.Player.Role.Name}이(가) {player.Role.Name}의 뒷통수를 쳤습니다.");
                    player.Hurt(-1, Exiled.API.Enums.DamageType.Scp173);
                }
            }
        }

        public async void OnLunging(Exiled.Events.EventArgs.Scp939.LungingEventArgs ev)
        {
            for (float i = 0; i < 0.7f; i += 0.01f)
            {
                if (Physics.Raycast(ev.Player.ReferenceHub.PlayerCameraReference.position + ev.Player.ReferenceHub.PlayerCameraReference.forward * 0.2f, ev.Player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 2f, InventorySystem.Items.Firearms.Modules.StandardHitregBase.HitregMask) &&
                hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
                {
                    var player = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>());

                    if (ev.Player != player && player.IsScp)
                    {
                        Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 1.5f);
                        player.Hurt(125, Exiled.API.Enums.DamageType.Scp939);

                        if (ev.Player.IsDead)
                            Server.ExecuteCommand($"/cassie {ev.Player.Role.Name}이(가) {player.Role.Name}의 뒷통수를 쳤습니다.");
                    }
                }

                await Task.Delay(10);
            }
        }

        public class HitboxPatchPostfix
        {
            public static void Postfix(ref bool __result)
            {
                __result = true;
            }
        }
    }
}
