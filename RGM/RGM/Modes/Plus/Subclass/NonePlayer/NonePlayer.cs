using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp096;
using Exiled.Events.EventArgs.Scp173;
using HarmonyLib;
using MEC;
using Mirror;

using PlayerRoles;
using PlayerStatsSystem;
using RGM.API.Features;
using RGM.UserSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static RGM.Variables.Variable;

namespace RGM.Modes.SubClass
{
    public static class NonePlayer
    {
        public static List<Player> Players = new();

        public static void Create(Player player)
        {
            var black = new List<ItemType>
            {
                ItemType.SCP1509,
                ItemType.SCP1507Tape,
                ItemType.SCP244a,
                ItemType.SCP244b,
                ItemType.SCP018,
                ItemType.SpecialCoal,
                ItemType.SCP1507Tape
            };

            IEnumerator<float> main()
            {
                if (!Players.Contains(player))
                    Players.Add(player);

                player.Role.Set(RoleTypeId.Tutorial);
                player.Position = new Vector3(76.74454f, 27.96138f, 12.53338f);
                player.AddItem(Tools.EnumToList<ItemType>().GetRandomValue(x => x.IsWeapon() && !black.Contains(x)));
                player.AddItem(Tools.EnumToList<ItemType>().GetRandomValue(x => !black.Contains(x)));

                GodModePlayers.Add(player);

                Timing.CallDelayed(3, () =>
                {
                    GodModePlayers.Remove(player);
                });

                yield break;
            }

            var main_c = Timing.RunCoroutine(main());

            void OnPlacingBulletHole(PlacingBulletHoleEventArgs ev)
            {
                if (ev.Player == player)
                    ev.IsAllowed = false;
            }

            void OnDying(DyingEventArgs ev)
            {
                if (ev.Player == player)
                    ev.Player.ClearInventory();
            }

            void OnDied(DiedEventArgs ev)
            {
                if (ev.Player == player)
                    ev.Ragdoll.Destroy();
            }

            void OnShot(ShotEventArgs ev)
            {
                if (ev.Player == player && ev.Firearm.AmmoType != AmmoType.None)
                    ev.Player.AddAmmo(ev.Firearm.AmmoType, 1);
            }

            void OnDroppingItem(DroppingItemEventArgs ev)
            {
                if (ev.Player == player)
                    ev.IsAllowed = false;
            }

            void OnDroppingAmmo(DroppingAmmoEventArgs ev)
            {
                if (ev.Player == player)
                    ev.IsAllowed = false;
            }

            void OnChangingRole(ChangingRoleEventArgs ev)
            {
                if (ev.Player == player)
                {
                    ev.Player.ClearInventory();

                    if (Players.Contains(player))
                        Players.Remove(player);

                    Exiled.Events.Handlers.Map.PlacingBulletHole -= OnPlacingBulletHole;

                    Exiled.Events.Handlers.Player.Dying -= OnDying;
                    Exiled.Events.Handlers.Player.Died -= OnDied;
                    Exiled.Events.Handlers.Player.Shot -= OnShot;
                    Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;
                    Exiled.Events.Handlers.Player.DroppingAmmo -= OnDroppingAmmo;
                    Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
                }
            }

            Exiled.Events.Handlers.Map.PlacingBulletHole += OnPlacingBulletHole;

            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.Shot += OnShot;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.DroppingAmmo += OnDroppingAmmo;
            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
        }
    }
}
