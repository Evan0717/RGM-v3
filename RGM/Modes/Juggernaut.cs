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

            Timing.RunCoroutine(OnModeStarted());
            Timing.RunCoroutine(AutoWarhead());

            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.SearchingPickup += OnSearchingPickup;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.ReceivingEffect += OnReceivingEffect;
            Exiled.Events.Handlers.Player.Handcuffing += OnHandcuffing;
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(10);

            foreach (var player in Player.List)
            {
                Spawned(player);
            }

            juggernaut = RGM.GetRandomValue(Player.List.ToList());

            juggernaut.Role.Set(RoleTypeId.Tutorial);
            juggernaut.Scale = new Vector3(1.2f, 1.1f, 1.2f);
            juggernaut.MaxHealth = 300 * Player.List.Count() + 100 * Player.List.Count();
            juggernaut.Health = juggernaut.MaxHealth;
            juggernaut.IsBypassModeEnabled = true;
            juggernaut.EnableEffect(EffectType.SinkHole);
            juggernaut.EnableEffect(EffectType.DamageReduction, 10);
            juggernaut.AddBroadcast(10, "<b><size=30>당신은 <color=#298A08>저거너트</color>입니다.</size></b>\n<size=25><i>본인을 제외한 모두를 사살하십시오.</i></size>");
            juggernaut.Position = new Vector3(123.8387f, 988.7921f, 25.39412f);

            List<ItemType> Items = new List<ItemType>() { ItemType.GunLogicer };
            foreach (var Item in Items)
                juggernaut.AddItem(Item);

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

            RGM.Instance.AutoNuke = true;
            Warhead.Start();
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            if (player.Role.Type == RoleTypeId.Scp3114)
                player.Role.Set(RoleTypeId.Scp939);

            if (player.Role is Scp079Role scp079)
                scp079.AddExperience(1205);
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
                if (ev.Player == juggernaut && ev.DamageHandler.Damage == -1)
                    ev.DamageHandler.Damage = 120.5f;
            }
        }

        public void OnReceivingEffect(Exiled.Events.EventArgs.Player.ReceivingEffectEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        public void OnHandcuffing(Exiled.Events.EventArgs.Player.HandcuffingEventArgs ev)
        {
            ev.IsAllowed = false;
        }
    }
}
