using Discord;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using Exiled.API.Features.Items;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Thirdperson;
using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers.OverlayAnims;
using PlayerStatsSystem;
using RGM.API.Interfaces;
using RGM.Modes.SubClass;
using RGM.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Windows;
using UserSettings.ServerSpecific;
using static RGM.Variables.Variable;

namespace RGM.API.Features
{
    public static class PlayerManager
    {
        public static List<Player> List
        {
            get => Player.List.Where(x => !NonePlayer.Players.Contains(x)).ToList();
        }

        public static void Hit(this Player player, Player attacker, float damage)
        {
            attacker.ShowHitMarker(damage / 10);
            player.Hurt(new DisruptorDamageHandler(new InventorySystem.Items.Firearms.ShotEvents.DisruptorShotEvent(InventorySystem.Items.ItemIdentifier.None, attacker.Footprint, InventorySystem.Items.Firearms.Modules.DisruptorActionModule.FiringState.FiringRapid), player.Position, damage));
        }

        public static void AddCustomKeycard(this Player player, string info)
        {
            Server.ExecuteCommand($"/ckeycard {player.Id} {info}");

            Server.ExecuteCommand($"/ckeycard 1 KeycardCustomSite02 커스텀_키카드 0 0 0 #56C491 #C1CE79 jumpscare-Scp939 #891064 AudioClips 100");
        }

        public static bool AddBadge(this string userId, string args, out string response, ArraySegment<string>? arguments = null)
        {
            if (arguments.HasValue && arguments.Value.Count < 2)
            {
                response = "칭호추가 <player> <badge name>";
                return false;
            }
            else if (Badges.ContainsKey(args))
            {
                List<string> uc = UsersManager.UsersCache[userId];

                if (uc[10] == "0")
                {
                    uc[10] = args;
                    UsersManager.UsersCache[userId] = uc;
                    response = "Successfully add badge.";

                    UsersManager.SaveUsers();
                    return true;
                }
                else
                {
                    if (uc[10].Split('/').Contains(args))
                    {
                        response = "This player already have this badge.";
                        return false;
                    }
                    else
                    {
                        uc[10] += $"/{args}";
                        UsersManager.UsersCache[userId] = uc;
                        response = "Successfully add badge.";

                        UsersManager.SaveUsers();
                        return true;
                    }
                }
            }
            else
            {
                response = "This badge is not exist.";
                return false;
            }
        }

        public static bool AddCustom(this string userId, string args, out string response, ArraySegment<string>? arguments = null)
        {
            if (arguments.HasValue && arguments.Value.Count < 2)
            {
                response = "커스텀추가 <player> <custom feature name>";
                return false;
            }
            else if (Customizations.ContainsKey(args))
            {
                List<string> uc = UsersManager.UsersCache[userId];

                if (uc[7] == "0")
                {
                    uc[7] = args;
                    UsersManager.UsersCache[userId] = uc;
                    response = "Successfully add custom feature.";

                    UsersManager.SaveUsers();
                    return true;
                }
                else
                {
                    if (uc[7].Split('/').Contains(args))
                    {
                        response = "This player already have this custom feature.";
                        return false;
                    }
                    else
                    {
                        uc[7] += $"/{args}";
                        UsersManager.UsersCache[userId] = uc;
                        response = "Successfully add custom feature.";

                        UsersManager.SaveUsers();
                        return true;
                    }
                }
            }
            else
            {
                response = "This custom feature is not exist.";
                return false;
            }
        }

        public static bool AddKillEffect(this string userId, string args, out string response, ArraySegment<string>? arguments = null)
        {
            if (arguments.HasValue && arguments.Value.Count < 2)
            {
                response = "킬이펙트추가 <player> <kill effect name>";
                return false;
            }
            else if (KillEffects.ContainsKey(args))
            {
                List<string> uc = UsersManager.UsersCache[userId];

                if (uc[3] == "0")
                {
                    uc[3] = args;
                    UsersManager.UsersCache[userId] = uc;
                    response = "Successfully add kill effect.";

                    UsersManager.SaveUsers();
                    return true;
                }
                else
                {
                    if (uc[3].Split('/').Contains(args))
                    {
                        response = "This player already have this kill effect.";
                        return false;
                    }
                    else
                    {
                        uc[3] += $"/{args}";
                        UsersManager.UsersCache[userId] = uc;
                        response = "Successfully add kill effect.";

                        UsersManager.SaveUsers();
                        return true;
                    }
                }
            }
            else
            {
                response = "This kill effect is not exist.";
                return false;
            }
        }

        public static bool AddPaint(this string userId, string args, out string response, ArraySegment<string>? arguments = null)
        {
            if (arguments.HasValue && arguments.Value.Count < 2)
            {
                response = "페인트추가 <player> <paint name>";
                return false;
            }
            else if (Paints.ContainsKey(args))
            {
                List<string> uc = UsersManager.UsersCache[userId];

                if (uc[8] == "0")
                {
                    uc[8] = args;
                    UsersManager.UsersCache[userId] = uc;
                    response = "Successfully add paint.";

                    UsersManager.SaveUsers();
                    return true;
                }
                else
                {
                    if (uc[8].Split('/').Contains(args))
                    {
                        response = "This player already have this paint.";
                        return false;
                    }
                    else
                    {
                        uc[8] += $"/{args}";
                        UsersManager.UsersCache[userId] = uc;
                        response = "Successfully add paint.";

                        UsersManager.SaveUsers();
                        return true;
                    }
                }
            }
            else
            {
                response = "This paint is not exist.";
                return false;
            }
        }

