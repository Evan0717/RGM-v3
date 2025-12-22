using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
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
    public static class Scp008
    {
        public static Player Create(Player player)
        {
            player.MaxHealth = 800;
            player.Health = player.MaxHealth;
            player.AddHint("SCP-008 설명",
"""
<size=25>
당신은 <color=red>SCP-008</color>(<color=#f4fe48>Euclid</color>)입니다.
</size>
<size=20>
당신은 이미 감염되었으며, 3분 뒤 <color=red>SCP-049-2</color>로 변모합니다.
• 변모 시, 주변의 인간들을 전부 <color=red>SCP-049-2</color>로 만듭니다.
• 인간을 사망에 이르게 한 경우 그 대상도 <color=red>SCP-049-2</color>로 만듭니다.
</size>
""", 20);
                

            IEnumerator<float> main()
            {
                yield return Timing.WaitForSeconds(3 * 60);

                player.Role.Set(RoleTypeId.Scp0492, RoleSpawnFlags.AssignInventory);
                player.MaxHealth = 1500;
                player.Health += 700;
                player.EnableEffect(EffectType.MovementBoost, 25);

                foreach (var p in PlayerManager.List.Where(x => !x.IsScpRole() && !AddScpMode.SpecialScps.Contains(x) && Vector3.Distance(player.Position, x.Position) < 6))
                {
                    p.Role.Set(RoleTypeId.Scp0492, RoleSpawnFlags.None);
                    p.AddHint("SCP-008에 의해 감염됨", $"<size=25>당신은 <color=red>SCP-008-X</color>(<color=#f4fe48>Euclid</color>)입니다.</size>\n<size=20>당신은 {player.DisplayNickname}(으)로 인해 감염되었습니다.</size>", 20);
                }
            }

            var main_c = Timing.RunCoroutine(main());

            void OnDying(DyingEventArgs ev)
            {
                if (ev.Attacker != null && ev.Attacker == player)
                {
                    ev.IsAllowed = false;
                    ev.Player.Role.Set(RoleTypeId.Scp0492, RoleSpawnFlags.None);
                }

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

                                Exiled.Events.Handlers.Player.Dying -= OnDying;
                            }
                        }
                    });
                }
            }

            Exiled.Events.Handlers.Player.Dying += OnDying;
            return player;
        }
    }
}
