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
using Exiled.API.Enums;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using InventorySystem.Items.MicroHID;
using Mirror;
using RGM.API.Features;
using RGM.API.DataBases;
using MultiBroadcast.API;

using static RGM.Variables.ServerManagers;

using static RGM.Modes.ABattleVariables.Abilities;
using static RGM.Modes.ABattleVariables.AbiltiyManagers;
using static RGM.Modes.ABattleVariables.MainManagers;
using static RGM.Modes.ABattleVariables.Cooldowns;
using static RGM.Modes.ABattleVariables.Serials;

using static RGM.Modes.ABattleFunctions.AbilityManagers;
using static RGM.Modes.ABattleFunctions.MainManagers;
using static RGM.Modes.ABattleFunctions.SpecificAbilities;

namespace RGM.Modes.ABattleEventArgs
{
    public static class PlayerEvents
    {
        public static void OnVerified(Exiled.Events.EventArgs.Player.VerifiedEventArgs ev)
        {
            if (!PlayerAbilities.ContainsKey(ev.Player))
            {
                PlayerWorkstation.Add(ev.Player, new List<Vector3>());
                PlayerAbilities.Add(ev.Player, new List<string>());
            }
        }

        public static async void OnTogglingNoClip(Exiled.Events.EventArgs.Player.TogglingNoClipEventArgs ev)
        {
            if (!ev.Player.IsCuffed)
            {
                if (PlayerAbilities[ev.Player].Contains("[일반] 회축"))
                {
                    if (Tools.TryGetLookPlayer(ev.Player, 4f, out Player player))
                    {
                        if (ev.Player != player && !MeleeCooldown.Contains(ev.Player) && ev.Player.LeadingTeam != player.LeadingTeam)
                        {
                            Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 0.7f);
                            player.Hurt(12.05f * 3 * DuplicateCount(ev.Player, "[일반] 회축"), "무지성으로 구타당해 죽었습니다.");

                            MeleeCooldown.Add(ev.Player);
                            await Task.Delay(1000);
                            MeleeCooldown.Remove(ev.Player);
                        }
                    }
                }

                if (PlayerAbilities[ev.Player].Contains("[전용] 절도죄"))
                {
                    if (!PickPocketCooldown.Contains(ev.Player))
                    {
                        if (Tools.TryGetLookPlayer(ev.Player, 2f, out Player player))
                        {
                            if (ev.Player != player && !MeleeCooldown.Contains(ev.Player))
                            {
                                if (!player.IsInventoryEmpty)
                                {
                                    Item Item = Tools.GetRandomValue(player.Items.ToList());

                                    player.RemoveItem(Item);
                                    player.ShowHint("주머니가 허전합니다..", 1.2f);
                                    Item I = ev.Player.AddItem(Item.Type);
                                    ev.Player.ShowHint("소매치기에 성공했습니다.", 1.2f);

                                    if (ev.Player.IsScp)
                                        ev.Player.CurrentItem = I;

                                    Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 0.7f);

                                    MeleeCooldown.Add(ev.Player);
                                    await Task.Delay(60 * 1000);
                                    MeleeCooldown.Remove(ev.Player);
                                }
                                else
                                {
                                    player.ShowHint("누군가가 소매치기를 하려고 시도했습니다.", 1.2f);
                                    ev.Player.ShowHint("소매치기에 실패했습니다.\n대상은 아이템을 가지고 있지 않습니다.", 1.2f);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void OnJumping(Exiled.Events.EventArgs.Player.JumpingEventArgs ev)
        {
            if (Physics.Raycast(ev.Player.Position, Vector3.down, out RaycastHit hit, 5, (LayerMask)1))
            {
                if (hit.transform != null)
                {
                    Transform WorkStation = hit.transform.parent.parent;

                    if (WorkStation.name.Contains("Work Station") && !PlayerWorkstation[ev.Player].Contains(WorkStation.position))
                    {
                        PlayerWorkstation[ev.Player].Add(WorkStation.position);

                        AddAbilityVote(ev.Player);
                    }
                }
            }

            if (PlayerAbilities[ev.Player].Contains("[영웅] 점멸"))
            {
                if (!TeleportCooldown.Contains(ev.Player))
                {
                    TeleportCooldown.Add(ev.Player);

                    Door nearestDoor = null;
                    float radius = 99999;

                    foreach (var door in Door.List.Where(x => !x.IsElevator && x.Zone != ZoneType.LightContainment && !x.Type.ToString().Contains("Scp079")))
                    {
                        float Distance = Vector3.Distance(door.Position, ev.Player.Position);

                        if (Distance < radius)
                        {
                            nearestDoor = door;
                            radius = Distance;
                        }
                    }

                    Vector3 pos = nearestDoor.Position;

                    ev.Player.Position = new Vector3(pos.x, pos.y + 2, pos.z);

                    Timing.CallDelayed(15, () => { TeleportCooldown.Remove(ev.Player); });
                }
            }
        }

        public static void OnChangedItem(Exiled.Events.EventArgs.Player.ChangedItemEventArgs ev)
        {
            if (ev.Item != null)
            {
                if (PickCoinSerials.Contains(ev.Item.Serial))
                    ev.Player.ShowHint($"이 동전을 튕기면 <b><color={RatingColor["일반"]}>뽑기</color></b> 능력을 사용할 수 있습니다.");

                else if (EscapeCoinSerials.Contains(ev.Item.Serial))
                    ev.Player.ShowHint($"이 동전을 튕기면 <b><color={RatingColor["일반"]}>위기 탈출</color></b> 능력을 사용할 수 있습니다.");

                else if (EarthquakeCoinSerials.Contains(ev.Item.Serial))
                    ev.Player.ShowHint($"이 동전을 튕기면 <b><color={RatingColor["일반"]}>지진</color></b> 능력을 사용할 수 있습니다.");

                else if (FollowCoinSerials.Contains(ev.Item.Serial))
                    ev.Player.ShowHint($"이 동전을 튕기면 <b><color={RatingColor["희귀"]}>순간이동</color></b> 능력을 사용할 수 있습니다.");

                else if (GrapCoinSerials.Contains(ev.Item.Serial))
                    ev.Player.ShowHint($"이 동전을 튕기면 <b><color={RatingColor["희귀"]}>갈고리</color></b> 능력을 사용할 수 있습니다.");

                else if (ClockCoinSerials.Contains(ev.Item.Serial))
                    ev.Player.ShowHint($"이 동전을 튕기면 <b><color={RatingColor["희귀"]}>회중시계</color></color></b> 능력을 사용할 수 있습니다.");

                else if (ContractCoinSerials.Contains(ev.Item.Serial))
                    ev.Player.ShowHint($"이 동전을 튕기면 <b><color={RatingColor["희귀"]}>계약</color></color></b> 능력을 사용할 수 있습니다.");

                else if (CallSnakeHandsSerials.Contains(ev.Item.Serial))
                    ev.Player.ShowHint($"우클릭하면 <b><color={RatingColor["전설"]}>뱀의 손 무전기</color></b> 능력을 사용할 수 있습니다.");

                else if (FlashLightSerials.Contains(ev.Item.Serial))
                    ev.Player.ShowHint($"손전등을 상대에게 비추면 <b><color={RatingColor["전설"]}>플래시라이트</color></b> 능력을 사용할 수 있습니다.");

                else if (FlamethrowerSerials.Contains(ev.Item.Serial))
                    ev.Player.ShowHint($"<b><color={RatingColor["전설"]}>화염 방사기</color></b> 능력이 있는 Micro-HID 입니다!");

                else if (ChaosCoinSerials.Contains(ev.Item.Serial))
                    ev.Player.ShowHint($"이 동전을 튕기면 <b><color={RatingColor["전용"]}>혼돈의 손길</color></color></b> 능력을 사용할 수 있습니다.");
            }
        }

        public static void OnFlippingCoin(Exiled.Events.EventArgs.Player.FlippingCoinEventArgs ev)
        {
            ushort Serial = ev.Item.Serial;

            if (PickCoinSerials.Contains(Serial))
            {
                ev.Item.Destroy();

                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    for (int i = 1; i < 4; i++)
                        AddAbility(ev.Player);
                }
            }
            else if (EscapeCoinSerials.Contains(Serial))
            {
                if (Tools.TryGetLookPlayer(ev.Player, 10f, out Player player))
                {
                    if (player.LeadingTeam != ev.Player.LeadingTeam)
                    {
                        ev.Item.Destroy();

                        player.EnableEffect(EffectType.Ensnared, 3f);

                        Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 1f);
                    }
                    else
                        ev.Player.ShowHint("잘못된 대상입니다.");
                }
                else
                    ev.Player.ShowHint("대상을 정확히 지정해 주세요.");
            }
            else if (FollowCoinSerials.Contains(Serial))
            {
                ev.Item.Destroy();

                Player target = Tools.GetRandomValue(Player.List.Where(x => x != ev.Player && x.IsAlive && x.Role.Type != RoleTypeId.Scp079).ToList());
                ev.Player.Position = target.Position;
            }
            else if (GrapCoinSerials.Contains(Serial))
            {
                ev.Item.Destroy();

                Player target1 = Tools.GetRandomValue(Player.List.Where(x => x.IsAlive && x != ev.Player && x.Role.Type != RoleTypeId.Scp079).ToList());
                target1.Position = ev.Player.Position;
            }
            else if (ClockCoinSerials.Contains(Serial))
            {
                ev.Item.Destroy();

                ev.Player.EnableEffect(EffectType.Ensnared, 1, 3);

                GodModePlayers.Add(ev.Player);

                Timing.CallDelayed(3, () =>
                {
                    if (GodModePlayers.Contains(ev.Player))
                        GodModePlayers.Remove(ev.Player);
                });
            }
            else if (ContractCoinSerials.Contains(Serial))
            {
                ev.Item.Destroy();

                ev.Player.Kill("계약에 따라 당신은 죽었습니다.");

                Timing.WaitUntilTrue(() => !ev.Player.IsAlive);

                for (int i = 1; i < 4; i++)
                    AddAbility(ev.Player);
            }
            else if (ChaosCoinSerials.Contains(Serial))
            {
                ev.Item.Destroy();

                PlayerAbilities[ev.Player].Clear();
                PlayerWorkstation[ev.Player].Clear();

                ev.Player.DisableAllEffects();
            }
        }

        public static void OnTogglingRadio(Exiled.Events.EventArgs.Player.TogglingRadioEventArgs ev)
        {
            if (CallSnakeHandsSerials.Contains(ev.Item.Serial))
            {
                ev.Item.Destroy();

                ev.Player.Role.Set(RoleTypeId.Tutorial, SpawnReason.ForceClass, RoleSpawnFlags.None);

                CallSnakeHand(ev.Player, Player.List.Where(x => x.IsDead).ToList());
            }
        }

        public static void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            if (ev.DamageHandler.Type == DamageType.Unknown)
                return;

            if (PlayerAbilities[ev.Player].Contains("[전용] RTX4090"))
            {
                ev.IsAllowed = false;

                PlayerAbilities[ev.Player].Clear();

                Timing.CallDelayed(0.1f, () =>
                {
                    ev.Player.Role.Set(RoleTypeId.Tutorial);

                    Vector3 Pos = Door.Get(DoorType.Scp079First).Position;
                    ev.Player.Position = new Vector3(Pos.x, Pos.y + 2, Pos.z);

                    for (int i = 1; i < UnityEngine.Random.Range(7, 12) * DuplicateCount(ev.Player, "[전용] RTX4090"); i++)
                        AddAbility(ev.Player);
                });
            }

            if (ev.Attacker != null && ev.DamageHandler.Type != DamageType.Warhead)
            {
                if (PlayerAbilities[ev.Player].Contains("[일반] 보험"))
                {
                    ev.IsAllowed = false;

                    PlayerAbilities[ev.Player].Remove("[일반] 보험");
                    return;
                }

                if (PlayerAbilities[ev.Player].Contains("[영웅] 구사일생"))
                {
                    ev.IsAllowed = false;

                    PlayerAbilities[ev.Player].Remove("[영웅] 구사일생");

                    ev.Player.EnableEffect(EffectType.Blinded, 1, 3);
                    ev.Player.EnableEffect(EffectType.Invisible, 1, 3);
                    ev.Player.GetEffect(EffectType.MovementBoost).Intensity += 20;
                    GodModePlayers.Add(ev.Player);

                    Timing.CallDelayed(3f, () =>
                    {
                        if (ev.Player.GetEffect(EffectType.MovementBoost).Intensity >= 20)
                            ev.Player.GetEffect(EffectType.MovementBoost).Intensity -= 20;

                        else
                            ev.Player.GetEffect(EffectType.MovementBoost).Intensity = 0;

                        if (GodModePlayers.Contains(ev.Player))
                            GodModePlayers.Remove(ev.Player);
                    });

                    return;
                }

                if (PlayerAbilities[ev.Player].Contains("[전설] 마술사"))
                {
                    ev.IsAllowed = false;

                    PlayerAbilities[ev.Player].Remove("[전설] 마술사");

                    ev.Player.Role.Set(ev.Attacker.Role, SpawnReason.ForceClass, RoleSpawnFlags.None);
                    ev.Player.Health = ev.Attacker.Health;
                    foreach (Item Item in ev.Attacker.Items)
                        ev.Player.AddItem(Item.Type);

                    PlayerAbilities[ev.Player].Clear();

                    foreach (string ability in PlayerAbilities[ev.Attacker])
                        AddAbility(ev.Player, ability);

                    ev.Attacker.Kill($"몸이 교체되는 마술에 당했네요!");

                    return;
                }

                if (PlayerAbilities[ev.Player].Contains("[신화] 조커"))
                {
                    ev.IsAllowed = false;

                    PlayerAbilities[ev.Player].Remove("[신화] 조커");

                    Player.List.ToList().ForEach(x => x.ShowHint("<b><i><color=#FF0000>조</color><color=#E70717>커</color><color=#D00F2E>를</color> <color=#A21E5C>건</color><color=#8B2673>들</color><color=#732E8B>인</color> <color=#453DB9>죄</color><color=#2E45D0>다</color><color=#174DE7>!</color></i></b>", 3));

                    ev.Player.MaxHealth *= UnityEngine.Random.Range(1, 4);
                    ev.Player.Health = ev.Player.MaxHealth;

                    GodModePlayers.Add(ev.Player);

                    if (PlayerAbilities.ContainsKey(ev.Attacker))
                    {
                        if (PlayerAbilities[ev.Attacker].Count > 0)
                            PlayerAbilities[ev.Attacker].Remove(Tools.GetRandomValue(PlayerAbilities[ev.Attacker].ToList()));
                    }

                    for (int i = 1; i < 3; i++)
                        AddAbility(ev.Player, Tools.GetRandomValue(LegendAbilities.Keys.ToList()));

                    Timing.CallDelayed(3, () =>
                    {
                        if (GodModePlayers.Contains(ev.Player))
                            GodModePlayers.Remove(ev.Player);
                    });

                    return;
                }

                // 죽음이 확정된 상황

                if (PlayerAbilities[ev.Player].Contains("[영웅] 최후의 발악"))
                {
                    ev.IsAllowed = false;

                    PlayerAbilities[ev.Player].Remove("[영웅] 최후의 발악");

                    ev.Player.GetEffect(EffectType.MovementBoost).Intensity += 30;
                    GodModePlayers.Add(ev.Player);

                    Timing.CallDelayed(5f, () =>
                    {
                        if (GodModePlayers.Contains(ev.Player))
                            GodModePlayers.Remove(ev.Player);

                        if (ev.Player.IsAlive)
                            ev.Player.Kill("최후의 발악의 효과로 사망하였습니다.");
                    });
                }

                if (PlayerAbilities[ev.Player].Contains("[희귀] 순교"))
                {
                    for (int i = 1; i < DuplicateCount(ev.Player, "[희귀] 순교") + 1; i++)
                    {
                        var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, ev.Player);
                        g.FuseTime = 3f;
                        g.SpawnActive(ev.Player.Position, ev.Player);
                    }
                }

                if (PlayerAbilities[ev.Player].Contains("[영웅] 슈퍼 스타"))
                {
                    foreach (var player in Player.List.Where(x => !x.IsNPC))
                        player.AddBroadcast(10, $"<size=20><color={RatingColor["영웅"]}>슈퍼 스타</color>였던 {ev.Player.Nickname}(<color={ev.Player.Role.Color.ToHex()}>{Trans.Role[ev.Player.Role.Type]}</color>)(은)는 " +
                            $"{ev.Attacker.Nickname}(<color={ev.Attacker.Role.Color.ToHex()}>{Trans.Role[ev.Attacker.Role.Type]}</color>)에 의해 <b>{ev.Player.CurrentRoom.Name}</b>에서 사망하였습니다.</size>");
                }

                if (PlayerAbilities[ev.Player].Contains("[영웅] 극독"))
                {
                    ev.Attacker.EnableEffect(EffectType.CardiacArrest, 1, 12 * DuplicateCount(ev.Player, "[영웅] 극독"));

                    ev.Attacker.ShowHint("극독에 당했습니다!");
                }

                if (ev.Attacker != null)
                {
                    if (PlayerAbilities[ev.Attacker].Contains("[신화] 차원 강탈자"))
                    {
                        foreach (var Ability in PlayerAbilities[ev.Player])
                            PlayerAbilities[ev.Attacker].Add(Ability);

                        ev.Player.ShowHint("능력을 강탈당했습니다!");
                    }
                }
            }
        }

        public static void OnDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            if (ev.DamageHandler.Type == DamageType.Unknown)
                return;

            try
            {
                if (ev.Attacker != null)
                {
                    if (PlayerAbilities[ev.Player].Contains("[일반] 대물림"))
                    {
                        for (int i = 1; i < DuplicateCount(ev.Player, "[일반] 대물림") + 1; i++)
                        {
                            List<Player> GetList = Player.List.Where(x => x != ev.Player && x.IsAlive && x.Role.Team == ev.TargetOldRole.GetTeam()).ToList();
                            Player Get = null;
                            string Ability = null;

                            if (GetList.Count > 0)
                            {
                                Get = Tools.GetRandomValue(GetList);
                                Ability = Tools.GetRandomValue(PlayerAbilities[ev.Player]);

                                if (PlayerAbilities.ContainsKey(Get))
                                    PlayerAbilities[Get].Add(Ability);

                                ev.Player.ShowHint($"{Get.Nickname}(에)게 {Ability}(을)를 양도하였습니다.", 5);
                                Get.ShowHint($"{ev.Player.Nickname}(으)로부터 {Ability}(을)를 양도받았습니다.", 5);
                            }
                            else
                                ev.Player.ShowHint($"대물림 사용에 실패했습니다. 아군이 존재하지 않습니다.", 5);
                        }
                    }
                    if (PlayerAbilities[ev.Attacker].Contains("[전설] 킬스트릭"))
                    {
                        if (UnityEngine.Random.Range(1, 3) == 1)
                            AddAbilityVote(ev.Attacker);

                        else
                            AddAbility(ev.Attacker);
                    }
                    if (PlayerAbilities[ev.Attacker].Contains("[전용] 공포"))
                    {
                        foreach (var player in Player.List.Where(x => !x.IsNPC && !x.IsScp))
                        {
                            if (Vector3.Distance(player.Position, ev.Attacker.Position) <= 10)
                                player.EnableEffect(EffectType.Ensnared, 1, 0.75f * DuplicateCount(ev.Attacker, "[전용] 공포"));
                        }
                    }
                }
            }
            finally
            {
                PlayerWorkstation[ev.Player].Clear();
                PlayerAbilities[ev.Player].Clear();
                ev.Player.Scale = new Vector3(1, 1, 1);
                Server.ExecuteCommand($"/speak {ev.Player.Id} disable");
                ev.Player.IsUsingStamina = true;
                if (GodModePlayers.Contains(ev.Player))
                    GodModePlayers.Remove(ev.Player);
            }
        }

