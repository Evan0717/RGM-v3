using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using UnityEngine;
using PlayerRoles;
using MEC;
using Exiled.API.Features.Roles;
using Exiled.API.Features.Items;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features.Serializable;
using MapEditorReborn.API.Features;
using MultiBroadcast.API;
using RGM.API.Features;
using RGM.API.DataBases;
using Exiled.API.Enums;

using static RGM.Modes.ABattleVariables.Abilities;
using static RGM.Modes.ABattleVariables.AbiltiyManagers;
using static RGM.Modes.ABattleVariables.MainManagers;
using static RGM.Modes.ABattleVariables.Cooldowns;
using static RGM.Modes.ABattleVariables.Serials;

using static RGM.Modes.ABattleFunctions.AbilityManagers;
using static RGM.Modes.ABattleFunctions.MainManagers;
using static RGM.Modes.ABattleFunctions.SpecificAbilities;

namespace RGM.Modes.ABattleIEnumerators
{
    public static class SpecificAbilities
    {
        public static IEnumerator<float> UpgradeBody()
        {
            while (true)
            {
                foreach (var player in Player.List.Where(x => !x.IsNPC))
                {
                    try
                    {
                        if (PlayerAbilities[player].Contains("[희귀] 육체 강화"))
                            if (player.MaxHealth > player.Health)
                                player.Health += DuplicateCount(player, "[희귀] 육체 강화");
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public static IEnumerator<float> Spirit()
        {
            while (true)
            {
                foreach (var player in Player.List.Where(x => !x.IsNPC))
                {
                    try
                    {
                        if (PlayerAbilities[player].Contains("[신화] 스피릿"))
                            player.EnableEffect(EffectType.Invisible);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }

                yield return Timing.WaitForSeconds(2f);
            }
        }

        public static IEnumerator<float> FlashLight()
        {
            while (true)
            {
                foreach (var player in Player.List.Where(x => !x.IsNPC))
                {
                    try
                    {
                        if (player.CurrentItem != null && FlashLightSerials.Contains(player.CurrentItem.Serial))
                        {
                            if (Physics.Raycast(player.ReferenceHub.PlayerCameraReference.position + player.ReferenceHub.PlayerCameraReference.forward * 0.2f, player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 45f, InventorySystem.Items.Firearms.Modules.StandardHitregBase.HitregMask) &&
                                hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
                            {
                                var target = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>());

                                if (player != target && player.LeadingTeam != target.LeadingTeam)
                                {
                                    Hitmarker.SendHitmarkerDirectly(player.ReferenceHub, 0.8f);
                                    target.EnableEffect(EffectType.Flashed, 1, 1f);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }

                yield return Timing.WaitForSeconds(0.1f);
            }
        }

        public static IEnumerator<float> Flamethrower()
        {
            while (true)
            {
                foreach (var Item in Item.List.Where(x => x.Type == ItemType.MicroHID))
                {
                    try
                    {
                        if (FlamethrowerSerials.Contains(Item.Serial))
                        {
                            MicroHid MicroHID = (MicroHid)Item;

                            MicroHID.Energy += 0.05f;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public static IEnumerator<float> Blessing()
        {
            while (true)
            {
                foreach (var player in Player.List.Where(x => !!x.IsNPC))
                {
                    try
                    {
                        if (player.IsAlive && PlayerAbilities[player].Contains("[전설] 영매"))
                        {
                            int s = player.CurrentSpectatingPlayers.Count();

                            player.GetEffect(EffectType.MovementBoost).Intensity = (byte)(2.5 * s * DuplicateCount(player, "[전설] 영매"));
                            player.GetEffect(EffectType.DamageReduction).Intensity = (byte)(2.5 * s * DuplicateCount(player, "[전설] 영매"));
                            player.Heal(0.35f * s * DuplicateCount(player, "[전설] 영매"));

                            if (player.Role is Scp079Role scp079)
                                scp079.Energy += 0.35f * s * DuplicateCount(player, "[전설] 영매");

                            if (s > 5)
                                player.IsUsingStamina = false;

                            else
                                player.IsUsingStamina = true;

                            if (s > 10)
                                player.IsBypassModeEnabled = true;

                            else
                                player.IsBypassModeEnabled = false;

                            if (s > 15)
                                player.EnableEffect(EffectType.Ghostly, 1, 1.2f);

                            if (s > 20)
                                player.EnableEffect(EffectType.Invisible, 1, 1.2f);

                            if (s > 25)
                            {
                                if (UnityEngine.Random.Range(1, 51) == 1)
                                {
                                    Item Item = player.AddItem(Tools.GetRandomValue(Tools.EnumToList<ItemType>()));

                                    if (player.IsScp)
                                        player.CurrentItem = Item;
                                }
                            }

                            if (s > 30)
                            {
                                if (!player.IsNoclipPermitted)
                                    player.IsNoclipPermitted = true;

                                player.AddBroadcast(1, "<b><i>[ALT] 키를 눌러 <color=red>신의 권능</color>을 사용할 수 있습니다!!!</i></b>");
                            }

                            else
                            {
                                if (player.IsNoclipPermitted)
                                    player.IsNoclipPermitted = false;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public static IEnumerator<float> Twinkle()
        {
            while (true)
            {
                foreach (var player in Player.List.Where(x => !x.IsNPC))
                {
                    try
                    {
                        if (PlayerAbilities.ContainsKey(player) && PlayerAbilities[player].Contains("[신화] 눈빛맨"))
                        {
                            foreach (var near in Player.List.Where(x => x.IsAlive && Vector3.Distance(x.Position, player.Position) < 11))
                            {
                                if (player != near && player.LeadingTeam != near.LeadingTeam)
                                {
                                    near.EnableEffect(EffectType.SinkHole, 1, 0.2f);
                                    near.EnableEffect(EffectType.Blinded, 1, 0.2f);
                                    near.Hurt(player.MaxHealth / 120, "눈빛의 힘에 압도당했습니다.");
                                    Hitmarker.SendHitmarkerDirectly(player.ReferenceHub, 1f);
                                }
                            }

                            if (Tools.TryGetLookPlayer(player, 100f, out Player target))
                            {
                                if (player != target && player.LeadingTeam != target.LeadingTeam)
                                {
                                    target.EnableEffect(EffectType.SinkHole, 1, 0.2f);
                                    target.EnableEffect(EffectType.Blinded, 1, 0.2f);
                                    target.Hurt(player.MaxHealth / 120, "눈빛의 힘에 압도당했습니다.");
                                    Hitmarker.SendHitmarkerDirectly(player.ReferenceHub, 0.5f);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }

                yield return Timing.WaitForSeconds(0.1f);
            }
        }

        public static IEnumerator<float> Medical()
        {
            while (true)
            {
                foreach (var player in Player.List.Where(x => !x.IsNPC && x.IsAlive))
                {
                    try
                    {
                        if (PlayerAbilities.ContainsKey(player))
                        {
                            if (PlayerAbilities[player].Contains("[전용] 의무병"))
                            {
                                foreach (var team in Player.List.Where(x => x.LeadingTeam == player.LeadingTeam && x.IsAlive && x != player))
                                {
                                    if (team.Health < team.MaxHealth)
                                        team.Health += 0.5f;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public static IEnumerator<float> Radar()
        {
            while (true)
            {
                foreach (var player in Player.List.Where(x => !x.IsNPC))
                {
                    try
                    {
                        if (player.CurrentItem != null && RadarSerials.Contains(player.CurrentItem.Serial))
                        {
                            if (Tools.TryGetNearestPlayer(player, out Player nearestPlayer, out float radius))
                            {
                                if (nearestPlayer != null && radius < 99999)
                                    player.ShowHint($"<color={nearestPlayer.Role.Color.ToHex()}>{Trans.Role[nearestPlayer.Role.Type]}</color> - {radius}m", 1.2f);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public static IEnumerator<float> Radiation()
        {
            LightSourceSerializable LightSource = new LightSourceSerializable("#FFD700", 100, 20, true);
            LightSourceObject Light = ObjectSpawner.SpawnLightSource(LightSource, Vector3.zero);

            while (true)
            {
                foreach (Player player in Player.List.Where(x => x.IsAlive))
                {
                    try
                    {
                        if (Tools.TryGetLookPlayer(player, 45f, out Player target))
                        {
                            if (PlayerAbilities.ContainsKey(target))
                            {
                                if (PlayerAbilities[target].Contains("[시너지] 광휘"))
                                {
                                    if (player != target && player.LeadingTeam != target.LeadingTeam)
                                    {
                                        Light.Position = target.Position;

                                        Hitmarker.SendHitmarkerDirectly(target.ReferenceHub, 0.8f);
                                        player.EnableEffect(EffectType.Flashed, 1, 1f);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }

                yield return Timing.WaitForSeconds(0.1f);
            }
        }

        public static IEnumerator<float> StickySwamp()
        {
            while (true)
            {
                foreach (var player in Player.List.Where(x => !x.IsNPC))
                {
                    try
                    {
                        if (PlayerAbilities[player].Contains("[전용] 끈적한 늪"))
                        {
                            foreach (var near in Player.List.Where(x => x.IsAlive && Vector3.Distance(x.Position, player.Position) < 6))
                            {
                                if (player != near && player.LeadingTeam != near.LeadingTeam)
                                    near.EnableEffect(EffectType.SinkHole, 1, 0.5f);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
