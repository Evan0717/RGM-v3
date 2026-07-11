using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.Unlimited)]
    class Unlimited : Mode
    {
        public override string Name => "무제한";
        public override string Description => "말 그대로 제한이 없습니다.";
        public override string Detail =>
"""
몇몇 개의 기능들은 제한이 있습니다.
""";
        public override string Color => "3F13AB";

        public static Unlimited Instance;

        int Tantrum = 0;

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.Healing += OnHealing;
            Exiled.Events.Handlers.Player.UsingItem += OnUsingItem;
            Exiled.Events.Handlers.Player.UsingRadioBattery += OnUsingRadioBattery;
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            Exiled.Events.Handlers.Player.ChangingMicroHIDState += OnChangingMicroHIDState;
            Exiled.Events.Handlers.Player.UsingMicroHIDEnergy += OnUsingMicroHIDEnergy;

            Exiled.Events.Handlers.Scp106.Teleporting += OnTeleporting;
            Exiled.Events.Handlers.Scp106.Stalking += OnStalking;
            Exiled.Events.Handlers.Scp106.Attacking += OnScp106Attacking;

            Exiled.Events.Handlers.Scp939.PlayingSound += OnPlayingSound;

            Exiled.Events.Handlers.Scp079.ChangingCamera += OnChangingCamera;

            Exiled.Events.Handlers.Scp049.StartingRecall += OnStartingRecall;
            Exiled.Events.Handlers.Scp049.Attacking += OnScp049Attacking;

            Exiled.Events.Handlers.Scp096.Enraging += OnEnraging;

            Exiled.Events.Handlers.Scp173.PlacingTantrum += OnPlacingTantrum;
            Exiled.Events.Handlers.Scp173.UsingBreakneckSpeeds += OnUsingBreakneckSpeeds;

            Exiled.Events.Handlers.Item.ChargingJailbird += OnChargingJailbird;

            Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            Exiled.Events.Handlers.Player.Healing -= OnHealing;
            Exiled.Events.Handlers.Player.UsingItem -= OnUsingItem;
            Exiled.Events.Handlers.Player.UsingRadioBattery -= OnUsingRadioBattery;
            Exiled.Events.Handlers.Player.Shooting -= OnShooting;
            Exiled.Events.Handlers.Player.ChangingMicroHIDState -= OnChangingMicroHIDState;
            Exiled.Events.Handlers.Player.UsingMicroHIDEnergy -= OnUsingMicroHIDEnergy;

            Exiled.Events.Handlers.Scp106.Teleporting -= OnTeleporting;
            Exiled.Events.Handlers.Scp106.Stalking -= OnStalking;
            Exiled.Events.Handlers.Scp106.Attacking -= OnScp106Attacking;

            Exiled.Events.Handlers.Scp939.PlayingSound -= OnPlayingSound;

            Exiled.Events.Handlers.Scp079.ChangingCamera -= OnChangingCamera;

            Exiled.Events.Handlers.Scp049.StartingRecall -= OnStartingRecall;
            Exiled.Events.Handlers.Scp049.Attacking -= OnScp049Attacking;

            Exiled.Events.Handlers.Scp096.Enraging -= OnEnraging;

            Exiled.Events.Handlers.Scp173.PlacingTantrum -= OnPlacingTantrum;
            Exiled.Events.Handlers.Scp173.UsingBreakneckSpeeds -= OnUsingBreakneckSpeeds;

            Exiled.Events.Handlers.Item.ChargingJailbird -= OnChargingJailbird;

            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            if (Random.Range(1, 101) <= 10) { //10% 확률로 워크스테이션 업그레이드 시작
                Tools.TryInstallMode(ModeType.ABattle);
            }
            PlayerManager.List.ToList().ForEach(Spawned);

            while (true)
            {
                foreach (var player in PlayerManager.List)
                {
                    if (player.Role is Scp049Role scp049)
                    {
                        if (scp049.CallCooldown > 0)
                            scp049.CallCooldown = 0;

                        if (scp049.GoodSenseCooldown > 0)
                            scp049.GoodSenseCooldown = 0;

                        if (scp049.RemainingAttackCooldown > 0)
                            scp049.RemainingAttackCooldown = 0;
                    }
                    else if (player.Role is Scp106Role scp106)
                    {
                        if (scp106.CaptureCooldown > 0)
                            scp106.CaptureCooldown = 0;

                        if (scp106.RemainingSinkholeCooldown > 0)
                            scp106.RemainingSinkholeCooldown = 0;
                    }
                    else if (player.Role is Scp173Role scp173)
                    {
                        if (scp173.BlinkCooldown > 1f)
                            scp173.BlinkCooldown = 1f;

                        if (scp173.RemainingBreakneckCooldown > 1f)
                            scp173.RemainingBreakneckCooldown = 1f;
                    }
                    else if (player.Role is Scp096Role scp096)
                    {
                        if (scp096.EnrageCooldown > 0)
                            scp096.EnrageCooldown = 0;

                        if (scp096.ChargeCooldown > 0)
                            scp096.ChargeCooldown = 0;
                    }
                    else if (player.Role is Scp939Role scp939)
                    {
                        if (scp939.MimicryCooldown > 0)
                            scp939.MimicryCooldown = 0;

                        if (scp939.AmnesticCloudCooldown > 0)
                            scp939.AmnesticCloudCooldown = 0;

                        if (scp939.AttackCooldown > 0)
                            scp939.AttackCooldown = 0;
                    }
                    else if (player.Role is Scp079Role scp079)
                    {
                        if (scp079.BlackoutZoneCooldown > 0)
                            scp079.BlackoutZoneCooldown = 0;

                        if (scp079.RoomLockdownCooldown > 0)
                            scp079.RoomLockdownCooldown = 0;
                    }
                    else if (player.Role is Scp0492Role scp0492)
                    {
                    }
                    else if (player.Role is Scp3114Role scp3114)
                    {
                    }
                }

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
                player.IsUsingStamina = false;

                if (player.Role.Type == RoleTypeId.Scp0492)
                    player.MaxHealth += 100;
            }
        }


        public void OnHealing(Exiled.Events.EventArgs.Player.HealingEventArgs ev)
        {
            ev.Player.MaxHealth += ev.Amount;
        }

        public IEnumerator<float> OnUsingItem(Exiled.Events.EventArgs.Player.UsingItemEventArgs ev)
        {
            yield return Timing.WaitForOneFrame;

            ev.Cooldown = 0;
        }

        public void OnUsingRadioBattery(Exiled.Events.EventArgs.Player.UsingRadioBatteryEventArgs ev)
        {
            ev.Drain = 0;
        }

        public IEnumerator<float> OnTeleporting(Exiled.Events.EventArgs.Scp106.TeleportingEventArgs ev)
        {
            yield return Timing.WaitForOneFrame;

            ev.Scp106.RemainingSinkholeCooldown = 0;
        }

        public IEnumerator<float> OnStalking(Exiled.Events.EventArgs.Scp106.StalkingEventArgs ev)
        {
            yield return Timing.WaitForOneFrame;

            ev.Scp106.RemainingSinkholeCooldown = 0;
        }

        public IEnumerator<float> OnScp106Attacking(Exiled.Events.EventArgs.Scp106.AttackingEventArgs ev)
        {
            yield return Timing.WaitForOneFrame;

            ev.Scp106.CaptureCooldown = 0;
        }

        public IEnumerator<float> OnPlayingSound(Exiled.Events.EventArgs.Scp939.PlayingSoundEventArgs ev)
        {
            yield return Timing.WaitForOneFrame;

            ev.Scp939.MimicryCooldown = 0;
        }

        public void OnChangingCamera(Exiled.Events.EventArgs.Scp079.ChangingCameraEventArgs ev)
        {
            ev.Scp079.Energy = 100000;
        }

        public IEnumerator<float> OnStartingRecall(Exiled.Events.EventArgs.Scp049.StartingRecallEventArgs ev)
        {
            yield return Timing.WaitForOneFrame;

            ev.Scp049.RemainingCallDuration = 0;
        }

        public IEnumerator<float> OnScp049Attacking(Exiled.Events.EventArgs.Scp049.AttackingEventArgs ev)
        {
            yield return Timing.WaitForOneFrame;

            ev.Scp049.RemainingAttackCooldown = 0;
            ev.Scp049.RemainingGoodSenseDuration = 0;
        }

        public IEnumerator<float> OnEnraging(Exiled.Events.EventArgs.Scp096.EnragingEventArgs ev)
        {
            yield return Timing.WaitForOneFrame;

            ev.Scp096.EnrageCooldown = 0;
            ev.Scp096.EnragedTimeLeft = 99999;
            ev.Scp096.SprintingSpeed = 500;
        }

        public IEnumerator<float> OnPlacingTantrum(Exiled.Events.EventArgs.Scp173.PlacingTantrumEventArgs ev)
        {
            if (Tantrum >= 10)
            {
                ev.Player.AddHint("무제한 땅콩 똥 제한", $"렉 방지를 위해 10개로 제한됩니다. (하나 당 180초)", 1);
                ev.IsAllowed = false;
            }
            else
            {
                Tantrum += 1;

                yield return Timing.WaitForOneFrame;

                ev.Cooldown.Remaining = 0;

                yield return Timing.WaitForSeconds(180f);

                Tantrum -= 1;
            }
        }

        public IEnumerator<float> OnUsingBreakneckSpeeds(Exiled.Events.EventArgs.Scp173.UsingBreakneckSpeedsEventArgs ev)
        {
            yield return Timing.WaitForSeconds(1f);

            ev.Scp173.RemainingBreakneckCooldown = 0;
        }

        public void OnShooting(Exiled.Events.EventArgs.Player.ShootingEventArgs ev)
        {
            ev.Player.CurrentItem.As<Exiled.API.Features.Items.Firearm>().MagazineAmmo = 250;
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
            ev.Jailbird.TotalCharges = 0;
            ev.Jailbird.TotalDamageDealt = 0;
        }
    }
}