        public static void OnEscaping(Exiled.Events.EventArgs.Player.EscapingEventArgs ev)
        {
            if (new List<RoleTypeId>()
                {
                    RoleTypeId.Scientist,
                    RoleTypeId.ClassD
                }.Contains(ev.Player.Role.Type))
            {
                PlayerAbilities[ev.Player].Clear();
                PlayerWorkstation[ev.Player].Clear();
            }
        }

        public static void OnInteractingDoor(Exiled.Events.EventArgs.Player.InteractingDoorEventArgs ev)
        {
            if (ev.Door.Type == DoorType.Scp079First)
            {
                ev.Player.ShowHint("이 헤비도어는 능력으로 개폐가 불가능합니다.", 1.2f);
                return;
            }
            else if (ev.IsAllowed)
                return;

            else if (ev.Player != null && ((PlayerAbilities[ev.Player].Contains("[일반] 행운") && UnityEngine.Random.Range(1, 101) <= 5 * DuplicateCount(ev.Player, "[일반] 행운")
                || PlayerAbilities[ev.Player].Contains("[영웅] 수리 기사"))))
            {
                if (ev.Door.IsOpen)
                    ev.Door.IsOpen = false;

                else
                    ev.Door.IsOpen = true;
            }
        }

        public static void OnDroppedItem(Exiled.Events.EventArgs.Player.DroppedItemEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[영웅] 도박꾼"))
            {
                if (UnityEngine.Random.Range(1, 101) <= 10 * ((1 / 2) * DuplicateCount(ev.Player, "[영웅] 도박꾼")))
                    ev.Player.EnableEffect(EffectType.SeveredHands);

                else
                {
                    ev.Pickup.Destroy();

                    List<ItemType> ItemTypes = Tools.EnumToList<ItemType>();

                    Item Item = Item.Create(Tools.GetRandomValue(ItemTypes));
                    Item.CreatePickup(ev.Player.Position);
                }
            }
        }