        public static bool SetCash(this string userId, int cash, out string response, bool result = true)
        {
            List<string> uc = UsersManager.UsersCache[userId];

            if (result)
            {
                if (cash < 0)
                {
                    response = "0 upper";
                    return false;
                }
                else
                {
                    uc[2] = cash.ToString();
                    UsersManager.UsersCache[userId] = uc;
                    response = "successfully set up Cash.";

                    UsersManager.SaveUsers();
                    return true;
                }
            }
            else
            {
                response = $"{uc[2]}";
                return true;
            }
        }

        public static bool SetRC(this string userId, int rc, out string response, bool result = true)
        {
            List<string> uc = UsersManager.UsersCache[userId];

            if (result)
            {
                if (rc < 0)
                {
                    response = "0 upper.";
                    return false;
                }
                else
                {
                    uc[1] = rc.ToString();
                    UsersManager.UsersCache[userId] = uc;
                    response = "successfully set up Random Coin.";

                    UsersManager.SaveUsers();
                    return true;
                }
            }
            else
            {
                response = $"{uc[1]}";
                return false;
            }
        }

        public static bool AddProduct(this string userId, string args, out string response, ArraySegment<string>? arguments = null)
        {
            if (arguments.HasValue && arguments.Value.Count < 2)
            {
                response = "아이템추가 <player> <item name>";
                return false;
            }
            else if (Products.Select(x => x.Name).Contains(args))
            {
                List<string> uc = UsersManager.UsersCache[userId];

                if (uc[18] == "0")
                {
                    uc[18] = args;
                    UsersManager.UsersCache[userId] = uc;
                    response = "Successfully add item.";

                    UsersManager.SaveUsers();
                    return true;
                }
                else
                {
                    uc[18] += $"/{args}";
                    UsersManager.UsersCache[userId] = uc;
                    response = "Successfully add item.";

                    UsersManager.SaveUsers();
                    return true;
                }
            }
            else
            {
                response = "This item is not exist.";
                return false;
            }
        }

        public static bool AddWarn(this string userId, string args, out string response, ArraySegment<string>? arguments = null)
        {
            List<string> uc = UsersManager.UsersCache[userId];

            if (args == "")
            {
                response = $"{string.Join("\n", uc[22].Split('/'))}\n-";
                return true;
            }
            else
            {
                if (uc[22] == "0")
                {
                    uc[22] = args;
                    UsersManager.UsersCache[userId] = uc;
                    response = $"Successfully add warn.\n\n{string.Join("\n", uc[22].Split('/'))}\n-";

                    UsersManager.SaveUsers();
                    return true;
                }
                else
                {
                    uc[22] += $"/{args}";
                    UsersManager.UsersCache[userId] = uc;
                    response = $"Successfully add warn.\n\n{string.Join("\n", uc[22].Split('/'))}\n-";

                    UsersManager.SaveUsers();
                    return true;
                }
            }
        }

        public static void AddCandy(this Player player, CandyKindID candyKindID)
        {
            void add()
            {
                Scp330 scp330 = (Scp330)Item.Create(ItemType.SCP330);
                scp330.AddCandy(candyKindID);
                scp330.RemoveCandy(scp330.Candies.ToList()[0]);
                player.AddItem(scp330);
            }

            if (player.HasItem(ItemType.SCP330))
            {
                bool success = false;

                foreach (var scp330 in player.Items.Where(x => x.Type == ItemType.SCP330).Select(x => (Scp330)x))
                {
                    if (scp330.Candies.Count() == 6)
                    {
                        continue;
                    }
                    else
                    {
                        scp330.AddCandy(candyKindID);
                        success = true;
                    }
                }

                if (!success)
                {
                    add();
                }
            }
            else
            {
                add();
            }
        }

        public static void Push(this Player player, Player target, float distance = 5, float height = 1)
        {
            Vector3 horizontalDirection = target.Position - player.Position;
            horizontalDirection.y = 0;
            horizontalDirection = horizontalDirection.normalized;

            Vector3 throwVector = horizontalDirection * distance;
            throwVector.y = height;

            RaycastHit[] hits = Physics.RaycastAll(
                player.ReferenceHub.PlayerCameraReference.position + player.ReferenceHub.PlayerCameraReference.forward * 0.2f,
                player.ReferenceHub.PlayerCameraReference.forward,
                distance
            );

            RaycastHit? validHit = null;
            foreach (var h in hits.OrderBy(hit => hit.distance))
            {
                if (!h.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
                {
                    validHit = h;
                    break;
                }
            }

            if (validHit.HasValue)
            {
                target.Position = validHit.Value.point;
            }
            else
            {
                target.Position = player.Position + throwVector;
            }
        }

        public static void ApplyGodMode(this Player player, float time)
        {
            GodModePlayers.Add(player);

            Timing.CallDelayed(time, () =>
            {
                if (GodModePlayers.Contains(player))
                    GodModePlayers.Remove(player);
            });
        }

        public static void Grab(this Player player)
        {
            OverlayAnimationsSubcontroller subcontroller;
            if (!(player.ReferenceHub.roleManager.CurrentRole is IFpcRole currentRole) ||
                !(currentRole.FpcModule.CharacterModelInstance is AnimatedCharacterModel
                    characterModelInstance) ||
                !characterModelInstance.TryGetSubcontroller<OverlayAnimationsSubcontroller>(out subcontroller))
            {
                return;
            }
            subcontroller._overlayAnimations[1].OnStarted();
            subcontroller._overlayAnimations[1].SendRpc();
        }
    }
}
