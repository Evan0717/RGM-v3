using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Enums;
using MEC;
using PlayerRoles;
using Exiled.Events.EventArgs.Player;
using static RGM.Variables.Variable;
using RGM.API.Features;
using MapGeneration.Holidays;
using UnityEngine;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.Spirit)]
    class Spirit : Mode
    {
        public override string Name => "스피릿";
        public override string Description => "죽으면 영혼 상태에 돌입합니다!";
        public override string Detail =>
"""
죽으면 영혼 상태로 부활합니다. 이 상태에서 사망하면 성불됩니다.
또한, 자살로 사망한 경우 곧바로 성불됩니다.
""";
        public override string Color => "CED8F6";

        public static Spirit Instance;

        List<Player> spirits = new List<Player>();

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.Shot += OnShot;
            Exiled.Events.Handlers.Player.Hurt += OnHurt;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Player.Died -= OnDied;
            Exiled.Events.Handlers.Player.Shot -= OnShot;
            Exiled.Events.Handlers.Player.Hurt -= OnHurt;

            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            if (Random.Range(1, 101) <= 10) { //10% 확률로 워크스테이션 업그레이드 시작
                Tools.TryInstallMode(ModeType.ABattle);
            }
            while (true)
            {
                foreach (var player in PlayerManager.List)
                {
                    if (spirits.Contains(player))
                    {
                        player.EnableEffect(EffectType.Invisible);
                        if (!HolidayUtils.IsHolidayActive(HolidayType.Halloween))
                            player.EnableEffect(EffectType.Ghostly);
                    }
                }

                yield return Timing.WaitForSeconds(1.5f);
            }
        }

        public void OnHurting(HurtingEventArgs ev)
        {
            if (spirits.Contains(ev.Player))
            {
                if (GodModePlayers.Contains(ev.Player))
                    GodModePlayers.Remove(ev.Player);

                ev.Player.Kill("유령은 퇴치당했습니다.");
            }
        }

        public IEnumerator<float> OnDied(DiedEventArgs ev)
        {
            if (spirits.Contains(ev.Player) || ev.DamageHandler.Type == DamageType.Falldown || ev.DamageHandler.Type == DamageType.Warhead)
            {
                ev.Player.AddHint("스피릿 성불", $"성불했습니다..", 3);
                spirits.Remove(ev.Player);
            }
            else
            {
                for (int i = 1; i < 6; i++)
                {
                    ev.Player.ShowHint($"{6 - i}초 뒤 영혼 상태에 돌입합니다.", 1.2f);

                    yield return Timing.WaitForSeconds(1f);
                }

                Timing.CallDelayed(1f, () =>
                {
                    ev.Player.MaxHealth = 1;
                    ev.Player.Health = ev.Player.MaxHealth;
                    ev.Player.EnableEffect(EffectType.Ghostly);
                });

                spirits.Add(ev.Player);

                ev.Player.Role.Set(RoleTypeId.Tutorial, RoleSpawnFlags.None);
            }
        }

        public void OnShot(ShotEventArgs ev)
        {
            if (spirits.Contains(ev.Player))
                ev.Player.DisableEffect(EffectType.Invisible);
        }

        public void OnHurt(HurtEventArgs ev)
        {
            if (ev.Attacker != null && spirits.Contains(ev.Attacker))
                ev.Attacker.DisableEffect(EffectType.Invisible);

            if (spirits.Contains(ev.Player))
                ev.Player.DisableEffect(EffectType.Invisible);
        }
    }
}