        public static void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            if (ev.Attacker != null)
            {
                if (PlayerAbilities[ev.Attacker].Contains("[일반] 단련"))
                    ev.DamageHandler.Damage = (int)(ev.DamageHandler.Damage * (1 + (0.2 * DuplicateCount(ev.Player, "[일반] 단련"))));

                if (PlayerAbilities[ev.Attacker].Contains("[희귀] 흡혈귀") && ev.Attacker.LeadingTeam != ev.Player.LeadingTeam)
                    ev.Attacker.AddAhp((20 * DuplicateCount(ev.Player, "[희귀] 흡혈귀")) * (ev.DamageHandler.Damage / 100));

                if (ev.Attacker.CurrentItem != null && FlamethrowerSerials.Contains(ev.Attacker.CurrentItem.Serial))
                {
                    ev.DamageHandler.Damage /= 5;

                    ev.Player.EnableEffect(EffectType.Burned, 1, 1.2f);
                }

                if (PlayerAbilities[ev.Player].Contains("[희귀] 반창고"))
                {
                    if (ev.Player.Health <= ev.Player.MaxHealth / 2)
                    {
                        PlayerAbilities[ev.Player].Remove("[희귀] 반창고");

                        ev.Player.Health = ev.Player.MaxHealth;
                    }
                }

                if (PlayerAbilities[ev.Attacker].Contains("[신화] 로켓 런처") && ev.Attacker.LeadingTeam != ev.Player.LeadingTeam)
                {
                    if (UnityEngine.Random.Range(1, 6) == 1)
                        Server.ExecuteCommand($"/rocket {ev.Player.Id} 1");
                }

                if (PlayerAbilities[ev.Attacker].Contains("[전용] 집단 지성") && ev.Attacker.LeadingTeam != ev.Player.LeadingTeam)
                {
                    int PowerCount = 0;

                    foreach (var player in Player.List.Where(x => !x.IsNPC && x.IsAlive && x.LeadingTeam == ev.Player.LeadingTeam && x != ev.Player))
                    {
                        if (Vector3.Distance(player.Position, ev.Player.Position) < 11)
                            PowerCount++;
                    }

                    ev.DamageHandler.Damage = (int)(ev.DamageHandler.Damage * (1 + (0.1 * PowerCount)));
                }

                if (PlayerAbilities[ev.Player].Contains("[전용] 신기루"))
                {
                    if (UnityEngine.Random.Range(1, 21) == 1)
                        ev.Player.EnableEffect(EffectType.Invisible, 1, 1.25f * DuplicateCount(ev.Player, "[전용] 신기루"));
                }

                if (PlayerAbilities[ev.Player].Contains("[전용] 격노"))
                {
                    if (ev.Player.Role is Scp096Role Scp096)
                    {
                        if (Scp096.RageManager.IsEnraged && ev.Attacker != null && ev.Attacker != ev.Player)
                            ev.DamageHandler.Damage /= DuplicateCount(ev.Player, "[전용] 격노") + 1;
                    }
                }

                if (PlayerAbilities[ev.Attacker].Contains("[전용] 별자리 찢기"))
                {
                    if (UnityEngine.Random.Range(1, 5) == 1)
                        ev.DamageHandler.Damage = -1;
                }

                if (PlayerAbilities[ev.Attacker].Contains("[전용] 숙련된 암살자"))
                {
                    if (ev.DamageHandler.Type == DamageType.Strangled)
                        ev.DamageHandler.Damage *= 10;
                }

                if (PlayerAbilities[ev.Player].Contains("[전용] 반블럭"))
                {
                    ev.Player.GetEffect(EffectType.MovementBoost).Intensity += 25;

                    Timing.CallDelayed(3f, () =>
                    {
                        if (ev.Player.GetEffect(EffectType.MovementBoost).Intensity >= 25)
                            ev.Player.GetEffect(EffectType.MovementBoost).Intensity -= 25;

                        else
                            ev.Player.GetEffect(EffectType.MovementBoost).Intensity = 0;
                    });
                }

                if (PlayerAbilities[ev.Player].Contains("[시너지] 드루이드"))
                {
                    if (UnityEngine.Random.Range(1, 3) == 1)
                    {
                        ev.IsAllowed = false;
                        ev.Attacker.Hurt(ev.DamageHandler.Damage, $"{ev.Player.Nickname}의 정령의 가호에 의해 사망하였습니다.");
                    }
                }
            }
        }

