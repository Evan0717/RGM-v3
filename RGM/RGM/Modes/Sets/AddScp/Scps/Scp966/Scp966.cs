using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp3114;
using MEC;
using PlayerRoles;
using ProjectMER.Events.Arguments;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using RemoteAdmin;
using RGM.API.Components;
using RGM.API.Features;
using RGM.Modes.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static RGM.Variables.Variable;

namespace RGM.Modes.Sets.AddScp.Scps
{
    public static class Scp966
    {
        public static Player Create(Player player)
        {
            player.Role.Set(RoleTypeId.Scp3114, RoleSpawnFlags.None);
            player.MaxHealth = 966;
            player.Health = player.MaxHealth;
            player.EnableEffect(EffectType.Slowness, 20);
            player.AddHint("SCP-966 설명",
"""
<size=25>
당신은 <color=red>SCP-966</color>(<color=#f4fe48>Euclid</color>)입니다.
</size>
<size=20>
인간의 눈에는 보이지 않는 특수한 개체입니다.
• 공격이 매우 약합니다.
• 이동속도가 매우 느립니다.
• 동료 <color=red>SCP</color>가 전부 사망하면 모습이 드러납니다.
</size>
""", 20);

            IEnumerator<float> main()
            {
                while (PlayerManager.List.Count(x => x.IsScpRole()) > 1)
                {
                    if (!player.IsEffectActive<Invisible>())
                    {
                        player.EnableEffect(EffectType.Invisible, 1);
                    }

                    yield return Timing.WaitForOneFrame;
                }

                player.DisableEffect(EffectType.Invisible);
                player.EnableEffect(EffectType.Fade, 230);
            }

            var main_c = Timing.RunCoroutine(main());

            void OnStrangling(StranglingEventArgs ev)
            {
                if (ev.Player == player)
                {
                    ev.IsAllowed = false;
                }
            }

            void OnDisguising(DisguisingEventArgs ev)
            {
                if (ev.Player == player)
                {
                    ev.IsAllowed = false;
                }
            }

            void OnHurting(HurtingEventArgs ev)
            {
                if (ev.Attacker != null && ev.Attacker == player)
                {
                    ev.DamageHandler.Damage /= 2;
                }
            }

            void OnDying(DyingEventArgs ev)
            {
                if (ev.Player == player)
                {
                    Vector3 pos = ev.Player.Position;

                    Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                    {
                        if (ev.Player.IsDead)
                        {
                            if (ev.Player == player)
                            {
                                Timing.KillCoroutines(main_c);

                                Exiled.Events.Handlers.Scp3114.Strangling -= OnStrangling;
                                Exiled.Events.Handlers.Scp3114.Disguising -= OnDisguising;
                                Exiled.Events.Handlers.Player.Hurting -= OnHurting;
                                Exiled.Events.Handlers.Player.Dying -= OnDying;
                            }
                        }
                    });
                }
            }

            Exiled.Events.Handlers.Scp3114.Strangling += OnStrangling;
            Exiled.Events.Handlers.Scp3114.Disguising += OnDisguising;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            return player;
        }
    }
}
