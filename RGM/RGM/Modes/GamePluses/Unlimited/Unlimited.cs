using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using MultiBroadcast;
using PlayerRoles;

namespace RGM.Modes
{
    class Unlimited
    {
        public static Unlimited Instance;

        public int Tantrum = 0;

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.Healing += OnHealing;

            Exiled.Events.Handlers.Scp106.Teleporting += OnTeleporting;
            Exiled.Events.Handlers.Scp106.Stalking += OnStalking;
            Exiled.Events.Handlers.Scp106.Attacking += OnScp106Attacking;

            Exiled.Events.Handlers.Scp939.PlayingSound += OnPlayingSound;

            Exiled.Events.Handlers.Scp079.ChangingCamera += OnChangingCamera;

            Exiled.Events.Handlers.Scp049.StartingRecall += OnStartingRecall;
            Exiled.Events.Handlers.Scp049.Attacking += OnScp049Attacking;

            Exiled.Events.Handlers.Scp0492.ConsumedCorpse += OnConsumedCorpse;

            Exiled.Events.Handlers.Scp096.Enraging += OnEnraging;

            Exiled.Events.Handlers.Scp173.PlacingTantrum += OnPlacingTantrum;
            Exiled.Events.Handlers.Scp173.UsingBreakneckSpeeds += OnUsingBreakneckSpeeds;

            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            Exiled.Events.Handlers.Player.ChangingMicroHIDState += OnChangingMicroHIDState;
            Exiled.Events.Handlers.Player.UsingMicroHIDEnergy += OnUsingMicroHIDEnergy;

            Exiled.Events.Handlers.Item.ChargingJailbird += OnChargingJailbird;
        }

        public IEnumerator<float> OnModeStarted()
        {
            Player.List.ToList().ForEach(Spawned);

            yield return 1f;
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            player.IsUsingStamina = false;

            if (player.Role.Type == RoleTypeId.Scp0492)
                player.MaxHealth += 100;
        }


        public void OnHealing(Exiled.Events.EventArgs.Player.HealingEventArgs ev)
        {
            ev.Player.MaxHealth = ev.Player.Health + ev.Amount;
        }

        public async void OnTeleporting(Exiled.Events.EventArgs.Scp106.TeleportingEventArgs ev)
        {
            await Task.Delay(100);
            ev.Scp106.RemainingSinkholeCooldown = 0;
        }

        public async void OnStalking(Exiled.Events.EventArgs.Scp106.StalkingEventArgs ev)
        {
            await Task.Delay(100);
            ev.Scp106.RemainingSinkholeCooldown = 0;
        }

        public async void OnScp106Attacking(Exiled.Events.EventArgs.Scp106.AttackingEventArgs ev)
        {
            await Task.Delay(100);
            ev.Scp106.CaptureCooldown = 0;
        }

        public async void OnPlayingSound(Exiled.Events.EventArgs.Scp939.PlayingSoundEventArgs ev)
        {
            await Task.Delay(100);
            ev.Scp939.MimicryCooldown = 0;
        }

        public void OnChangingCamera(Exiled.Events.EventArgs.Scp079.ChangingCameraEventArgs ev)
        {
            ev.Scp079.Energy = 100000;
        }

        public async void OnStartingRecall(Exiled.Events.EventArgs.Scp049.StartingRecallEventArgs ev)
        {
            await Task.Delay(100);
            ev.Scp049.RemainingCallDuration = 0;
        }

        public async void OnScp049Attacking(Exiled.Events.EventArgs.Scp049.AttackingEventArgs ev)
        {
            await Task.Delay(100);
            ev.Scp049.RemainingAttackCooldown = 0;
            ev.Scp049.RemainingGoodSenseDuration = 0;
        }
        
        public void OnConsumedCorpse(Exiled.Events.EventArgs.Scp0492.ConsumedCorpseEventArgs ev)
        {
            ev.Player.MaxHealth = ev.Player.MaxHealth + 100;
        }

        public async void OnEnraging(Exiled.Events.EventArgs.Scp096.EnragingEventArgs ev)
        {
            await Task.Delay(100);
            ev.Scp096.EnrageCooldown = 0;
            ev.Scp096.EnragedTimeLeft = 99999;
            ev.Scp096.SprintingSpeed = 500;
        }

        public async void OnPlacingTantrum(Exiled.Events.EventArgs.Scp173.PlacingTantrumEventArgs ev)
        {
            if (Tantrum >= 10)
            {
                ev.Player.ShowHint($"렉 방지를 위해 10개로 제한됩니다. (하나 당 180초)", 1);
                ev.IsAllowed = false;
            }
            else
            {
                Tantrum += 1;
                await Task.Delay(100);
                ev.Cooldown.Remaining = 0;
                await Task.Delay(180 * 1000);
                Tantrum -= 1;
            }
        }

        public async void OnUsingBreakneckSpeeds(Exiled.Events.EventArgs.Scp173.UsingBreakneckSpeedsEventArgs ev)
        {
            await Task.Delay(100);
            ev.Scp173.RemainingBreakneckCooldown = 0;
        }

        public void OnShooting(Exiled.Events.EventArgs.Player.ShootingEventArgs ev)
        {
            ev.Player.CurrentItem.As<Exiled.API.Features.Items.Firearm>().Ammo = 250;
        }

        public void OnChangingMicroHIDState(Exiled.Events.EventArgs.Player.ChangingMicroHIDStateEventArgs ev)
        {
            ev.MicroHID.Energy += 100;
        }

        public void OnUsingMicroHIDEnergy(Exiled.Events.EventArgs.Player.UsingMicroHIDEnergyEventArgs ev)
        {
            ev.MicroHID.Energy += 100;
        }

        public void OnChargingJailbird(Exiled.Events.EventArgs.Item.ChargingJailbirdEventArgs ev)
        {
            ev.Item.As<Exiled.API.Features.Items.Jailbird>().TotalCharges = 0;
        }
    }
}