        public static void OnHurt(Exiled.Events.EventArgs.Player.HurtEventArgs ev)
        {
            if (ev.Attacker != null && PlayerAbilities[ev.Attacker].Contains("[신화] 스피릿"))
                ev.Attacker.DisableEffect(EffectType.Invisible);

            if (PlayerAbilities[ev.Player].Contains("[신화] 스피릿"))
                ev.Player.DisableEffect(EffectType.Invisible);
        }

        public static void OnTriggeringTesla(Exiled.Events.EventArgs.Player.TriggeringTeslaEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[영웅] 수리 기사"))
                ev.DisableTesla = true;
        }

        public static void OnChangingMicroHIDState(Exiled.Events.EventArgs.Player.ChangingMicroHIDStateEventArgs ev)
        {
            if (FlamethrowerSerials.Contains(ev.Player.CurrentItem.Serial))
            {
                if (ev.OldState == HidState.Idle && ev.NewState == HidState.PoweringUp)
                    ev.NewState = HidState.Firing;
            }
        }

        public static async void OnVoiceChatting(Exiled.Events.EventArgs.Player.VoiceChattingEventArgs ev)
        {
            if (PlayerAbilities[ev.Player].Contains("[전설] 괴성"))
            {
                if (!RoaringSoundCooldown.Contains(ev.Player))
                {
                    if (Tools.TryGetLookPlayer(ev.Player, 10f, out Player target) && target.LeadingTeam != ev.Player.LeadingTeam)
                    {
                        RoaringSoundCooldown.Add(ev.Player);

                        string sn = $"{UnityEngine.Random.Range(-9999999.9f, 9999999.9f)}";
                        Player dj = Tools.SpawnDJ($"{ev.Player.Nickname}의 괴성", RoleTypeId.Spectator, Vector3.zero, sn);

                        GGUtils.Gtool.PlaySound(sn, "GmanRoaringSound", VoiceChat.VoiceChatChannel.Intercom);

                        foreach (var player in Player.List.Where(x => !x.IsNPC && x.LeadingTeam != ev.Player.LeadingTeam && x.IsAlive))
                        {
                            player.EnableEffect(EffectType.Flashed, 1, 0.3f);
                            player.EnableEffect(EffectType.Blinded, 1, 7.5f);
                            player.EnableEffect(EffectType.SinkHole, 1, 12f);
                            player.EnableEffect(EffectType.Slowness, 150, 5.5f);
                        }

                        await Task.Delay(650);

                        Player.List.ToList().ForEach(x => x.ShowHint("<b><i><color=#B08A03>저</color><color=#9C7A02>?</color><color=#886B02>!</color><color=#755C01>주</color><color=#614C01>받</color><color=#4E3D01>은</color> <color=#271E00>괴</color><color=#130F00>성</color></i></b>", 5));

                        for (int i = 1; i < 71; i++)
                        {
                            Warhead.Shake();

                            await Task.Delay(100);
                        }

                        Timing.CallDelayed(5f, () =>
                        {
                            NetworkServer.Destroy(dj.ReferenceHub.gameObject);
                        });

                        Timing.CallDelayed(100 * (1 / DuplicateCount(ev.Player, "[전설] 괴성")), () =>
                        {
                            if (RoaringSoundCooldown.Contains(ev.Player))
                                RoaringSoundCooldown.Remove(ev.Player);
                        });
                    }
                }
            }
            if (PlayerAbilities[ev.Player].Contains("[전용] 고대의 존재 압도"))
            {
                foreach (var player in Player.List.Where(x => !x.IsNPC && !x.IsScp && x.IsAlive))
                {
                    if (player.CurrentRoom == ev.Player.CurrentRoom)
                        player.EnableEffect(EffectType.SinkHole, 1, 0.1f * DuplicateCount(ev.Player, "[전용] 고대의 존재 압도"));
                }
            }
        }
    }
}
