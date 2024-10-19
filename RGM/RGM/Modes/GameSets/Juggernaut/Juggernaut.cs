using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CustomRendering;
using Exiled.API.Features;
using MEC;
using Mirror;
using UnityEngine;
using Exiled.API.Features.Items;
using MultiBroadcast;
using MultiBroadcast.API;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using PlayerRoles;
using Exiled.API.Features.Roles;
using RGM.API.Features;
using RGM.API.DataBases;

using static RGM.Variables.ServerManagers;

namespace RGM.Modes
{
    class Juggernaut
    {
        public static Juggernaut Instance;

        public Player juggernaut;
        public List<Player> ScpAttackCooldown = new List<Player>();

        public void OnEnabled()
        {
            Server.FriendlyFire = true;
            Round.IsLocked = true;

            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.SearchingPickup += OnSearchingPickup;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.ReceivingEffect += OnReceivingEffect;
            Exiled.Events.Handlers.Player.Handcuffing += OnHandcuffing;

            Exiled.Events.Handlers.Item.ChargingJailbird += OnChargingJailbird;

            Timing.RunCoroutine(OnModeStarted());
            Timing.RunCoroutine(AutoWarhead());
        }

        public IEnumerator<float> OnModeStarted()
        {
            foreach (var player in Player.List)
            {
                Spawned(player);
            }

            juggernaut = Tools.GetRandomValue(Player.List.ToList());

            juggernaut.Role.Set(RoleTypeId.Tutorial);
            juggernaut.Scale = new Vector3(1.2f, 1.1f, 1.2f);
            juggernaut.MaxHealth = 300 * Player.List.Count() + 100 * Player.List.Count();
            juggernaut.Health = juggernaut.MaxHealth;
            juggernaut.IsBypassModeEnabled = true;
            juggernaut.EnableEffect(EffectType.SinkHole);
            juggernaut.EnableEffect(EffectType.DamageReduction, 10);
            juggernaut.AddBroadcast(10, "<b><size=30>당신은 <color=#298A08>저거너트</color>입니다.</size></b>\n<size=25><i>본인을 제외한 모두를 사살하십시오.</i></size>");
            juggernaut.Position = new Vector3(123.8387f, 988.7921f, 25.39412f);

            List<ItemType> Items = new List<ItemType>() { ItemType.GunLogicer, ItemType.Jailbird };
            foreach (var Item in Items)
                juggernaut.AddItem(Item);

            Timing.RunCoroutine(FindLocate());

            bool IsEnd = false;
            while (!IsEnd)
            {
                if (juggernaut.IsAlive)
                {
                    if (Player.List.Where(x => x.IsAlive).Count() <= 1)
                    {
                        Player.List.ToList().ForEach(x => x.AddBroadcast(20, "<size=25>더 이상 저지할 수 있는 <b>Site-76 구성원</b>이 없습니다.</size>\n<color=#298A08>저거너트</color>의 승리입니다."));
                        IsEnd = true;
                    }
                }
                else
                {
                    Player.List.ToList().ForEach(x => x.AddBroadcast(20, "<size=25><color=#298A08>저거너트</color>가 사망했습니다.</size>\n<b>Site-76 구성원</b>들의 승리입니다."));
                    IsEnd = true;
                }

                yield return Timing.WaitForSeconds(1f);
            }

            Player.List.ToList().ForEach(x => x.Role.Set(RoleTypeId.Tutorial, RoleSpawnFlags.None));
            Round.IsLocked = false;
        }

        public IEnumerator<float> AutoWarhead()
        {
            yield return Timing.WaitForSeconds(9 * 60);

            Server.ExecuteCommand("/cassie_sl 1분 뒤 <color=red>자동핵</color>이 작동됩니다.");

            yield return Timing.WaitForSeconds(1 * 60);

            AutoNuke = true;
            Warhead.Start();
        }

        public IEnumerator<float> FindLocate()
        {
            while (true)
            {
                if (Tools.TryGetNearestPlayer(juggernaut, out Player nearestPlayer, out float radius))
                    juggernaut.ShowHint($"<b>[ <color={nearestPlayer.Role.Color.ToHex()}>{Trans.Role[nearestPlayer.Role.Type]}</color>, 거리: {radius} ]</b>", 1.2f);

                else
                    juggernaut.ShowHint("당신은 임무를 완수하였습니다.", 1.2f);

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            if (player.IsAlive)
            {
                List<RoleTypeId> ScpsList = new List<RoleTypeId>()
                {
                    RoleTypeId.Scp3114,
                    RoleTypeId.Scp079
                };

                if (ScpsList.Contains(player.Role))
                    player.Role.Set(Tools.GetRandomValue(Tools.EnumToList<RoleTypeId>().Where(x => !ScpsList.Contains(x)).ToList()));
            }
        }

        public void OnSearchingPickup(Exiled.Events.EventArgs.Player.SearchingPickupEventArgs ev)
        {
            if (ev.Player == juggernaut)
                ev.IsAllowed = false;
        }

        public void OnDroppingItem(Exiled.Events.EventArgs.Player.DroppingItemEventArgs ev)
        {
            if (ev.Player == juggernaut)
                ev.IsAllowed = false;
        }

        public void OnShooting(Exiled.Events.EventArgs.Player.ShootingEventArgs ev)
        {
            if (ev.Player == juggernaut)
                ev.Player.CurrentItem.As<Firearm>().Ammo = 250;
        }

        public async void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            if (ev.Attacker != null)
            {
                if (ev.Player == juggernaut || ev.Attacker == juggernaut)
                {
                    if (ev.Attacker == juggernaut && ev.Player != juggernaut)
                    {
                        ev.DamageHandler.Damage = ev.DamageHandler.Damage * 3;
                    }
                    else if (ev.Attacker != juggernaut && ev.Player == juggernaut)
                    {
                        List<RoleTypeId> Scps = new List<RoleTypeId>() 
                        { 
                            RoleTypeId.Scp173,
                            RoleTypeId.Scp049,
                            RoleTypeId.Scp106,
                            RoleTypeId.Scp096,
                            RoleTypeId.Scp939
                        };

                        if (Scps.Contains(ev.Attacker.Role.Type) && !ScpAttackCooldown.Contains(ev.Attacker))
                        {
                            ev.IsAllowed = false;
                            ev.Player.Hurt(120.5f, DamageType.Scp);
                            ev.Attacker.ShowHitMarker(1.5f);

                            ScpAttackCooldown.Add(ev.Attacker);
                            await Task.Delay(1500);
                            ScpAttackCooldown.Remove(ev.Attacker);
                        }
                    }
                }
                else
                {
                    ev.IsAllowed = false;
                }
            }
        }

        public void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            if (ev.Attacker != null)
            {
                if (ev.Player == juggernaut && ev.Attacker.Role.Type == RoleTypeId.Scp173)
                    ev.DamageHandler.Damage = 120.5f;
            }
        }

        public void OnReceivingEffect(Exiled.Events.EventArgs.Player.ReceivingEffectEventArgs ev)
        {
            if (ev.Player == juggernaut && ev.Effect.GetEffectType() != EffectType.SinkHole)
                ev.IsAllowed = false;
        }

        public void OnHandcuffing(Exiled.Events.EventArgs.Player.HandcuffingEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        public void OnChargingJailbird(Exiled.Events.EventArgs.Item.ChargingJailbirdEventArgs ev)
        {
            if (ev.Player == juggernaut)
                ev.Item.As<Exiled.API.Features.Items.Jailbird>().TotalCharges = 0;
        }
    }
}
